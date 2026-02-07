using System.Linq.Expressions;
using Beyondourborders.Script.Entities.Players.Properties;
using Beyondourborders.Script.Weapons;
using Godot;
using WeaponBase = Beyondourborders.Script.Weapons.WeaponBase;

namespace Beyondourborders.Script.Entities.Enemies;

public partial class BossChampi : EnemyBase
{
    public BossChampi() : base(hp:200, speed:400f, jumpVelocity:0) { }

    private int i = 1;
    private double _middleX;
    
    [Export] private int RightLimit;
    [Export] private int LeftLimit;
    
    private bool GoLeft = true;
    private WeaponBase Weapon2;
    private WeaponBase Weapon3;
    private WeaponBase Weapon4;
    private WeaponBase Weapon5;

    private Spore Spore;
    private Spore Spore2;
    private Spore Spore3;
    private Spore Spore4;
    private Spore Spore5;

    private bool _first
    {
        get => Spore.CurrentHp > 0;
    }
    private bool _second {
        get => Spore2.CurrentHp > 0;
    }
    private bool _third  {
        get => Spore3.CurrentHp > 0;
    }
    private bool _fourth  {
        get => Spore4.CurrentHp > 0;
    }
    private bool _fifith  {
        get => Spore5.CurrentHp > 0;
    }

    private bool _done = false;

    private bool _activate = false;
    private bool _fuit = false;

    private bool _isInvocing = false;
    private bool _canInvoc
    {
        get => !(_first && _second && _third && _fourth && _fifith);
    }
    private CollisionShape2D _coll;
    

    public override bool AttackCondition()
    {
        return !AttackTimer.IsStopped() && AttackTimer.WaitTime - AttackTimer.TimeLeft <= 1.4 && !(SmallBobOnDetectionArea || BigBobOnDetectionArea) && !_isInvocing;
    }

