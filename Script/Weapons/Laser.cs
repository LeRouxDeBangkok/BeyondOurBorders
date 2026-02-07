using System;
using Godot;

namespace Beyondourborders.Script.Weapons;

public partial class Laser : WeaponBase
{
    private Timer _timer;
    public override void Start()
    {
        Start(false,1);
    }

    public override void _Ready()
    {
        base._Ready();
        _timer = GetNode<Timer>("Timer");
    }

    public void Start(bool flip, int i)
    {
        Visibility = true;
        Sprite.Stop();
        Sprite.Play("default");
        Sprite.FlipH = flip; 
        if (!flip)
        {
            if (i == 1)
            {
                Rotation = 0;
                Position = new Vector2(368,-48);
            }
            else if (i == 2)
            {
                RotationDegrees = 8;
                Position = new Vector2(365,0);
            }
            else
            {
                RotationDegrees = 19;
                Position = new Vector2(351,64);
            }
        }
        else
        {
            if (i == 1)
            {
                RotationDegrees = 0;
                Position = new Vector2(-422,-39);
            }
            else if (i == 2)
            {
                RotationDegrees = -8.8f;
                Position = new Vector2(-417,17 );
            }
            else
            {
                RotationDegrees = -19f;
                Position = new Vector2(-400,91);
            }
        }
        Show();
        _timer.Start();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_timer.IsStopped())
        {
            Hide();
            Visibility = false;
        }
    }
}