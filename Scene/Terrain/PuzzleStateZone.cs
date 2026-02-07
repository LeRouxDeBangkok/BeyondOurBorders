using Godot;
using System;

public partial class PuzzleStateZone : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void BodyEntered(Node2D body){
		var myGDScript = GD.Load<GDScript>("res://Scene/Meta/node.gd");
		var myGDScriptNode = (GodotObject)myGDScript.New(); // This is a GodotObject.
		myGDScriptNode.Call("_enterPuzzleAreaEvent()");
		GD.Print("OK");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
