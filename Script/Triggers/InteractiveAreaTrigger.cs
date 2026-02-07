using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Triggers;

public partial class InteractiveAreaTrigger : SimpleResizableArea {
    [Export] public string Text { get; set; } = "Interact";
    // This class is meant to like show a little popup
    // Eg: https://www.shutterstock.com/image-vector/pixel-speech-bubble-cloud-text-260nw-2165430969.jpg
    // without the little thing bottom right, if possible appearing
    // with a little animation sliding up (& opposite when disappearing)
    // It's also made so that when you use the interact key when inside it, it does 
    // a specific action, eg change zone when you tap it while in front of a door.
    // TODO:
    // - add the callback once called here.
    // - implement the check if key pressed in the player class
    // - implement the popup display
    // NOTE:
    // could be made abstract and then have subclasses implement diff behaviors.
    // Eg "InteractiveTalkTrigger", "InteractiveZoneChangeTrigger"
    // Would have the advantage to be ez to implement in the players (use this type in the player class)
    // and not have to worry about setting functions from the godot editor which is weird.
    
    // public event Action ButtonUp
    // {
    //     add => Connect(SignalName.ButtonUp, Callable.From(value));
    //     remove => Disconnect(SignalName.ButtonUp, Callable.From(value));
    // }
    
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