    public override void _Ready()
    {
        base._Ready();
        Weapon = GetNode<WeaponBase>("Weapon");
        Weapon2 = GetNode<WeaponBase>("Weapon2");
        Weapon3 = GetNode<WeaponBase>("Weapon3");
        Weapon4 = GetNode<WeaponBase>("Weapon4");
        Weapon5 = GetNode<WeaponBase>("Weapon5");
        _coll = GetNode<CollisionShape2D>("CollisionShape2D2");

        Spore = GetNode<Spore>("Spore");
        Spore2 = GetNode<Spore>("Spore2");
        Spore3 = GetNode<Spore>("Spore3");
        Spore4 = GetNode<Spore>("Spore4");
        Spore5 = GetNode<Spore>("Spore5");

        Spore.Disapear();
        Spore2.Disapear();
        Spore3.Disapear();
        Spore4.Disapear();
        Spore5.Disapear();

        
        _middleX = DetectionArea.GlobalPosition.X;
        Weapon.HitArea.Active = false;
        Weapon2.HitArea.Active = false;
        Weapon3.HitArea.Active = false;
        Weapon4.HitArea.Active = false;
        Weapon5.HitArea.Active = false;
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (CurrentHp > 0)
        {
            var velocity = Velocity;
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }

            if (!(SmallBobOnDetectionArea || BigBobOnDetectionArea) && !_fuit)
            {
                attack();
            }
            else
            {
                _fuit = true;
                if (Position.X > 2500)
                {
                    GD.Print("PosX: " + Position.X);
                    velocity.X = -Speed;
                }
                else
                {
                    velocity = Vector2.Zero;
                    PlayerSprite.FlipH = false;
                    if (AttackTimer.IsStopped())
                    {
                        _isInvocing = false;
                    }
                    if (_isInvocing)
                    {
                        if (AttackTimer.WaitTime - AttackTimer.TimeLeft >= 0.624 && !_done)
                        {
                            Invoc();
                        }
                    }
                    else if (_canInvoc)
                    {
                        if (AttackTimer.IsStopped())
                        {
                            AttackTimer.WaitTime = 2;
                            AttackTimer.Start();
                            _done = false;
                            _isInvocing = true;
                        }
                    }
                }
               
                
            }
            
            Velocity = velocity;
            MoveAndSlide();
            UpdateAnimation();
        }
        else
        {
            _coll.Disabled = true;
        }
    }
    
    private Vector2 fuite(Vector2 velocity)
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

        return velocity;
    }

    private void attack()
    {
        if (AttackTimer.IsStopped())
        {
            AttackTimer.Start();
        }
        else if (AttackTimer.WaitTime - AttackTimer.TimeLeft <= 0.5)
        {
            _activate = false;
        }
        else if (!_activate && AttackTimer.WaitTime - AttackTimer.TimeLeft >= 1 &&
                 AttackTimer.WaitTime - AttackTimer.TimeLeft <= 1.5)
        {
            _activate = true;
            if (i == 1)
            {
                Weapon.Visibility = true;
                Weapon.Show();
                Weapon.Start();        
                Weapon.HitArea.Active = true;

            }
            else if (i == 2)
            {
                Weapon2.Visibility = true;
                Weapon2.Start();
                Weapon2.Show();
                Weapon2.HitArea.Active = true;

            }
            else if (i == 3)
            {
                Weapon3.Visibility = true;
                Weapon3.Start();
                Weapon3.Show();                
                Weapon3.HitArea.Active = true;

            }
            else if (i == 4)
            {
                Weapon4.Visibility = true;
                Weapon4.Start();
                Weapon4.Show();
                Weapon4.HitArea.Active = true;

            }
            else
            {
                Weapon5.Visibility = true;
                Weapon5.Start();
                Weapon5.Show();
                Weapon5.HitArea.Active = true;

            }

            i++;
            if (i > 5)
            {
                i -= 5;
            }
        }
    }
    private void Invoc()
    {
        Vector2 vec = Vector2.Zero;
        if (PlayerSprite.FlipH)
        {
            vec.X -= 4;
            vec.Y -= 6;
            if (!_first)
            {
                Spore.Apear(vec);
                Spore.GoLeft = false;
            }
            else if (!_second)
            {
                Spore2.Apear(vec);
                Spore2.GoLeft = false;
            }
            else if (!_third)
            {
                Spore3.Apear(vec);
                Spore3.GoLeft = false;
            }
            else if (!_fourth)
            {
                Spore4.Apear(vec);
                Spore4.GoLeft = false;
            }
            else if (!_fifith)
            {
                Spore5.Apear(vec);
                Spore5.GoLeft = false;
            }
        }
        else
        {
            vec.X += 5;
            vec.Y -= 6;
            if (!_first)
            {
                Spore.Apear(vec);
                Spore.GoLeft = true;
            }
            else if (!_second)
            {
                Spore2.Apear(vec);
                Spore2.GoLeft = true;
            }
            else if (!_third)
            {
                Spore3.Apear(vec);
                Spore3.GoLeft = true;
            }
            else if (!_fourth)
            {
                Spore4.Apear(vec);
                Spore4.GoLeft = true;
            }
            else if (!_fifith)
            {
                Spore5.Apear(vec);
                Spore5.GoLeft = true;
            }
        }
        _done = true;

    }

    public override void UpdateAnimation()
    {
        AnimationType newAnimation;

        if (CurrentHp <= 0)
        {
            newAnimation = AnimationType.Death;
        }
        else if (TakingDamage)
        {
            newAnimation = AnimationType.Damage;
        }
        else if (_isInvocing)
        {
            if (AttackTimer.WaitTime - AttackTimer.TimeLeft <= 1)
            {
                newAnimation = AnimationType.Invocation;
            }
            else
            {
                newAnimation = AnimationType.Idle;
            }
        }
        else if (AttackCondition())
        {
            newAnimation = AnimationType.Attack;
        }
        else if (Velocity.X == 0 && Velocity.Y == 0) // Not moving
        {
            newAnimation = AnimationType.Idle;
        }
        else if (Velocity.Y == 0) { // Moving only horizontally
            newAnimation = AnimationType.Run;
        }
        else { // Moving either only vertically or vertically + horizontally
            newAnimation = AnimationType.None; // Should be AnimationType.Jump - NOT YET IMPLEMENTED
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