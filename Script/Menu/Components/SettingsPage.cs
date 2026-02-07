using Godot;
using Beyondourborders.Script.ClientData;

namespace Beyondourborders.Script.Menu.Components;

public partial class SettingsPage : Node2D, IMenuPage {
	public (int, int) GetButtonOffsetFromPreviousPage() {
		return (0, -720);
	}

	public (int, int) GetBackgroundOffsetFromPreviousPage() {
		return (0, -200);
	}
	
	public void OnGoBackPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		MainMenuController.Instance.OnEscapePressed();
	}
	
	public void OnVolumeSliderValueChanged(float valueb)
	{
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_changeVolumeValueEvent", valueb);
	}
}
