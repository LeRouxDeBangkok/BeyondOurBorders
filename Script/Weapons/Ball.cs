using WeaponBase = Beyondourborders.Script.Weapons.WeaponBase;

using Godot;

namespace Beyondourborders.Script.Weapons;

public partial class Ball : WeaponBase
{
    public Ball() { }
    
    public override void Start()
    {
        Position = new Vector2(7,6);
        this.Show();
        Sprite.Animation = "idle";
        Sprite.Play();
        Visibility = true;

    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        velocity += GetGravity() * (float)delta * 3.5f;
        velocity.X = 500;
        Velocity = velocity;
        MoveAndSlide();
    }
}