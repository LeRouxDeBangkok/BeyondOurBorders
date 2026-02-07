using System;
using System.Collections.Generic;
using System.Linq;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Entities.Players.Properties;
using Beyondourborders.Script.Overlay;
using Beyondourborders.Script.Weapons;
using Godot;

namespace Beyondourborders.Script.Entities.Enemies;

public partial class FinalBoss : EnemyBase
{
    public FinalBoss() : base(speed:0,jumpVelocity:0,hp:500)
    {}
    private Area2D _DetectionZone2;
    private Area2D _DetectionFarZone;
    private Area2D _DetectionFarZone2;
    private EnemyBase Mushroom1;
    private EnemyBase Mushroom2;
    private EnemyBase Bat1;
    private EnemyBase Bat2;
    private EnemyBase Bat3;
    private EnemyBase Bat4;
    private EnemyBase Bat5;
    private EnemyBase Goblin;
    private EnemyBase Skeleton;

    private Random _random = new Random(7355608);
    
    private bool _smallAttack
    {
        get
        {
            foreach (var Body in DetectionArea.GetOverlappingBodies())
            {
                if (Body is PlayerBase)
                {
                    return true;
                }
            }
            foreach (var Body in _DetectionZone2.GetOverlappingBodies())
            {
                if (Body is PlayerBase)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private bool _longAttack
    {
        get
        {
            bool a = false;
            foreach (var Body in _DetectionFarZone.GetOverlappingBodies())
            {
                if (Body is PlayerBase)
                {
                    a = true;
                }
            }
            foreach (var Body in _DetectionFarZone2.GetOverlappingBodies())
            {
                if (Body is PlayerBase)
                {
                    a = true;
                }
            }

            return !_smallAttack && a;
        }
    }

    private bool _flip
    {
        get
        {
            foreach (var Body in DetectionArea.GetOverlappingBodies())
            {

                if (Body is PlayerBase)
                {
                    return true;
                }
            }
            foreach (var Body in _DetectionFarZone.GetOverlappingBodies())
            {

                if (Body is PlayerBase)
                {

                    return true;
                }
            }

            return false;
        }
    }

    private bool _long = false;
    private bool _near = false;
    private bool _first = false;
    private bool _second = false;
    private bool _third = false;
    private bool _fliiip = false;
    private bool _isInvocing = false;
    private bool _canInvoc 
    {
        get
        {
            return !(Mushroom1.CurrentHp > 0 ||
                     Mushroom2.CurrentHp > 0 || 
                     Goblin.CurrentHp > 0 || 
                     Bat1.CurrentHp > 0 || 
                     Bat2.CurrentHp > 0 || 
                     Bat3.CurrentHp > 0 || 
                     Bat4.CurrentHp > 0 || 
                     Bat5.CurrentHp > 0 ||
                     Skeleton.CurrentHp > 0);
        } 
    }

    private Laser _weapon;
    private Laser _weapon2;
    private Laser _weapon3;
    [Export] private Control _credits;

    private AnimationPlayer _anim;

    private float _waitTime
    {
        get
        {
            if (_smallAttack)
            {
                return 2f;
            }
            
            if (_longAttack)
            {
                return 4f;
            }
            return 0.01f;
        }
    }

    public override bool AttackCondition()
    {
        return !_isInvocing && _long && AttackTimer.TimeLeft > 1f;
    }

    public override void _Ready()
    {
        base._Ready();
        _DetectionZone2 = GetNode<Area2D>("DetectionArea2");
        _DetectionFarZone = GetNode<Area2D>("DetectionAreaFarLeft");
        _DetectionFarZone2 = GetNode<Area2D>("DetectionAreaFarRight");
        _weapon = GetNode<Laser>("Weapon");
        _weapon2 = GetNode<Laser>("Weapon2");
        _weapon3 = GetNode<Laser>("Weapon3");
        _anim = GetNode<AnimationPlayer>("AnimationPlayer");
        Mushroom1 = GetNode<EnemyBase>("Enemies/Mushroom");
        Mushroom2 = GetNode<EnemyBase>("Enemies/Mushroom2");
        Goblin = GetNode<EnemyBase>("Enemies/Goblin");
        Bat1 = GetNode<EnemyBase>("Enemies/Bat/Bat");
        Bat2 = GetNode<EnemyBase>("Enemies/Bat/Bat2");
        Bat3 = GetNode<EnemyBase>("Enemies/Bat/Bat3");
        Bat4 = GetNode<EnemyBase>("Enemies/Bat/Bat4");
        Bat5 = GetNode<EnemyBase>("Enemies/Bat/Bat5");
        Skeleton = GetNode<EnemyBase>("Enemies/Skeleton");
        Mushroom1.Disapear();
        Mushroom2.Disapear();
        Goblin.Disapear();
        Bat1.Disapear();
        Bat2.Disapear();
        Bat3.Disapear();
        Bat4.Disapear();
        Bat5.Disapear();
        Skeleton.Disapear();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (AttackTimer.IsStopped())
        {
            AttackTimer.WaitTime = _waitTime;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if(CurrentHp > 0)
        {
            if (AttackTimer.IsStopped())
            {
                Int64 aaa = _random.NextInt64(0, 2);
                _anim.Stop();
                if (_longAttack && (aaa != 1 || !_canInvoc))
                {
                    AttackTimer.WaitTime = _waitTime;
                    AttackTimer.Start();
                    _long = true;
                    _first = false;
                    _second = false;
                    _third = false;
                    _near = false;
                    _isInvocing = false;
                    PlayerSprite.FlipH = _flip;
                    _fliiip = PlayerSprite.FlipH;
                }
                else if (_longAttack)
                {
                    AttackTimer.WaitTime = 4f;
                    AttackTimer.Start();
                    _isInvocing = true;
                    _near = false;
                    _long = false;
                }
                else if (_smallAttack)
                {
                    AttackTimer.WaitTime = _waitTime;
                    AttackTimer.Start();
                    _near = true;
                    _long = false;
                    _isInvocing = false;

                    _fliiip = _flip;
                    PlayerSprite.FlipH = _flip;
                    if (!_fliiip)
                    {
                        _anim.Play("attack");
                    }
                    else
                    {
                        _anim.Play("attack_flip");
                    }
                }
                else
                {
                    _long = false;
                    _near = false;
                    _isInvocing = false;
                    PlayerSprite.FlipH = _flip;
                }

                UpdateAnimation();
            }
            else
            {
                if (_long)
                {
                    LaserAttack();
                }
                else if (_isInvocing && AttackTimer.WaitTime - AttackTimer.TimeLeft >= 1 && _canInvoc)
                {
                    Invocation();
                }
                else if (_near)
                {
                    Coup();
                }
            }
        }
        else
        {
            Hud.Instance.End = true;
        }
        
    }

    public void LaserAttack()
    {
        if (!_first && AttackTimer.WaitTime - AttackTimer.TimeLeft >= 1 &&
            AttackTimer.WaitTime - AttackTimer.TimeLeft <= 1.05)
        {
            _weapon.Start(_fliiip,1);
            _first = true;
        }
        else if (!_second && AttackTimer.WaitTime - AttackTimer.TimeLeft >= 2 &&
                 AttackTimer.WaitTime - AttackTimer.TimeLeft <= 2.05)
        {
            _weapon2.Start(_fliiip,2);
            _second = true;
        }
        else if (!_third && AttackTimer.WaitTime - AttackTimer.TimeLeft >= 3 &&
                 AttackTimer.WaitTime - AttackTimer.TimeLeft <= 3.05)
        {
            _weapon3.Start(_fliiip,3);
            _third = true;
        }
        UpdateAnimation();
    }

    public void Invocation()
    {
        if(!_fliiip)
        {
            Int64 rand = _random.NextInt64(0, 3);
            if (rand == 0)
            {
                GD.Print("Mushroom");
                Mushroom1.Apear(new Vector2(-2463, -3241));
                Mushroom2.Apear(new Vector2(-2679, -3238));
            }
            else if (rand == 1)
            {
                GD.Print("Bat");
                Bat1.Apear(new Vector2(-2463, -3206));
                Bat2.Apear(new Vector2(-2377, -3237));
                Bat3.Apear(new Vector2(-2574, -3189));
                Bat4.Apear(new Vector2(-2693, -3230));
                Bat5.Apear(new Vector2(-2566, -3260));
            }
            else
            {
                GD.Print("Goblin");
                Goblin.Apear(new Vector2(-3331, -3246));
            }
        }
        else
        {
            Int64 rand = _random.NextInt64(0, 3);
            if (rand == 0)
            {
                GD.Print("Skeleton");
                Skeleton.Apear(new Vector2(-4269, -3181));
            }
            else
            {
                GD.Print("Mushroom");
                Mushroom1.Apear(new Vector2(-4859, -3241));
                Mushroom2.Apear(new Vector2(-4624, -3238));
            }
        }
        UpdateAnimation();

    }

    public void Coup()
    {
        var velocity = Velocity;
        if(AttackTimer.TimeLeft <= 0.75)
        {
            if (_fliiip)
            {
                velocity.Y = -100;
                velocity.X = 100;
            }
            else
            {
                velocity.Y = -100;
                velocity.X = -100;
            }
        }
        else
        {
            if (_fliiip)
            {
                velocity.Y = 100;
                velocity.X = -100;
            }
            else
            {
                velocity.Y = 100;
                velocity.X = 100;
            }
        }

        Velocity = velocity;
        MoveAndSlide();
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
        else if (_isInvocing && _canInvoc)
        {
            newAnimation = AnimationType.Invocation;
        }
        else if (AttackCondition())
        {
            newAnimation = AnimationType.Attack;
        }
        else
        {
            newAnimation = AnimationType.Idle;
        }

        if (newAnimation != CurrentAnimation)
        {
            PlayerSprite.Stop();
            CurrentAnimation = newAnimation;
            PlayerSprite.Play(CurrentAnimation.ToString().ToLower());
        }
    }
}