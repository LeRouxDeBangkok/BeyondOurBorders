using Beyondourborders.Script.Entities.Players;
using Godot;

namespace Beyondourborders.Script.Weapons;

public partial class Arrow : WeaponBase
{
    [Export] private float _speed = 600f;
    [Export] private SmallBob _bob;
    private Timer _timer;
    private bool jsp;
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.WaitTime = 5f;
        base._Ready();
        Sprite.Animation = "default";
        jsp = false;
    }

    public Arrow() { }
    public override void _PhysicsProcess(double delta)
    {
        var dir = -1;
        if (Sprite.FlipH)
        {
            dir = 1;
        }

        Vector2 velocity = Velocity;
        velocity.X = dir * _speed;
        Velocity = velocity;

        MoveAndSlide();
        
    }

    public override void _Process(double delta)
    {
        if (Visibility)
        {
            HitArea.Damage = _bob.Damage;
        }
        if (_timer.IsStopped() && jsp )
        {
            Visibility = false;
            this.Hide();
            jsp = false;
        }

        else if ( !jsp && (IsOnWall() || HitArea.EnemyTouch) && _timer.IsStopped())
        {
            _timer.Start();
            jsp = true;
        }

        if (!_bob.AttackTimer.IsStopped() &&
            _bob.AttackTimer.WaitTime - _bob.AttackTimer.TimeLeft >= 0.8 && _bob.AttackTimer.WaitTime - _bob.AttackTimer.TimeLeft <= 0.85)
        {
            this.Start();
        }
        base._Process(delta);
      
    }

    public override void Start()
    {
        jsp = false;
        Visibility = true;
        if (_bob.PlayerSprite.FlipH)
        {
            Position = new Vector2(25,37);
            Sprite.FlipH = true;
        }
        else
        {
            Position = new Vector2(-21,37);
            Sprite.FlipH = false;
        }
        Show();
    }
}