using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Triggers;

public partial class InteractiveAreaTrigger : SimpleResizableArea {
    [Export] public string Text { get; set; } = "Interact";
    
    public InteractiveAreaTrigger() : base("InteractiveAreaTrigger") {}
    
    public override void _Ready() {
        base._Ready();
        BodyEntered += Entered;
        BodyExited += Left;
    }
    
    public void Entered(Node2D body) {
        if (!IsCurrentActivePlayer(body))
            return;

        var player = (PlayerBase)body;
        player.CurrentInteractArea = this;
		
        GD.Print("Interacted Player " + body.Name + " with " + Name);
    }

    public void Left(Node2D body) {
        if (!IsCurrentActivePlayer(body))
            return;
		
        var player = (PlayerBase)body;
        player.CurrentInteractArea = null;
        
        GD.Print("Interacted Player " + body.Name + " with " + Name);	
    }
}