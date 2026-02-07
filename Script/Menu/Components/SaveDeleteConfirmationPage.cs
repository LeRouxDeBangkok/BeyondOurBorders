using Beyondourborders.Script.ClientData;
using Godot;

namespace Beyondourborders.Script.Menu.Components;

public partial class SaveDeleteConfirmationPage : Node2D, IMenuPage {
	public (int, int) GetButtonOffsetFromPreviousPage() {
		return (1280, 0);
	}

	public (int, int) GetBackgroundOffsetFromPreviousPage() {
		return (300, 0);
	}

	
	public void OnNoPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		MainMenuController.Instance.OnEscapePressed();
	}
	
	public void OnYesPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		var selected = MainMenuController.Instance.SaveSelectionPage.CurrentlySelectedSave;
		SaveHelper.Instance.RemoveSave(selected);
		MainMenuController.Instance.SaveSelectionPage.RefreshPage(selected);
		MainMenuController.Instance.OnEscapePressed();
	}}
