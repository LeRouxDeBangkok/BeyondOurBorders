using Beyondourborders.Script.ClientData;
using Godot;

namespace Beyondourborders.Script.Menu.Components;

public partial class JoinHostIpPortSelectionPage : Node2D, IMenuPage {
    public (int, int) GetButtonOffsetFromPreviousPage() {
        return (1280, 0);
    }

    public (int, int) GetBackgroundOffsetFromPreviousPage() {
        return (300, 0);
    }

    private TextEdit _ipTextEdit;
    private TextEdit _portTextEdit;

    public override void _Ready() {
        base._Ready();
        _ipTextEdit = GetNode("IpButton").GetNode<TextEdit>("TextEdit");
        _portTextEdit = GetNode("PortButton").GetNode<TextEdit>("TextEdit");
        _ipTextEdit.Text = "127.0.0.1";
        _portTextEdit.Text = "55405";
    }

    public void OnJoinThisServerClick() {
        var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
        myGDScript.Call("_onClickButtonEvent");
        
        MultiplayerManager.Instance.JoinGame(
            _ipTextEdit.Text,
            int.Parse(_portTextEdit.Text)
            );
    }
}