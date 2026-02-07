using Beyondourborders.Script.ClientData;
using Godot;

namespace Beyondourborders.Script.Menu.Components;

public partial class NewSaveConfirmationPage : Node2D, IMenuPage {
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
		var myGdScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGdScript.Call("_onClickButtonEvent");
		GameData.SetCurrentGameData(new GameData(
			saveNum: MainMenuController.Instance.SaveSelectionPage.CurrentlySelectedSave,
			playTime: 0,
			lastCheckpoint: CheckpointList.DefaultCheckpoint.IdName,
			isLevel2DoorOpen: false,
			weapon: 0,
			abilities: "",
			isLevel4DoorOpen: false
		));
		Client.Instance.LoadGameFromGameDataHost();
	}}
