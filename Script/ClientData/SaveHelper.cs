using System;
using Beyondourborders.Script.Overlay;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.ClientData;

public partial class SaveHelper : Node {
	public static SaveHelper Instance { get; private set; } = null!;

	public SaveHelper() {
		Instance = this;
	}
	
	// TO BE CALLED ON THE HOST ONLY.
	public void Save() {
		Rpc("ShowSaveScreenCommon");
		TriggerSaveAndEnd();
	}
	
	// will be called using RPC First when you want to save.
	[Rpc(CallLocal = true)]
	private void ShowSaveScreenCommon() {
		Client.Instance.IsSaving = true;
		Hud.Instance.TransitionScreen.ShowScreen(Client.Instance.IsGuest ? "Host is saving..." : "Saving...");
	}
	
	// Actual save, will be called on the host only.
	private void TriggerSaveAndEnd() {
		var startTime = Time.Singleton.GetUnixTimeFromSystem();
		
		SaveCurrentGameDataToDiskSkipAllChecks();
		
		WaitUtils.WaitUntil(
			() => startTime + 1.5 > Time.Singleton.GetUnixTimeFromSystem(),
			() => CallDeferred("OnDoneWaiting")
		);
	}
	private void OnDoneWaiting() {
		Rpc("HideSaveScreenCommon");
	}

	// End, called by the actual save on the host.
	[Rpc(CallLocal = true)]
	private void HideSaveScreenCommon() {
		GD.Print("Done !");
		Hud.Instance.TransitionScreen.RemoveScreen();
		Client.Instance.IsSaving = false;
	}
	
	// Actual saving/loading functions below.
	// -> https://docs.godotengine.org/en/stable/tutorials/io/saving_games.html
	public void SaveCurrentGameDataToDiskSkipAllChecks() {
		var gameData = GameData.CurrentGameData;
		if (gameData is null) {
			GD.PushError("Attempting to save null gamedata.");
			return;
		}
		
		using var saveFile = FileAccess.Open($"user://save_{gameData.SaveNum}.json", FileAccess.ModeFlags.Write);
		
		gameData.UpdatePlaytime();
		
		var saveDict = new Godot.Collections.Dictionary<string, Variant>()
		{
			{ "saveNum",  gameData.SaveNum}, // Counts double but still.
			{ "playTime", gameData.PlayTime },
			{ "lastCheckpoint", gameData.LastCheckpoint },
			{ "isLevel2DoorOpen", gameData.IsLevel2DoorOpen },
			{ "weapon", gameData.Weapon },
			{ "abilities", gameData.Abilities },
			{ "isLevel4DoorOpen", gameData.IsLevel4DoorOpen }
		};
		
		saveFile.StoreLine(Json.Stringify(saveDict));
	}

	public GameData? GetSaveData(int saveNum) {
		using var saveFile = FileAccess.Open($"user://save_{saveNum}.json", FileAccess.ModeFlags.Read);
		if (saveFile is null)
			return null;
		
		var jsonString = saveFile.GetLine();

		// Creates the helper class to interact with JSON.
		var json = new Json();
		var parseResult = json.Parse(jsonString);
		if (parseResult != Error.Ok)
		{
			GD.PushError($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
			return null;
		}
		
		var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);

		try {
			var content = new GameData(
				saveNum: (int)nodeData["saveNum"], // Could also just use saveNum.
				playTime: (double)nodeData["playTime"],
				lastCheckpoint: (string)nodeData["lastCheckpoint"],
				isLevel2DoorOpen: (bool)nodeData["isLevel2DoorOpen"],
				weapon: (int)nodeData["weapon"],
				abilities: (string)nodeData["abilities"],
				isLevel4DoorOpen: (bool)nodeData["isLevel4DoorOpen"]
			);
			return content;
		} catch (Exception e) {
			GD.PushError($"Failed to build GameData object: {e}");
			return null;
		}
	}

	public void RemoveSave(int saveNum) {
		using DirAccess dir = DirAccess.Open($"user://");
		if (dir != null && dir.FileExists($"save_{saveNum}.json"))
			dir.Remove($"save_{saveNum}.json");
		else
			GD.PushWarning($"Couldn't delete nonexistent file: save_{saveNum}.json");
	}
}
