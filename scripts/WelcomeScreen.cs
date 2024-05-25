using Godot;
using System;

public partial class WelcomeScreen : Node2D
{
	private void OnButtonPressed()
	{
		var res = GetTree().ChangeSceneToFile("res://scenes/world.tscn");
		Console.WriteLine(res.ToString());
	}
}



