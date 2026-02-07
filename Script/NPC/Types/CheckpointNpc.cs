using Godot;

namespace Beyondourborders.Script.NPC.Types;

public partial class CheckpointNpc : NpcBase {
    [Export] public string CheckpointName { get; set; }
    
    public new void OnPlayerEnter(Node2D body) {
        base.OnPlayerEnter(body);
        Sprite.Play("FlagBoop");
    }
    
    public new void OnPlayerLeave(Node2D body) {
        base.OnPlayerLeave(body);
        Sprite.Pause();
    }
}