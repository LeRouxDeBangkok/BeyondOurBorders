using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Menu;
using Godot;

namespace Beyondourborders.Script.Overlay.Components.PauseMenuComponents;

public partial class PauseMenuMainPage : Node2D, IMenuPage {
	
	public (int, int) GetButtonOffsetFromPreviousPage() {
		return (0, 0);
	}
	
	// Unused in the pause menu.
	public (int, int) GetBackgroundOffsetFromPreviousPage() {
		return (0, 0);
	}
	
	public void OnResumePressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		myGDScript.Call("_closePauseMenuEvent");
		// resume is only on the last page,
		// so just using that lmao
		
		Hud.Instance.PauseMenu.OnEscapePressed();
	}

	public void OnSettingsPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		Hud.Instance.PauseMenu.ShowPage(Hud.Instance.PauseMenu.SettingsPage);
	}
	
	public void OnVolumeSliderValueChanged(float valueb)
	{
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_changeVolumeValueEvent", valueb);
	}
	
	public void OnExitPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		Hud.Instance.PauseMenu.ShowPage(Hud.Instance.PauseMenu.QuitConfirmation);
	}

	public void On2PPressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_onClickButtonEvent");
		if (MultiplayerManager.Instance.IsBroadcasting) {
			MultiplayerManager.Instance.StopLanBroadcasting();
			UpdateButtons();
			return;
		}
		
		// TODO: Another page w input for port and shit.
		MultiplayerManager.Instance.StartLanBroadcasting(MultiplayerManager.Instance.DefaultPort);
		UpdateButtons();
	}

	private Button _2PButton = null!;
	private Label _2PButtonLabel = null!;
	private Label _exitButtonLabel = null!;
	
	public override void _Ready() {
		_2PButton = GetNode<Button>("Lan");
		_2PButtonLabel = _2PButton.GetNode<Label>("Label");
		_exitButtonLabel = GetNode("Exit").GetNode<Label>("Label");
		UpdateButtons();
	}
	
	public void UpdateButtons() {
		if (Client.Instance.IsMultiplayer) {
			_exitButtonLabel.Text = Client.Instance.IsHost ? "End Sess." : "Disconnect";
		} else {
			_exitButtonLabel.Text = "Exit";
		}

		if (Client.Instance.IsHost) {
			_2PButton.SetDisabled(false);
			_2PButtonLabel.Text = MultiplayerManager.Instance.IsBroadcasting ? "Disable\n2P Mode" : "Enable\n2P Mode";
		} else {
			_2PButtonLabel.Text = "Multiplayer";
			_2PButton.SetDisabled(true);
		}
		
	}
}
