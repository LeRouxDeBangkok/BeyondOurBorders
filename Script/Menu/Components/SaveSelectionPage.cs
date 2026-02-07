using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Menu.Components;

public partial class SaveSelectionPage : Node2D, IMenuPage {
	public (int, int) GetButtonOffsetFromPreviousPage() {
		return (1280, 0);
	}

	public (int, int) GetBackgroundOffsetFromPreviousPage() {
		return (300, 0);
	}

	private bool[] _alreadyCreated = null!;
	
	public int CurrentlySelectedSave { get; private set; }
	
	public void RefreshPage(int saveN) {
		var i = saveN - 1;
		
		var root = GetNode<Button>($"Save{saveN}");
		var clearButton = root.GetNode<Button>("Clear");
		var label = root.GetNode<Label>("Label");
		var existingSaveIcon = root.GetNode<TextureRect>("ExistingSaveIcon");
		var emptySaveIcon = root.GetNode<TextureRect>("EmptySaveIcon");

		root.Pressed += () => OnSaveClick(saveN);
		clearButton.Pressed += () => OnClearClick(saveN);
		
		var saveData = SaveHelper.Instance.GetSaveData(saveN);
		
		if (saveData is null) {
			_alreadyCreated[i] = false;
			clearButton.Hide();
			
			label.HorizontalAlignment = HorizontalAlignment.Center;
			label.Text = "~ No Data ~";
			
			existingSaveIcon.Hide();
			emptySaveIcon.Show();
		} else {
			_alreadyCreated[i] = true;
			var checkpointObj = CheckpointList.GetCheckpoint(saveData.LastCheckpoint);
			var checkpointSaveName = checkpointObj == null ? "Unknown" : checkpointObj.SaveSelectorName;
			label.Text =
				$"Save {saveN}- {checkpointSaveName}\n{StringUtils.SsToHhMm((int)saveData.PlayTime)} playtime";
			
			existingSaveIcon.Show();
			emptySaveIcon.Hide();
		
		}
	}
	public override void _Ready() {
		_alreadyCreated = new bool[5];

		for (int i = 0; i < 5; i++) {
			var saveN = i + 1;

			var root = GetNode<Button>($"Save{saveN}");
			var clearButton = root.GetNode<Button>("Clear");
			var label = root.GetNode<Label>("Label");
			var existingSaveIcon = root.GetNode<TextureRect>("ExistingSaveIcon");
			var emptySaveIcon = root.GetNode<TextureRect>("EmptySaveIcon");

			root.Pressed += () => OnSaveClick(saveN);
			clearButton.Pressed += () => OnClearClick(saveN);

			var saveData = SaveHelper.Instance.GetSaveData(saveN);

			if (saveData is null) {
				_alreadyCreated[i] = false;
				clearButton.Hide();

				label.HorizontalAlignment = HorizontalAlignment.Center;
				label.Text = "~ No Data ~";

				existingSaveIcon.Hide();
				emptySaveIcon.Show();
			}
			else {
				_alreadyCreated[i] = true;
				var checkpointObj = CheckpointList.GetCheckpoint(saveData.LastCheckpoint);
				var checkpointSaveName = checkpointObj == null ? "Unknown" : checkpointObj.SaveSelectorName;
				label.Text =
					$"Save {saveN}- {checkpointSaveName}\n{StringUtils.SsToHhMm((int)saveData.PlayTime)} playtime";

				existingSaveIcon.Show();
				emptySaveIcon.Hide();
			}
		}
	}

	public void OnSaveClick(int saveNum) {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		// Actually not sure if I should show the confirmation page every time or not.
		GD.Print("Loading save !");
		if (_alreadyCreated[saveNum - 1]) {
			// Shouldn't be null unless files were tampered with in the meantime
			GameData.SetCurrentGameData(SaveHelper.Instance.GetSaveData(saveNum)!); 
			Client.Instance.LoadGameFromGameDataHost();
		} else {
			CurrentlySelectedSave = saveNum;
			MainMenuController.Instance.ShowPage(MainMenuController.Instance.NewSaveConfirmationPage);
		}
	}

	public void OnClearClick(int saveNum) {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		CurrentlySelectedSave = saveNum;
		MainMenuController.Instance.ShowPage(MainMenuController.Instance.SaveDeleteConfirmationPage);
	}

	public void OnJoinHostedClick() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		MainMenuController.Instance.ShowPage(MainMenuController.Instance.JoinHostIpPortSelectionPage);
	}
}
