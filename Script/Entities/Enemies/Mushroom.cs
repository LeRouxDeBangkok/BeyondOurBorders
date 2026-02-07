using Beyondourborders.Script.Entities.Players;
using Godot;

//TODO Debug le collision shape de ce pauvre champi
namespace Beyondourborders.Script.Entities.Enemies;

public partial class Mushroom : EnemyBase
{
    private Area2D _touchableLeft;
    private Area2D _touchableRight;
    private Area2D _tooFarLeft;
    private Area2D _tooFarRight;
    private AnimationPlayer _animationPlayer;

    public Mushroom() : base(speed: 500.0f, jumpVelocity: 0f,hp:50)
    {
    }

    public override void _Ready()
    {
        base._Ready();
        _touchableLeft = GetNode<Area2D>("touchableLeft");
        _touchableRight = GetNode<Area2D>("touchableRight");
        _tooFarLeft = GetNode<Area2D>("tooFarLeft");
        _tooFarRight = GetNode<Area2D>("tooFarRight");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Stop();
    }

    public override bool AttackCondition()
    {
        bool attack = false;
        foreach (var body in _touchableLeft.GetOverlappingBodies())
        {
            if (body is PlayerBase)
            {
                attack = true;
                break;
            }
        }
        foreach (var body in _touchableRight.GetOverlappingBodies())
        {
            if (body is PlayerBase)
            {
                attack = true;
                break;
            }
        }
        return attack  && AttackTimer.TimeLeft > 0.6;
    }
    public override void _PhysicsProcess(double delta)
    {
        //Gravity
        if(CurrentHp > 0)
        {
            Vector2 velocity = Velocity;
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta * 3.5f;
            }

            bool attack = false;
            foreach (var body in _touchableRight.GetOverlappingBodies())
            {
                if (body is PlayerBase)
                {
                    attack = true;
                    PlayerSprite.FlipH = true;
                    break;
                }
            }

            foreach (var body in _touchableLeft.GetOverlappingBodies())
            {
                if (body is PlayerBase)
                {
                    attack = true;
                    PlayerSprite.FlipH = false;
                    break;
                }
            }

            //timer
            if (attack)
            {
                if (AttackTimer.IsStopped())
                {

                    AttackTimer.Start();
                    _animationPlayer.Play("attack");
                }
            }
            else
            {
                _animationPlayer.Stop();
            }

            if (!attack)
            {
                bool aBodyIsRight = false;
                bool aBodyIsLeft = false;

                foreach (var body in _tooFarRight.GetOverlappingBodies())
                {
                    if (body is PlayerBase)
                    {
                        aBodyIsRight = true;
                        PlayerSprite.FlipH = true;
                        break;
                    }
                }

                foreach (var body in _tooFarLeft.GetOverlappingBodies())
                {
                    if (body is PlayerBase)
                    {
                        aBodyIsLeft = true;
                        PlayerSprite.FlipH = false;
                        break;
                    }
                }

                if (aBodyIsLeft)
                {
                    velocity.X = -1 * Speed;
                }
                else if (aBodyIsRight)
                {
                    velocity.X = 1 * Speed;
                }
            }

            Velocity = velocity;

            MoveAndSlide();

            UpdateAnimation();
        }
    }
}