using System;
using Godot;

public partial class FastModeButton : Button
{
	private void OnButtonToggled(bool toggledOn)
	{
		if (toggledOn)
		{
			CommandDispatcher.DispatchCommand((int)Command.MODE_DRIVE_FAST);
		}
		else
		{
			CommandDispatcher.DispatchCommand((int)Command.MODE_DRIVE_SLOW);
		}
	}
}
