using Godot;

public partial class VerticalJoystick : Control
{
	[Export]
	public Color PressedColor { get; set; } = Colors.Gray;

	[Export(PropertyHint.Range, "0,200,1")]
	public float DeadzoneSize { get; set; } = 10f;

	[Export(PropertyHint.Range, "0,500,1")]
	public float ClampzoneSize { get; set; } = 75f;

	[Export]
	public string ActionUp { get; set; } = "vertical_joystick_up";

	[Export]
	public string ActionDown { get; set; } = "vertical_joystick_down";

	public bool IsPressed { get; private set; }
	public Vector2 Output { get; private set; } = Vector2.Zero;

	private int _touchIndex = -1;
	private TextureRect _base;
	private TextureRect _tip;
	private Vector2 _tipDefaultPosition;
	private Color _defaultColor;
	private Vector2 _lastTouchPosition;

	public override void _Ready()
	{
		_base = (TextureRect)GetNode("MarginContainer/TextureRect1");
		_tip = (TextureRect)GetNode("MarginContainer/MarginContainer/TextureRect3");
		_tipDefaultPosition = _tip.RectPosition;
		_defaultColor = _tip.Modulate;
		_tip.RectPivotOffset = _tip.RectSize / 2f;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.Pressed && IsPointInsideBase(touchEvent.Position) && _touchIndex == -1)
			{
				_touchIndex = touchEvent.Index;
				_tip.Modulate = PressedColor;
				_lastTouchPosition = touchEvent.Position;
				IsPressed = true;
				UpdateJoystick(touchEvent.Position);
			}
			else if (!touchEvent.Pressed && touchEvent.Index == _touchIndex)
			{
				Reset();
			}
		}
		else if (@event is InputEventScreenDrag dragEvent && dragEvent.Index == _touchIndex)
		{
			_lastTouchPosition = dragEvent.Position;
			UpdateJoystick(dragEvent.Position);
		}
	}

	public override void _Process(float delta)
	{
		if (IsPressed && _touchIndex != -1)
		{
			UpdateJoystick(_lastTouchPosition);
		}
	}

	private bool IsPointInsideBase(Vector2 point)
	{
		return _base.GetGlobalRect().HasPoint(point);
	}

	private void UpdateJoystick(Vector2 touchPosition)
	{
		// Calculate the base's global center
		var baseRect = _base.GetGlobalRect();
		var center = baseRect.Position + (baseRect.Size / 2f);

		// Calculate the vector from center to touch position, restrict to vertical
		var vector = touchPosition - center;
		vector.x = 0; // Enforce vertical-only movement

		// Clamp the vector to ClampzoneSize
		if (Mathf.Abs(vector.y) > ClampzoneSize)
		{
			vector.y = Mathf.Sign(vector.y) * ClampzoneSize;
		}

		// Desired global position of the tip's center
		var globalTipCenter = center + vector;

		// Convert to local coordinates relative to the tip's parent (MarginContainer)
		MarginContainer tipParent = (MarginContainer)_tip.GetParent();
		var localTipCenter = tipParent.GetGlobalTransform().AffineInverse().Xform(globalTipCenter);

		// Set the tip's position, adjusting for the pivot offset
		_tip.RectPosition = localTipCenter - (_tip.RectSize / 2f);

		// Handle deadzone and output
		if (Mathf.Abs(vector.y) > DeadzoneSize)
		{
			IsPressed = true;
			float outputY = vector.y / ClampzoneSize;
			Output = new Vector2(0, outputY);
		}
		else
		{
			IsPressed = false;
			Output = Vector2.Zero;
		}

		// Update input actions
		if (Output.y >= 0 && Input.IsActionPressed(ActionUp))
			Input.ActionRelease(ActionUp);
		if (Output.y <= 0 && Input.IsActionPressed(ActionDown))
			Input.ActionRelease(ActionDown);

		if (Output.y < 0)
			Input.ActionPress(ActionUp, -Output.y);
		if (Output.y > 0)
			Input.ActionPress(ActionDown, Output.y);
	}

	private void Reset()
	{
		IsPressed = false;
		Output = Vector2.Zero;
		_touchIndex = -1;
		_tip.Modulate = _defaultColor;
		_tip.RectPosition = _tipDefaultPosition;

		Input.ActionRelease(ActionUp);
		Input.ActionRelease(ActionDown);
	}
}
