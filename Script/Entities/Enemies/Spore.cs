using System.ComponentModel.DataAnnotations.Schema;
using Beyondourborders.Script.Entities.Enemies;
using Godot;

namespace Beyondourborders.Script.Weapons;

public partial class Spore : EnemyBase
{
    [Export] private int RightLimit;
    [Export] private int LeftLimit;
    public bool GoLeft = true;
    
    public Spore() : base(hp : 15, jumpVelocity:-300,speed:300){}
    public override bool AttackCondition()
    {
        return false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if(CurrentHp > 0)
        {
            var velocity = Velocity;
            if (!AttackTimer.IsStopped())
            {
                velocity.Y = JumpVelocity;
            }
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta * 3.5f;
            }
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

            Velocity = velocity;
            MoveAndSlide();
        }
    }

    public override void Apear(Vector2 pos)
    {
        base.Apear(pos);
        AttackTimer.Start();   
    }

    public override void UpdateAnimation()
    {
        base.UpdateAnimation();
    }
}