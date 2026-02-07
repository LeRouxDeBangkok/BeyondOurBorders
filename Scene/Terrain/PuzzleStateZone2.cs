using Godot;
using System;

public partial class PuzzleStateZone2 : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void BodyEntered(Node2D body){
		var myGDScript = GD.Load<GDScript>("res://Scene/Meta/node.gd");
		var myGDScriptNode = (GodotObject)myGDScript.New(); // This is a GodotObject.
		myGDScriptNode.Call("_leavePuzzleAreaEvent()");
	}

	// Called eve 		ry frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
