using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Menu;
using Godot;

namespace Beyondourborders.Script.Overlay.Components.PauseMenuComponents;

public partial class PauseMenuQuitConfirmation : Node2D, IMenuPage {
	public (int, int) GetButtonOffsetFromPreviousPage() {
		return (0, 720);
	}
	
	// Unused in the pause menu.
	public (int, int) GetBackgroundOffsetFromPreviousPage() {
		return (0, 0);
	}

	public void OnNoPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		Hud.Instance.PauseMenu.OnEscapePressed();
	}
	
	public void OnYesPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		GhettoMobSynchronizerManager.Instance.OnGuestLeaveMultiplayer();
		
		if (Client.Instance.IsHost) {
			Hud.Instance.PauseMenu.CloseMenuSafe();
			if (MultiplayerManager.Instance.IsBroadcasting) {
				MultiplayerManager.Instance.StopLanBroadcasting();
			}
		} else {
			Hud.Instance.PauseMenu.CloseMenuSafe();
			MultiplayerManager.Instance.DisconnectFromHost();
		}
		myGDScript.Call("_exitPauseMenuEvent");
		Client.Instance.LeaveGameToMenu();
	}
}
