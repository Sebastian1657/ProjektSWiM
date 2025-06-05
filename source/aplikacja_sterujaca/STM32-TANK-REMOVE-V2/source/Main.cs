using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node
{
	// UI Properties
	public Label ConnectionLabel { get; private set; }
	public Label LastCommandLabel { get; private set; }
	public ItemList DeviceList { get; private set; }
	public Button ScanButton { get; private set; }
	public Button ConnectButton { get; private set; }
	public TextEdit CommandWriter { get; private set; }
	public Button SendCommandButton { get; private set; }
	public MarginContainer Interface { get; private set; }
	public Control Scanner { get; private set; }

	// State Variables
	private bool _isDriveJoystickPressed = false;
	private int _driveJoystickCounter = 20; 
	private bool _isTurretJoystickPressed = false;
	private int _turretJoystickCounter = 20;
	private readonly Dictionary<string, string> _discoveredDevices = new Dictionary<string, string>();

	// Initialization
	public override void _Ready()
	{
		InitializeUIElements();
		ConnectUISignals();
		SetupInitialState();
	}

	private void InitializeUIElements()
	{
		ConnectionLabel = GetNode<Label>("Interface/LayoutContainer/ControlManagerContainer/InfoContainer/IndicatorsContainer/ConnectionContainer/ConnectionContainer/ConnectionLabel");
		LastCommandLabel = GetNode<Label>("Interface/LayoutContainer/ControlManagerContainer/InfoContainer/IndicatorsContainer/CommandContainer/CommandContainer/ConnectionLabel");
		DeviceList = GetNode<ItemList>("Scanner/VBoxContainer/ItemList");
		ScanButton = GetNode<Button>("Scanner/VBoxContainer/HBoxContainer/ScanButton");
		ConnectButton = GetNode<Button>("Scanner/VBoxContainer/HBoxContainer/ConnectButton");
		Interface = GetNode<MarginContainer>("Interface");
		Scanner = GetNode<Control>("Scanner");
	}

	private void ConnectUISignals()
	{
		ScanButton.Connect("pressed", this, nameof(OnScanButtonPressed));
		ConnectButton.Connect("pressed", this, nameof(OnConnectButtonPressed));
	}

	private void SetupInitialState()
	{
		CommandDispatcher.Init(this);
		ConnectionLabel.Text = "OFFLINE";
		LastCommandLabel.Text = "None";
		Interface.Visible = false;
		Scanner.Visible = true;
	}

	public void OnScanButtonPressed()
	{
		DeviceList.Clear();
		_discoveredDevices.Clear();
		CommandDispatcher.StartScan();
	}

	public void OnConnectButtonPressed()
	{
		if (DeviceList.GetSelectedItems().Length > 0)
		{
			int selectedIndex = DeviceList.GetSelectedItems()[0];
			string address = _discoveredDevices.Keys.ToArray()[selectedIndex];
			CommandDispatcher.ConnectToDevice(address);
		}
	}

	// Bluetooth Signal Handlers
	public void OnConnectionStatusChange(string status)
	{
		GD.Print($"Connection status changed: {status}");
		if (status == "connected")
		{
			Interface.Visible = true;
			Scanner.Visible = false;
			ConnectionLabel.Text = "ONLINE";
			ConnectionLabel.AddColorOverride("font_color", Colors.Green);
			CommandDispatcher.ListServicesAndCharacteristics();
			CommandDispatcher.SetConnected(true);
		}
		else if (status == "disconnected")
		{
			Scanner.Visible = true;
			Interface.Visible = false;
			ConnectionLabel.Text = "OFFLINE";
			ConnectionLabel.AddColorOverride("font_color", Colors.Red);
		}
		else
		{
			GD.PrintErr($"Connection error: {status}");
		}
	}

	public void OnDeviceFound(Godot.Collections.Dictionary device)
	{
		string address = device["address"].ToString();
		string name = device["name"].ToString();
		if (!_discoveredDevices.ContainsKey(address))
		{
			_discoveredDevices[address] = name;
			DeviceList.AddItem($"{name} ({address})");
		}
	}

	public void OnDebugMessage(string message)
	{
		GD.Print($"[DEBUG] {message}");
	}

	public void OnBluetoothStatusChange(string status)
	{
		GD.Print($"Bluetooth status: {status}");
	}

	public void OnLocationStatusChange(string status)
	{
		GD.Print($"Location status: {status}");
	}

	public void OnCharacteristicFound(Godot.Collections.Dictionary characteristic)
	{
		GD.Print($"Characteristic found: {characteristic}");
	}

	public void OnCharacteristicFinding(string status)
	{
		GD.Print($"Characteristic finding: {status}");
		if (status == "done")
		{
			CommandDispatcher.SetServicesListed(true);
		}
	}

	public void OnCharacteristicRead(Godot.Collections.Dictionary data)
	{
		GD.Print($"Characteristic read: {data}");
	}

	// Cleanup
	public override void _ExitTree()
	{
		CommandDispatcher.Deinit();
	}

	// Input Handling
	public override void _PhysicsProcess(float delta)
	{
		UpdateLastCommandLabel();
		HandleDriveJoystick();
		HandleTurretJoystick();
	}

	private void HandleDriveJoystick()
	{
		if (Input.IsActionPressed("joystick_left") || Input.IsActionPressed("joystick_right") ||
			Input.IsActionPressed("joystick_up") || Input.IsActionPressed("joystick_down"))
		{
			CommandDispatcher.DispatchCommand((int)Command.DRIVE);
			_isDriveJoystickPressed = true;
			_driveJoystickCounter = 20;
		}
		else if (_isDriveJoystickPressed)
		{
			if (_driveJoystickCounter > 0)
			{
				_driveJoystickCounter--;
				CommandDispatcher.DispatchCommand((int)Command.DRIVE_STOP);
			} 
			else
			{
				_isDriveJoystickPressed = false;
				_driveJoystickCounter = 20;
			}
		}
	
	}

	private void HandleTurretJoystick()
	{
		if (Input.IsActionPressed("vertical_joystick_up") || Input.IsActionPressed("vertical_joystick_down"))
		{
			CommandDispatcher.DispatchCommand((int)Command.TURRET_ROTATE);
			_isTurretJoystickPressed = true;
			_turretJoystickCounter = 20;
		}
		else if (_isTurretJoystickPressed)
		{
			if (_turretJoystickCounter > 0)
			{
				_turretJoystickCounter--;
				CommandDispatcher.DispatchCommand((int)Command.TURRET_ROTATE_STOP);
			}
			else
			{
				_isTurretJoystickPressed = false;
				_turretJoystickCounter = 20;
			}
		}
	
	}

	public void UpdateLastCommandLabel()
	{
		switch (CommandDispatcher.LastCommand)
		{
			case Command.SHOOT: LastCommandLabel.Text = "Shoot"; break;
			case Command.DRIVE: LastCommandLabel.Text = "Drive"; break;
			case Command.DRIVE_STOP: LastCommandLabel.Text = "Drive Stop"; break;
			case Command.TURRET_ROTATE: LastCommandLabel.Text = "Turret Rotate"; break;
			case Command.TURRET_ROTATE_STOP: LastCommandLabel.Text = "Turret Rotate Stop"; break;
			case Command.MODE_DRIVE_SLOW: LastCommandLabel.Text = "Drive Slow Mode"; break;
			case Command.MODE_DRIVE_FAST: LastCommandLabel.Text = "Drive Fast Mode"; break;
			case Command.MODE_MANUAL: LastCommandLabel.Text = "Manual Mode"; break;
			case Command.MODE_FOLLOW_LINE: LastCommandLabel.Text = "Follow Line Mode"; break;
			case Command.MODE_AVOID_OBSTACLES: LastCommandLabel.Text = "Avoid Obstacles Mode"; break;
			case Command.TEXT: LastCommandLabel.Text = "Text: " + CommandDispatcher.LastTextCommand; break;
			default: LastCommandLabel.Text = "None"; break;
		}
	}
}
