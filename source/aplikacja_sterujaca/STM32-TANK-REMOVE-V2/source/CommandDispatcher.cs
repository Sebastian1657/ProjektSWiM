using System;
using Godot;
using System.Collections.Generic;
using System.Text;

public static class CommandDispatcher
{
	private static Godot.Object _bluetoothPlugin;
	private const string SERVICE_UUID = "0000ffe0-0000-1000-8000-00805f9b34fb";
	private const string CHARACTERISTIC_UUID = "0000ffe1-0000-1000-8000-00805f9b34fb";
	private static string _deviceAddress = null;
	private static bool _isConnected = false;
	private static bool _servicesListed = false;
	private static Command _lastCommand = Command.NONE;
	private static string _lastTextCommand = "";

	public static bool IsConnected => _isConnected;
	public static Command LastCommand => _lastCommand;
	public static string LastTextCommand => _lastTextCommand;

	// Initialization
	public static void Init(Main parent)
	{
		if (Engine.HasSingleton("GodotBluetooth344"))
		{
			_bluetoothPlugin = Engine.GetSingleton("GodotBluetooth344");
			ConnectSignals(parent);
			GD.Print("Bluetooth plugin initialized.");
		}
		else
		{
			GD.PrintErr("GodotBluetooth344 singleton not found.");
		}
	}

	private static void ConnectSignals(Main parent)
	{
		if (!_bluetoothPlugin.IsConnected("_on_connection_status_change", parent, nameof(parent.OnConnectionStatusChange)))
			_bluetoothPlugin.Connect("_on_connection_status_change", parent, nameof(parent.OnConnectionStatusChange));
		if (!_bluetoothPlugin.IsConnected("_on_device_found", parent, nameof(parent.OnDeviceFound)))
			_bluetoothPlugin.Connect("_on_device_found", parent, nameof(parent.OnDeviceFound));
		if (!_bluetoothPlugin.IsConnected("_on_debug_message", parent, nameof(parent.OnDebugMessage)))
			_bluetoothPlugin.Connect("_on_debug_message", parent, nameof(parent.OnDebugMessage));
		if (!_bluetoothPlugin.IsConnected("_on_bluetooth_status_change", parent, nameof(parent.OnBluetoothStatusChange)))
			_bluetoothPlugin.Connect("_on_bluetooth_status_change", parent, nameof(parent.OnBluetoothStatusChange));
		if (!_bluetoothPlugin.IsConnected("_on_location_status_change", parent, nameof(parent.OnLocationStatusChange)))
			_bluetoothPlugin.Connect("_on_location_status_change", parent, nameof(parent.OnLocationStatusChange));
		if (!_bluetoothPlugin.IsConnected("_on_characteristic_found", parent, nameof(parent.OnCharacteristicFound)))
			_bluetoothPlugin.Connect("_on_characteristic_found", parent, nameof(parent.OnCharacteristicFound));
		if (!_bluetoothPlugin.IsConnected("_on_characteristic_finding", parent, nameof(parent.OnCharacteristicFinding)))
			_bluetoothPlugin.Connect("_on_characteristic_finding", parent, nameof(parent.OnCharacteristicFinding));
		if (!_bluetoothPlugin.IsConnected("_on_characteristic_read", parent, nameof(parent.OnCharacteristicRead)))
			_bluetoothPlugin.Connect("_on_characteristic_read", parent, nameof(parent.OnCharacteristicRead));
	}

	// Device Management
	public static void StartScan()
	{
		if (_bluetoothPlugin != null)
		{
			_bluetoothPlugin.Call("scan");
			GD.Print("Scanning for BLE devices...");
		}
	}

	public static void ConnectToDevice(string deviceAddress)
	{
		if (_bluetoothPlugin == null)
		{
			GD.PrintErr("GodotBluetooth344 not available.");
			return;
		}
		_deviceAddress = deviceAddress;
		_bluetoothPlugin.Call("connect", deviceAddress);
		GD.Print($"Attempting to connect to {deviceAddress}");
	}

	public static void ListServicesAndCharacteristics()
	{
		if (_bluetoothPlugin != null)
		{
			_bluetoothPlugin.Call("listServicesAndCharacteristics");
			GD.Print("Listing services and characteristics...");
		}
	}
	
	public static void SetConnected(bool connected)
	{
		_isConnected = connected;
		GD.Print($"_isConnected set to: {_isConnected}");
	}

