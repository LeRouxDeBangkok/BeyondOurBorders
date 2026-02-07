using System;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Overlay;
using Godot;

namespace Beyondourborders.Script.NPC;

public partial class GhettoCourse : Node
{
    private Area2D _start;
    private Area2D _end;
    private Timer _timer;
    private CharacterBody2D _door;
    private CollisionShape2D _col;

    private bool _playerOnStart
    {
        get
        {
            foreach (var body in _start.GetOverlappingBodies())
            {
                if (body is PlayerBase b)
                {
                    return true;
                }
            }

            return false;
        }
    }
    private bool _playerOnEnd{
        get
        {
            foreach (var body in _end.GetOverlappingBodies())
            {
                if (body is PlayerBase b)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private bool _havestarted;
   
    public override void _Ready()
    {
        base._Ready();
        _start = GetNode<Area2D>("Start");
        _end = GetNode<Area2D>("End");
        _timer = GetNode<Timer>("Timer");
        _door = GetNode<CharacterBody2D>("Door");
        _col = GetNode<CollisionShape2D>("Door/CollisionShape2D");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(!GameData.CurrentGameData.IsLevel3CourseComplete)
        {
            _door.Show();
            _col.Disabled = false;
            if (!_havestarted && _playerOnStart)
            {
                _timer.Start();
                _havestarted = true;
            }
            else if (_havestarted)
            {
                if (_playerOnEnd)
                {
                    GameData.CurrentGameData.IsLevel3CourseComplete = true;
                    Hud.Instance.OnInventoryChatUpdated("Good job !");
                }
                else if (_timer.IsStopped())
                {
                    _havestarted = false;
                    Hud.Instance.OnInventoryChatUpdated("Too slow... use your new ability");
                }
                else
                {
                    Hud.Instance.OnInventoryChatUpdated(Math.Round(_timer.TimeLeft).ToString());
                }
            }
        }
        else
        {
            _door.Hide();
            _col.Disabled = true;
        }
    }
}