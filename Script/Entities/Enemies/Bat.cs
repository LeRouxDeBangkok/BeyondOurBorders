using System.Security.Cryptography;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Entities.Players.Properties;
using Godot;

namespace Beyondourborders.Script.Entities.Enemies;

public partial class Bat : EnemyBase
{
    private Area2D _touchableLeft;
    private Area2D _touchableRight;
    [Export] private int RightLimit;
    [Export] private int LeftLimit;
    private bool GoLeft = true;
    private float y;

    public Bat() : base(speed: 275.0f, jumpVelocity: 0f,hp:25)
    {
    }

    public override void _Ready()
    {
        base._Ready();
        _touchableLeft = GetNode<Area2D>("touchableLeft");
        _touchableRight = GetNode<Area2D>("touchableRight");
        y = Position.Y;
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
        return attack  && AttackTimer.TimeLeft > 1.0;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(CurrentHp > 0)
        {
            bool attack = false;
            if (!GoLeft)
            {
                foreach (var body in _touchableRight.GetOverlappingBodies())
                {
                    if (body is PlayerBase)
                    {
                        attack = true;
                        break;
                    }
                }
            }
            else
            {
                foreach (var body in _touchableLeft.GetOverlappingBodies())
                {
                    if (body is PlayerBase)
                    {
                        attack = true;
                        break;
                    }
                }
            }

            Vector2 velocity = Velocity;
            if (attack)
            {
                if (AttackTimer.IsStopped())
                {
                    AttackTimer.Start();
                }

                if (GoLeft)
                {
                    velocity.X = -1 * Speed * 2;
                    PlayerSprite.FlipH = true;
                    if (AttackTimer.TimeLeft > 1.5)
                    {
                        velocity.Y = Speed / 2;
                    }
                    else
                    {
                        velocity.Y = -1 * Speed / 2;
                    }
                }
                else
                {
                    velocity.X = Speed * 2;
                    PlayerSprite.FlipH = false;
                    if (AttackTimer.TimeLeft > 2)
                    {
                        velocity.Y = -1 * Speed / 4;
                    }
                }

            }
            else
            {
                if (GoLeft)
                {
                    if (Position.X <= LeftLimit)
                    {
                        PlayerSprite.FlipH = false;
                        GoLeft = false;
                        velocity.X = Speed;
                    }
                    else
                    {
                        PlayerSprite.FlipH = true;
                        velocity.X = -1 * Speed;
                    }
                }
                else
                {
                    if (Position.X >= RightLimit)
                    {
                        PlayerSprite.FlipH = true;
                        GoLeft = true;
                        velocity.X = -1 * Speed;
                    }
                    else
                    {
                        PlayerSprite.FlipH = false;
                        velocity.X = Speed;
                    }
                }

                if (Position.Y > y)
                {
                    velocity.Y = -Speed / 4;
                }
                else if (Position.Y < y)
                {
                    velocity.Y = Speed / 4;
                }
            }

            Velocity = velocity;

            MoveAndSlide();

            UpdateAnimation();
        }
        else
        {
            
        }
    }
    public override void UpdateAnimation() 
    {
        AnimationType newAnimation;

        if (CurrentHp <= 0)
        {
            newAnimation = AnimationType.Death;
        }
        else if (AttackCondition())
        {
            newAnimation = AnimationType.Attack;
        }
        else { // Moving either only vertically or vertically + horizontally
            newAnimation = AnimationType.Idle; // Should be AnimationType.Jump - NOT YET IMPLEMENTED
        }
			
        // Apply new animation state
        if (newAnimation == AnimationType.None && CurrentAnimation != AnimationType.None)
        {
            // GD.Print("Set animation to None");
            CurrentAnimation = AnimationType.None;
            PlayerSprite.Stop();
        }
        else if (newAnimation != CurrentAnimation)
        {
            // GD.Print($"Stopped animation {CurrentAnimation} & set it to {newAnimation}");
            PlayerSprite.Stop();
            CurrentAnimation = newAnimation;
            PlayerSprite.Play(CurrentAnimation.ToString().ToLower());
        }
    }
}