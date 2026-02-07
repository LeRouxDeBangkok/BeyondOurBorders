using Beyondourborders.Script.Triggers;
using Godot;

namespace Beyondourborders.Script.Weapons;

public abstract partial class WeaponBase : CharacterBody2D
{
    public int Damage = 5;
    public AnimatedSprite2D Sprite;
    protected CollisionShape2D CollisionShape;
    public HitArea HitArea;

    public bool Visibility = false;
    //Animation

    public WeaponBase() { }
    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        HitArea = GetNode<HitArea>("HitZone");
    }
    
    public abstract void Start();

    public override void _Process(double delta)
    {
        if (Visibility)
        {
            CollisionShape.Disabled = false;
            HitArea.Active = true;
        }
        else
        {
            CollisionShape.Disabled = true;
            HitArea.Active = false;
        }
    }
}
