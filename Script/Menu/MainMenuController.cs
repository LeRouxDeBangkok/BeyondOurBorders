using System.Collections.Generic;
using Beyondourborders.Script.Menu.Components;
using Beyondourborders.Script.Utils;
using Godot;
using GodotPlugins.Game;

namespace Beyondourborders.Script.Menu;

// Copié collé de la classe "PauseMenu" dans l'overlay.
// Si possible faudrait réunir les deux psk c les mm la.
// Edit: ici ca gere le background aussi dcp a voir.
public partial class MainMenuController : Node2D {
	public static MainMenuController Instance { get; private set; }

	public MainMenuController() {
		Instance = this;
	}
	
	public TextureRect Background { get; private set; }
	
	public MainPage MainPage { get; private set; }
	public SettingsPage SettingsPage { get; private set; }
	public QuitConfirmationPage QuitConfirmation { get; private set; }
	public SaveSelectionPage SaveSelectionPage { get; private set; }
	public NewSaveConfirmationPage NewSaveConfirmationPage { get; private set; }
	public SaveDeleteConfirmationPage SaveDeleteConfirmationPage { get; private set; }
	public JoinHostIpPortSelectionPage JoinHostIpPortSelectionPage { get; private set; }

	private List<IMenuPage> _openedPages;
	
	private Tween _appearTween;
	private Tween _moveTweenCurrent;
	private Tween _moveTweenOther;
	private Tween _backgroundMoveTween;
	// public bool IsShown { get; private set; } = false;
	
	public override void _Ready() {
		Background = GetNode<TextureRect>("Background");
		
		MainPage = GetNode<MainPage>("MainPage");
		SettingsPage = GetNode<SettingsPage>("SettingsPage");
		QuitConfirmation = GetNode<QuitConfirmationPage>("QuitConfirmPage");
		SaveSelectionPage = GetNode<SaveSelectionPage>("SaveSelectionPage");
		NewSaveConfirmationPage = GetNode<NewSaveConfirmationPage>("NewSaveConfirmationPage");
		SaveDeleteConfirmationPage = GetNode<SaveDeleteConfirmationPage>("SaveDeleteConfirmationPage");
		JoinHostIpPortSelectionPage = GetNode<JoinHostIpPortSelectionPage>("JoinHostIpPortSelectionPage");
		
		_openedPages = new List<IMenuPage>();
		
		OpenMenu();
	}

	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("Pause")) {
			OnEscapePressed();
		}
	}

	public void OnEscapePressed() {
		// if (IsShown) {
		if (_openedPages.Count == 1) {
			ShowPage(QuitConfirmation);
			return;
		}
		PreviousPage();
		// }
		// else {
		//     OpenMenu();
		// }
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

		var bgOffset = page.GetBackgroundOffsetFromPreviousPage();
		_backgroundMoveTween = TweenUtils.RemakeTransitionTween(CreateTween, _backgroundMoveTween);
		_backgroundMoveTween.TweenProperty(
			Background, 
			"position", 
			new Vector2(Background.GlobalPosition.X - bgOffset.Item1, Background.GlobalPosition.Y - bgOffset.Item2),
			0.15f);
		
		_openedPages.Add(page);
	}

	public void PreviousPage() {
		var pageToQuit = _openedPages[^1];
		_openedPages.Remove(pageToQuit);

		var newPage = (Node2D)_openedPages[^1];
		
		var offset = pageToQuit.GetButtonOffsetFromPreviousPage();
		
		newPage.GlobalPosition = new Vector2(-offset.Item1, -offset.Item2);
		newPage.Show();
		
		_moveTweenCurrent = TweenUtils.RemakeTransitionTween(CreateTween, _moveTweenCurrent);
		_moveTweenCurrent.TweenProperty(
			(Node2D)pageToQuit, 
			"position", 
			new Vector2(offset.Item1, offset.Item2),
			0.15f);
		
		_moveTweenOther = TweenUtils.RemakeTransitionTween(CreateTween, _moveTweenOther);
		_moveTweenOther.TweenProperty(
			newPage, 
			"position", 
			new Vector2(0, 0),
			0.15f);

		_moveTweenCurrent.Finished += ((Node2D)pageToQuit).Hide;
		
		var bgOffset = pageToQuit.GetBackgroundOffsetFromPreviousPage();
		_backgroundMoveTween = TweenUtils.RemakeTransitionTween(CreateTween, _backgroundMoveTween);
		_backgroundMoveTween.TweenProperty(
			Background, 
			"position", 
			new Vector2(Background.GlobalPosition.X + bgOffset.Item1, Background.GlobalPosition.Y + bgOffset.Item2),
			0.15f);
	}

	// private void CloseMenu() {
	//     _appearTween = TweenUtils.RemakeTransitionTween(CreateTween, _appearTween);
	//
	//     _openedPages.Clear();
	//     
	//     IsShown = false;
	//     Modulate = new Color(1, 1, 1, 1);
	//     _appearTween.TweenProperty(
	//         this, 
	//         "modulate", 
	//         Color.Color8(1, 1, 1, 0),
	//         0.1f);
	//     _appearTween.Finished += Hide;
	// }

	private void OpenMenu() {
		_appearTween = TweenUtils.RemakeTransitionTween(CreateTween, _appearTween);
		
		MainPage.Show();
		_openedPages.Add(MainPage);
		
		Show();
		// IsShown = true;
		Modulate = new Color(1, 1, 1, 0);
		_appearTween.TweenProperty(
			this, 
			"modulate", 
			new Color(1, 1, 1, 1),
			0.5f);
	}
}
