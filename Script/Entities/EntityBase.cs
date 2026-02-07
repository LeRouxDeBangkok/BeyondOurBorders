using System;
using System.Threading.Tasks;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players.Properties;
using Beyondourborders.Script.Weapons;
using Godot;

namespace Beyondourborders.Script.Entities;

public abstract partial class EntityBase : CharacterBody2D
{
    
    [Export] protected float HitDelay = 0.5f;
    protected Timer HitDelayTimer; //Can take damage when the timer is stopped

    public AnimatedSprite2D PlayerSprite;
    protected CollisionShape2D CollisionShape;

    public Timer AttackTimer;
    
    public float Speed;
    public float JumpVelocity;

    public string RawZone;

    private float _startTime = 0;
    public bool TakingDamage = false;
    
    [Export] public virtual string Zone {
        get => RawZone;
        set => RawZone = value;
    }

    protected AnimationType? CurrentAnimation;

    // TODO: remake private if not used anymore in AreaSwitchTrigger.
    // This is a different offset than the normal Godot one, here to handle actual player offset
    // Different from the additional offset used on some transitions.
    // Tbh this is just an implementation detail, may get removed in favor of CameraObject's offset.
    public int HPMax;
    [Export] public int CurrentHp;
    public EntityBase(float speed, float jumpVelocity, int hp = 100) {
        this.Speed = speed;
        this.JumpVelocity = jumpVelocity;
        this.CurrentAnimation = AnimationType.Idle;

        HPMax = hp;
        CurrentHp = HPMax;
    }

    public virtual void TakeDamage(int damage)
    {
        if (HitDelayTimer.IsStopped())
        {
            GD.Print("take damage : " + damage);
            CurrentHp -= damage;
            TakingDamage = true;
            _startTime = Time.GetTicksMsec();
            HitDelayTimer.Start();
        }
    }

    public EntityBase()
    {
        throw new Exception(
            "This constructor only exists to avoid warnings. Please call the PlayerBase constructor with arguments.");
    }
    public override void _Ready() 
    {
        HitDelayTimer = GetNode<Timer>("HitDelay"); 
        HitDelayTimer.OneShot = true;
        HitDelayTimer.Autostart = false;
        HitDelayTimer.WaitTime = HitDelay;
        HitDelayTimer.Stop();
        
        this.PlayerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        this.CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        AttackTimer = GetNode<Timer>("AttackTimer");
        AttackTimer.Stop();
    }

    public override void _Process(double delta)
    {
        if (TakingDamage && _startTime - Time.GetTicksMsec() <= 1000)
        {
            TakingDamage = false;
        }
    }

    public virtual void UpdateAnimation() 
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
    public abstract bool AttackCondition();
}