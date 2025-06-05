using Godot;

public partial class ShotButton : Button
{
	private void OnPressed()
	{
		CommandDispatcher.DispatchCommand((int)Command.SHOOT);
	}
}