	public static void SetServicesListed(bool listed)
	{
		_servicesListed = listed;
		if (listed) GD.Print("Services and characteristics listed.");
	}

	public static void OnConnectionStatusChange(string status)
	{
		GD.Print($"Connection status changed: {status}");
		if (status == "connected")
		{
			_isConnected = true;
			GD.Print("Connected");
			ListServicesAndCharacteristics(); // Ensure services are listed after connection
		}
		else if (status == "disconnected")
		{
			_isConnected = false;
			_servicesListed = false; // Reset services listed on disconnect
			GD.Print("Disconnected");
		}
		else
		{
			_isConnected = false;
			_servicesListed = false;
			GD.PrintErr($"Connection error: {status}");
		}
	}

	// Command Dispatching
	public static void DispatchCommand(string text)
	{
		if (!_isConnected || !_servicesListed)
		{
			GD.PrintErr($"Cannot send text command '{text}': Not connected ({_isConnected}) or services not listed ({_servicesListed}).");
			return;
		}

		_bluetoothPlugin.Call("writeStringToCharacteristic", SERVICE_UUID, CHARACTERISTIC_UUID, text + "\r\n");
		GD.Print($"Sent text command: {text}");
		_lastCommand = Command.TEXT;
		_lastTextCommand = text;
	}

	public static void DispatchCommand(int commandID)
	{
		Command command = (Command)commandID;
		_lastCommand = command;

		if (!_isConnected || !_servicesListed)
		{
			GD.PrintErr($"Cannot send command {command}: Not connected ({_isConnected}) or services not listed ({_servicesListed}).");
			//return;
		}

		try
		{
			switch (command)
			{
				case Command.SHOOT:
					GD.Print("Shooting command triggered");
					DispatchCommand("S");
					break;
				case Command.DRIVE:
					GD.Print("Driving");
					HandleDriving();
					break;
				case Command.DRIVE_STOP:
					GD.Print("Driving stop");
					DispatchCommand("DS");
					break;
				case Command.TURRET_ROTATE:
					GD.Print("Turret rotating");
					HandleRotating();
					break;
				case Command.TURRET_ROTATE_STOP:
					GD.Print("Turret rotation stopped");
					DispatchCommand("TS");
					break;
				case Command.MODE_DRIVE_SLOW:
					GD.Print("Switching to slow mode");
					DispatchCommand("MDS");
					break;
				case Command.MODE_DRIVE_FAST:
					GD.Print("Switching to fast mode");
					DispatchCommand("MDF");
					break;
				case Command.MODE_MANUAL:
					GD.Print("Switching to manual mode");
					DispatchCommand("MM");
					break;
				case Command.MODE_FOLLOW_LINE:
					GD.Print("Switching to follow line mode");
					DispatchCommand("ML");
					break;
				case Command.MODE_AVOID_OBSTACLES:
					GD.Print("Switching to avoid obstacles mode");
					DispatchCommand("MA");
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(commandID), command, null);
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error dispatching command {command}: {ex.Message}");
			_lastCommand = Command.NONE;
		}
	}

	private static void HandleDriving()
	{
		float joystickLeft = Input.GetActionStrength("joystick_left");
		float joystickRight = Input.GetActionStrength("joystick_right");
		float joystickUp = Input.GetActionStrength("joystick_up");
		float joystickDown = Input.GetActionStrength("joystick_down");

		float xAxis = joystickRight - joystickLeft; // -1.0 to 1.0
		float yAxis = joystickUp - joystickDown;   // -1.0 to 1.0

		string command = $"D:{xAxis:F2},{yAxis:F2}";
		DispatchCommand(command);
	}

	private static void HandleRotating()
	{
		float verticalJoystickUp = Input.GetActionStrength("vertical_joystick_up");
		float verticalJoystickDown = Input.GetActionStrength("vertical_joystick_down");

		float xAxis = verticalJoystickUp - verticalJoystickDown;
		string command = $"T:{xAxis:F2}";
		DispatchCommand(command);
	}

	// Cleanup
	public static void Deinit()
	{
		if (_bluetoothPlugin != null && _isConnected)
		{
			_bluetoothPlugin.Call("disconnect");
		}
		_deviceAddress = null;
		_isConnected = false;
		_servicesListed = false;
		_lastCommand = Command.NONE;
		_lastTextCommand = "";
		GD.Print("Bluetooth plugin deinitialized.");
	}
}
