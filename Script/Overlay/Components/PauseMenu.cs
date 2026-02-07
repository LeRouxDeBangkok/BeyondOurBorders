using System.Collections.Generic;
using Beyondourborders.Script.Menu;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Overlay.Components.PauseMenuComponents;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Overlay.Components;

public partial class PauseMenu : ColorRect {
	public PauseMenuMainPage MainPage { get; private set; }
	public PauseMenuSettingsPage SettingsPage { get; private set; }
	public PauseMenuQuitConfirmation QuitConfirmation { get; private set; }

	private List<IMenuPage> _openedPages;
	
	private Tween _appearTween;
	private Tween _moveTweenCurrent;
	private Tween _moveTweenOther;
	public bool IsShown { get; private set; } = false;
	
	public override void _Ready() {
		MainPage = GetNode<PauseMenuMainPage>("MainPage");
		SettingsPage = GetNode<PauseMenuSettingsPage>("SettingsPage");
		QuitConfirmation = GetNode<PauseMenuQuitConfirmation>("QuitConfirm");
		
		_openedPages = new List<IMenuPage>();
	}

	public void OnEscapePressed() {
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		if (IsShown) {
			if (_openedPages.Count == 1) {
				CloseMenu();
				myGDScript.Call("_closePauseMenuEvent");
				return;
			}
			PreviousPage();
		}
		else {
			myGDScript.Call("_openPauseMenuEvent");
			OpenMenu();
		}
	}

	public void ShowPage<T>(T page) where T: Node2D, IMenuPage {
		
		var offset = page.GetButtonOffsetFromPreviousPage();
		page.GlobalPosition = new Vector2(offset.Item1, offset.Item2);

		_moveTweenCurrent = TweenUtils.RemakeTransitionTween(CreateTween, _moveTweenCurrent);
		_moveTweenOther = TweenUtils.RemakeTransitionTween(CreateTween, _moveTweenOther);
		
		page.Show();

		var previous = (Node2D)_openedPages[^1];
		
		_moveTweenCurrent.TweenProperty(
			previous, 
			"position", 
			new Vector2(-offset.Item1, -offset.Item2),
			0.15f);
		
		_moveTweenOther.TweenProperty(
			page, 
			"position", 
			new Vector2(0, 0),
			0.15f);

		_moveTweenCurrent.Finished += previous.Hide;
		
		_openedPages.Add(page);
	}

	public void PreviousPage() {
		var pageToQuit = _openedPages[^1];
		_openedPages.Remove(pageToQuit);

		var newPage = (Node2D)_openedPages[^1];
		
		_moveTweenCurrent = TweenUtils.RemakeTransitionTween(CreateTween, _moveTweenCurrent);
		_moveTweenOther = TweenUtils.RemakeTransitionTween(CreateTween, _moveTweenOther);

		var offset = pageToQuit.GetButtonOffsetFromPreviousPage();
		
		newPage.GlobalPosition = new Vector2(-offset.Item1, -offset.Item2);
		newPage.Show();
		
		_moveTweenCurrent.TweenProperty(
			(Node2D)pageToQuit, 
			"position", 
			new Vector2(offset.Item1, offset.Item2),
			0.15f);
		
		_moveTweenOther.TweenProperty(
			newPage, 
			"position", 
			new Vector2(0, 0),
			0.15f);

		_moveTweenCurrent.Finished += ((Node2D)pageToQuit).Hide;
	}

	public void CloseMenuSafe() {
		// This ensures every node is hidden as it should be.
		while (Hud.Instance.PauseMenu.IsShown)
			Hud.Instance.PauseMenu.OnEscapePressed();
	}

	private void CloseMenu() {
		_appearTween = TweenUtils.RemakeTransitionTween(CreateTween, _appearTween);

		_openedPages.Clear();
		
		IsShown = false;
		Color = new Color(1, 1, 1, 1);
		_appearTween.TweenProperty(
			this, 
			"modulate", 
			Color.Color8(1, 1, 1, 0),
			0.1f);
		_appearTween.Finished += Hide;
	}

	private void OpenMenu() {
		_appearTween = TweenUtils.RemakeTransitionTween(CreateTween, _appearTween);
		
		MainPage.Show();
		_openedPages.Add(MainPage);
				
		Show();
		IsShown = true;
		Color = new Color(255, 255, 255, 0);
		_appearTween.TweenProperty(
			this, 
			"modulate", 
			Color.Color8(255, 255, 255, 255),
			0.1f);
		
		MainPage.UpdateButtons();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Pause") && Client.Instance.CurrentPlayer is not null) 
		{
			var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
			myGDScript.Call("_onClickButtonEvent");
			Hud.Instance.PauseMenu.OnEscapePressed();
		}
	}
}
