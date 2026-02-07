using Beyondourborders.Script.Menu;
using Beyondourborders.Script.ClientData;
using Godot;

namespace Beyondourborders.Script.Overlay.Components.PauseMenuComponents;

public partial class PauseMenuSettingsPage : Node2D, IMenuPage {
	public (int, int) GetButtonOffsetFromPreviousPage() {
		return (0, -720);
	}
	
	// Unused in the pause menu.
	public (int, int) GetBackgroundOffsetFromPreviousPage() {
		return (0, 0);
	}
	
	public void OnGoBackPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		Hud.Instance.PauseMenu.OnEscapePressed();
	}
}
