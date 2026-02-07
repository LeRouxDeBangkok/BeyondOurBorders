using Godot;
using Beyondourborders.Script.Entities.Players;

namespace Beyondourborders.Script.World
{
    public partial class ItemPickup : Node2D
    {
        [Export] public global::Item ItemData;

        public override void _Ready()
        {
            GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;
            if (ItemData != null && ItemData.Icon != null)
            {
                var sprite = GetNode<Sprite2D>("Sprite2D");
                sprite.Texture = ItemData.Icon;
            }
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body is PlayerBase player && ItemData != null)
            {
                player.Collect(ItemData);
                GD.Print($"Player {player.Name} picked up: {ItemData.Name}");
                QueueFree();
            }
        }

    }
}  