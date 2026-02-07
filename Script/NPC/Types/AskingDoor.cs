using Godot;

namespace Beyondourborders.Script.NPC.Types;

public partial class AskingDoor : NpcBase {
	public CollisionShape2D Collision;

	public override void _Ready() {
		base._Ready();
		Collision = GetNode<CollisionShape2D>("CollisionShape2D");
	}
}
