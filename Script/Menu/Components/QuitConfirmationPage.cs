using Beyondourborders.Script.Utils;
using Beyondourborders.Script.ClientData;
using Godot;

namespace Beyondourborders.Script.Menu.Components;

public partial class QuitConfirmationPage : Node2D, IMenuPage {
	public (int, int) GetButtonOffsetFromPreviousPage() {
		return (0, 720);
	}

	public (int, int) GetBackgroundOffsetFromPreviousPage() {
		return (0, 200);
	}
	
	public void OnNoPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		MainMenuController.Instance.OnEscapePressed();
	}
	
	public void OnYesPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		// Funny like that kinda.
		GetTree().Root.Mode = Window.ModeEnum.Minimized;
		var t = Time.GetUnixTimeFromSystem();
		WaitUtils.WaitUntil(
			() => Time.GetUnixTimeFromSystem() > t+1,
			Quit
			);
	}

	private void Quit() {
		GetTree().Quit();
	}
}
