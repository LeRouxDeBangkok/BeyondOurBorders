using System;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Entities.Players.Properties;
using Beyondourborders.Script.Weapons;
using WeaponBase = Beyondourborders.Script.Weapons.WeaponBase;
using Beyondourborders.Script.Triggers;
using Godot;
using Beyondourborders.Script.World;

namespace Beyondourborders.Script.Entities.Enemies;

public abstract partial class EnemyBase : EntityBase
{
    protected Area2D DetectionArea;
    protected EnemyGhettoSynchronizer MultiplayerSynchronizer;
    public bool SmallBobOnDetectionArea = false;
    public bool BigBobOnDetectionArea = false;
    protected WeaponBase? Weapon;
    protected HitArea HitZone;

    [Export] public global::Item DroppableItem;
    [Export] public PackedScene ItemPickupScene;
    [Export] public int DropChancePercent;

    private bool _isDead = false;

    public EnemyBase(float speed, float jumpVelocity, int hp)
        : base(speed: speed, jumpVelocity: jumpVelocity, hp: hp)
    {
    }

    public EnemyBase()
    {
        throw new Exception(
            "This constructor only exists to avoid warnings. Please call the PlayerBase constructor with arguments.");
    }
    public override void _Ready()
    {
        base._Ready();
        DetectionArea = GetNode<Area2D>("DetectionArea");
        MultiplayerSynchronizer = GetNode<EnemyGhettoSynchronizer>("MultiplayerSynchronizer");
        DetectionArea.BodyEntered += _on_body_entered;
        DetectionArea.BodyExited += _on_body_exited;
        Weapon = null;
        HitZone = GetNode<HitArea>("HitZone");
        Client.Instance.AllMobs.Add(this);
    }

    private void _on_body_entered(Node2D body)
    {
        if (body is BigBob)
        {
            BigBobOnDetectionArea = true;
        }

        if (body is SmallBob)
        {
            SmallBobOnDetectionArea = true;
        }
    }

    private void _on_body_exited(Node2D body)
    {
        if (body is BigBob)
        {
            BigBobOnDetectionArea = false;
        }

        if (body is SmallBob)
        {
            SmallBobOnDetectionArea = false;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!_isDead && CurrentHp <= 0)
        {
            Kill();
        }

        if (CurrentHp > 0)
        {
            _isDead = false;
        }
    }

    protected virtual void Kill()
    {
        _isDead = true;
        PlayerSprite.Hide();
        HitZone.Active = false;
        CollisionShape.Disabled = true;

        DropItemIfApplicable();
        Disapear();
    }

    protected void DropItemIfApplicable()
    {
        if (ItemPickupScene == null || DroppableItem == null)
            return;

        GD.Randomize();

        if (GD.Randi() % 100 < DropChancePercent)
        {
            GD.Print($"Dropping item: {DroppableItem.Name}");
            var drop = (World.ItemPickup)ItemPickupScene.Instantiate();
            drop.ItemData = DroppableItem;
            drop.GlobalPosition = GlobalPosition;

            GetTree().CurrentScene.AddChild(drop);
        }
        else
        {
            GD.Print("No item dropped.");
        }
    }

    public virtual void Apear(Vector2 pos)
    {
        PlayerSprite.Show();
        HitZone.Active = true;
        CollisionShape.Disabled = false;
        Position = pos;
        CurrentHp = HPMax;
    }

    public void Disapear()
    {
        PlayerSprite.Hide();
        HitZone.Active = false;
        CollisionShape.Disabled = true;
        CurrentHp = 0;
    }

    private void DoMultiplayerSyncStuff() 
    {
        // TODO
    }
}
