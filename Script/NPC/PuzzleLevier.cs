using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Godot;

namespace Beyondourborders.Script.NPC;

public partial class PuzzleLevier : NpcBase //TODO ahahah ca marche pas en multi (logique mais faut synchro)
{
    [Export] public bool Big = false;
    [Export] private PuzzleLevier OtherLevier;
    
    protected float StartTime = 0;
    protected bool HaveStarted = false;

    public bool tmpActivate = false;
    public bool Activate = false;

    private ColorRect color;

    public override void _Ready()
    {
        base._Ready();
        color = GetNode<ColorRect>("ColorRect");
    }

    private bool IsRightPlayer
    {
        get
        {
            return (Big && Client.Instance.CurrentPlayer is BigBob) ||
                   (!Big && Client.Instance.CurrentPlayer is SmallBob);
        }
    }
    public override void _Process(double delta)
    {
        if (Activate)
        {
            color.Color = new Color(0, 255, 0);
        }
        else if (tmpActivate)
        {
            color.Color = new Color(255,0,0);
        }
        else
        {
            color.Color = new Color(0,0,0);
        }
        if (HaveStarted && Time.GetTicksMsec() - StartTime <= 5000)
        {
            if (OtherLevier.tmpActivate)
            {
                Activate = true;
                OtherLevier.Activate = true;
            }
        }
        else
        {
            HaveStarted = false;
            tmpActivate = false;
        }
        base._Process(delta);
        if (CurrentPlayerInZone && IsRightPlayer && Input.IsActionJustPressed("Interact"))
        {
            if (Client.Instance.IsMultiplayer)
            {
                if (OtherLevier.tmpActivate)
                {
                    tmpActivate = true;
                }
                else
                {
                    tmpActivate = true;
                    StartTime = Time.GetTicksMsec();
                    HaveStarted = true;
                }
            }
            else
            {
                Activate = true;
            }
        }
    }
}