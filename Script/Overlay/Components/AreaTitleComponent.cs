using Godot;

namespace Beyondourborders.Script.Overlay.Components;

public partial class AreaTitleComponent : TextureRect
{
	private Tween _areaTitleTween;
	private Timer _areaTitleTimer;
	
	public override void _Ready()
	{
		// Force the size & ignore scaling so that you can throw any garbage you want at it and itll just always display "the same way"
		// edit afer: still use a 700x300, otherwise the thing may appear too down.
		_areaTitleTimer = GetNode<Timer>("AreaTitleTimer");
		
		ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		CustomMinimumSize = new Vector2(700, 300);
		StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		
		_areaTitleTimer.Timeout += HideAreaTitle;
	}

	public void ShowAreaTitle(string areaName, float displayTime)
	{
		if (_areaTitleTween != null && _areaTitleTween.IsValid())
		{
			_areaTitleTween.Kill();
		}
		
		// Note: abrupt title changes may cause this to abruptly change. Too obscure to fix rn
		// TODO: use imported images as that doesn't work when exported
		
		Texture = ImageTexture.CreateFromImage(Image.LoadFromFile($"res://Assets/Custom/Logo/Logo Bob.png"));//TODO modife en fonction des zones ?
		
		// check if timer doesn't cause issues (should be fine from small experiments.)
		_areaTitleTimer.WaitTime = displayTime;
		
		_areaTitleTween = CreateTween();
		_areaTitleTween.SetTrans(Tween.TransitionType.Quad);
		_areaTitleTween.SetEase(Tween.EaseType.InOut);
		
		Visible = true;
		_areaTitleTween.TweenProperty(
			this,
			"modulate",
			Color.Color8(255, 255, 255),
			1);
		
		_areaTitleTimer.Start();
	}

	private void HideAreaTitle()
	{
		if (_areaTitleTween != null && _areaTitleTween.IsValid())
		{
			_areaTitleTween.Kill();
		}
		
		_areaTitleTween = CreateTween();
		_areaTitleTween.SetTrans(Tween.TransitionType.Quad);
		_areaTitleTween.SetEase(Tween.EaseType.InOut);
		_areaTitleTween.TweenProperty(
			this, 
			"modulate", 
			Color.Color8(255, 255, 255, 0),
			1);
		_areaTitleTween.Finished += DisableAreaTitleViewOnTweenDone;
	}

	private void DisableAreaTitleViewOnTweenDone()
	{
		Visible = false;
		_areaTitleTween.Finished -= DisableAreaTitleViewOnTweenDone;
	}
}
