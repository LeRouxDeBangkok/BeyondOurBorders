using Beyondourborders.Script.ClientData;
using Godot;

namespace Beyondourborders.Script.NPC;

public partial class PuzzleDoorLevel4 : NpcBase
{
    [Export] public PuzzleLevier Levier1;
    [Export] public PuzzleLevier Levier2;
    private CollisionShape2D _collision;
    
    private ColorRect color;
    private ColorRect color2;


    public override void _Ready()
    {
        base._Ready();
        _collision = GetNode<CollisionShape2D>("CollisionShape2D");
        color = GetNode<ColorRect>("ColorRect");
        color2 = GetNode<ColorRect>("ColorRect2");
        if (GameData.CurrentGameData.IsLevel4DoorOpen) {
            this.Hide();
            _collision.Disabled = true;
        }
    }

    public override void _Process(double delta)
    {
        if (!GameData.CurrentGameData.IsLevel4FirstLeverTriggered && Levier1.Activate) {
            GameData.CurrentGameData.IsLevel4FirstLeverTriggered = true;
        }
        if (!GameData.CurrentGameData.IsLevel4SecondLeverTriggered && Levier2.Activate) {
            GameData.CurrentGameData.IsLevel4SecondLeverTriggered = true;
        }
        if (!GameData.CurrentGameData.IsLevel4DoorOpen && !Visible) {
            GameData.CurrentGameData.IsLevel4DoorOpen = true;
        }
        if (Levier1.Activate)
        {
            color.Color = new Color(0, 255, 0);
        }
        else
        {
            color.Color = new Color(0, 0, 0);
        }
        if (Levier2.Activate)
        {
            color2.Color = new Color(0, 255, 0);
        }
        else
        {
            color2.Color = new Color(0, 0, 0);
        }
        base._Process(delta);
        if (Levier1.Activate && Levier2.Activate) //TODO maybe add des vibration sur la cam mais jsp comment faire
        {
            this.Hide();
            _collision.Disabled = true;
        }
    }
}