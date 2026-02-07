using Godot;

namespace Beyondourborders.Script.Weapons;

public partial class Sword : WeaponBase
{
    [Export] private float _speed = 200f;
    [Export] private float _direction = -1; //right = 1 left = -1
    public override void _Ready()
    {
        base._Ready();
        Sprite.Animation = "default";
    }

    public Sword(){ }
    public override void _PhysicsProcess(double delta)
    {
        if(Sprite.Visible)
        {
            Vector2 velocity = Velocity;
            Vector2 direction = new Vector2(_direction, 0.0f);
            velocity.X = direction.X * _speed;
            Velocity = velocity;

            MoveAndSlide();
        }
        else
        {
            HitArea.Active = false;
        }
    }

    public override void Start()
    {
        Position = new Vector2(29,2);
        Sprite.Stop();
        Sprite.Play();
        HitArea.Active = true;
        CollisionShape.Disabled = false;
        Visibility = true;
        Show();
    }

    public void Stop()
    {
        CollisionShape.Disabled = true;
        Position = new Vector2(29,2);
        Sprite.Stop();
        HitArea.Active = false;
        Visibility = false;
        Hide();
    }
}