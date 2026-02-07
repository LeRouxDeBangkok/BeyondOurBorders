using System.ComponentModel;
using Beyondourborders.Script.Camera;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Overlay.Components;

public partial class DebugComponent : Panel
{
    private Label _label;
    
    public override void _Ready()
    {
        _label = GetNode<Label>("DebugText");
    }

    public override void _Process(double delta) {
        var currentPlayer = Client.Instance.CurrentPlayer;
        if (currentPlayer is not null) {
            //Show();
            var currentPlayerType = StringUtils.GetSimpleType(currentPlayer);
            var currentCamera = currentPlayer.GetNode<BetterCamera>("Camera2D");
            var otherPlayer = Client.Instance.OtherPlayer;
            var otherPlayerType = StringUtils.GetSimpleType(otherPlayer);
            // ofs = offset (camera)
            _label.Text = $@"FPS: {Engine.GetFramesPerSecond()}
FRAMES: {Engine.GetFramesDrawn()}

Current ({currentPlayerType}):
x:{currentPlayer.GlobalPosition.X:0.00} y:{currentPlayer.GlobalPosition.Y:0.00}
zoom:({currentCamera.GetZoom().X:0.00}, {currentCamera.GetZoom().Y:0.00})
ofs:({currentCamera.GetOffset().X:0.00}, {currentCamera.GetOffset().Y:0.00})

Other ({otherPlayerType}):
x:{otherPlayer.GlobalPosition.X:0.00} y:{otherPlayer.GlobalPosition.Y:0.00}
";
        }
        else {
            Hide();
        }
    }
}