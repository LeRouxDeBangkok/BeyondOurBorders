using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Triggers;

// [Tool]
public partial class AreaSwitchTrigger : SimpleResizableArea {
	[Export] public string TargetZone { get; set; } = null;
	[Export] public Vector2 SpawnCoordinatesMain { get; set; }
	[Export] public Vector2 SpawnCoordinatesOther { get; set; }
	
	public AreaSwitchTrigger() : base("AreaSwitchTrigger") {}
	
	public override void _Ready() {
		base._Ready();
		BodyEntered += TeleportPlayer;
	}
	
	public void TeleportPlayer(Node2D body) {
		if (!IsCurrentActivePlayer(body))
			return;
		
		GD.Print("Teleported Player " + body.Name + " to zone " + TargetZone + " @ " + SpawnCoordinatesMain + " | " + SpawnCoordinatesOther);
		
		Client.Instance.LoadZone(TargetZone, SpawnCoordinatesMain, SpawnCoordinatesOther);
	}
}
