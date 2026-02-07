using Godot;

namespace Beyondourborders.Script.Entities.Enemies;

public partial class BossChampiPropertie : TileMapLayer
{

    [Export] private BossChampi _champi;
    public override void _Ready()
    {
        base._Ready();
        this.Hide();
        this.CollisionEnabled = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_champi.CurrentHp <= 0)
        {
            this.Show();
            this.CollisionEnabled = true;
        }
    }
}