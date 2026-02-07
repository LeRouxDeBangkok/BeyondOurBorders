using Godot;
using Beyondourborders.Script.ClientData;

namespace Beyondourborders.Script.Menu.Components;

public partial class MainPage : Node2D, IMenuPage {
	public (int, int) GetButtonOffsetFromPreviousPage() {
		return (0, 0);
	}

	public (int, int) GetBackgroundOffsetFromPreviousPage() {
		return (0, 0);
	}
	
	public void OnSettingsPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		MainMenuController.Instance.ShowPage(MainMenuController.Instance.SettingsPage);
	}

	public void OnExitPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		MainMenuController.Instance.ShowPage(MainMenuController.Instance.QuitConfirmation);
	}

	public void OnPlayPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		MainMenuController.Instance.ShowPage(MainMenuController.Instance.SaveSelectionPage);
	}
	
}
