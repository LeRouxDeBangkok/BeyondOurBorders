using Beyondourborders.Script.Weapons;
using Beyondourborders.Script.Weapons;
using Godot;
using Sword = Beyondourborders.Script.Weapons.Sword;
using WeaponBase = Beyondourborders.Script.Weapons.WeaponBase;

namespace Beyondourborders.Script.Entities.Enemies;

public partial class Skeleton : EnemyBase
{

    public Skeleton() : base(speed: 0f, jumpVelocity: 0f, hp:50) { }

    public override void _Ready()
    {
        base._Ready();
        Weapon = GetNode<WeaponBase>("Weapon");
    }

    public override bool AttackCondition()
    {
        return (SmallBobOnDetectionArea || BigBobOnDetectionArea) && AttackTimer.WaitTime - AttackTimer.TimeLeft <= 1;
    }

    public override void _PhysicsProcess(double delta)
    {
        Sword _sword = (Sword)Weapon;
        if(CurrentHp > 0)
        {
            //sword
            if (!AttackTimer.IsStopped() && AttackTimer.TimeLeft <= 2.05f && AttackTimer.TimeLeft >= 2f)
            {
                _sword.Start();
            }
            else if (AttackTimer.IsStopped() || AttackTimer.TimeLeft > 2f || AttackTimer.TimeLeft < 1f)
            {
                _sword.Stop();
            }

            //timer
            if (SmallBobOnDetectionArea || BigBobOnDetectionArea)
            {
                if (AttackTimer.IsStopped())
                {
                    AttackTimer.Start();
                }
            }
            else
            {
                AttackTimer.Stop();
            }

            // ===== Physique ===== 

            Vector2 velocity = Velocity;

            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }

            Velocity = velocity;

            MoveAndSlide();

            UpdateAnimation();
        }
        else
        {
            _sword.Stop();
            HitZone.Active = false;
        }
    }
}