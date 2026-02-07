using System.Linq;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Godot;

namespace Beyondourborders.Script.NPC;

public partial class NpcBase : CharacterBody2D {
	[Export] protected bool ShouldBeFlipped;

	protected AnimatedSprite2D Sprite;
	
	protected Area2D DetectionArea;
	
	protected NpcDialog Dialog; 

	protected bool CurrentPlayerInZone => DetectionArea.GetOverlappingBodies().Any(body => body is PlayerBase p && p == Client.Instance.CurrentPlayer!);
	protected bool CurrentPlayerInZoneRight => ShouldBeFlipped && Client.Instance.CurrentPlayer!.GlobalPosition.X > _middleX && CurrentPlayerInZone;

	private float _middleX;
	
	public override void _Ready() {
		Dialog = GetNode<NpcDialog>("RichTextLabel");
		Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		DetectionArea = GetNode<Area2D>("Area2D");
		DetectionArea.BodyEntered += OnPlayerEnter;
		DetectionArea.BodyExited += OnPlayerLeave;
		_middleX = DetectionArea.GlobalPosition.X;
	}

	public void OnPlayerEnter(Node2D body) {
		if (body is PlayerBase p && p == Client.Instance.CurrentPlayer) {
			Dialog.FancyShow();
		}
	}

	public void OnPlayerLeave(Node2D body) {
		if (body is PlayerBase p && p == Client.Instance.CurrentPlayer) {
			var myGDScript = GD.Load<GDScript>("res://Scene/Meta/node.gd");
			var myGDScriptNode = (GodotObject)myGDScript.New(); // This is a GodotObject.
			myGDScriptNode.Call("_stopTalkToNpcEvent");
			Dialog.FancyHide();
		}
	}

	public override void _Process(double delta) {
		// Note: Kinda unpotimized, should be using signals.
		if (CurrentPlayerInZoneRight) 
			Sprite.FlipH = true;
		else if (CurrentPlayerInZone)
			Sprite.FlipH = false;
		
		var myGdScript = GD.Load<GDScript>("res://Scene/Meta/node.gd");
		var myGdScriptNode = (GodotObject)myGdScript.New(); // This is a GodotObject.
		
		if (CurrentPlayerInZone && Input.IsActionJustPressed("Interact")) {
			Dialog.NextLine();
			myGdScriptNode.Call("_beginTalkToNpcEvent");
		}
	}
}
