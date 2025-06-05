using Godot;

public partial class ModeButton : Button
{
	private Label ModeStatusLabel;
	private Command command = Command.MODE_MANUAL;

	public override void _Ready()
	{
		ModeStatusLabel = GetNode<Label>("../../ModeStatusContainer/Label");
	}

	private void OnPressed()
	{
		switch (command)
		{
			case Command.MODE_MANUAL:           command = Command.MODE_FOLLOW_LINE;     ModeStatusLabel.Text = "FOLLOW LINE"; break;
			case Command.MODE_FOLLOW_LINE:      command = Command.MODE_AVOID_OBSTACLES; ModeStatusLabel.Text = "AUTONOMOUS";  break;
			case Command.MODE_AVOID_OBSTACLES:  command = Command.MODE_MANUAL;          ModeStatusLabel.Text = "MANUAL";      break;
		}

		CommandDispatcher.DispatchCommand((int)command);
	}
}
