using System;
using Godot;

namespace Beyondourborders.Script.Camera;

public partial class BetterCamera : Camera2D
{
	private Vector2 _targetZooms = new Vector2(1.0f, 1.0f); 
	private Tween _zoomTween;
	
	private Tween _offsetTween;


	public void SetZoom(float newZoom, float duration = 0.5f)
	{
		SetZoom(Vector2.One * newZoom, duration);
	}
	
	public void SetZoom(Vector2 newZooms, float duration = 0.5f) 
	{
		// Rider fix - avoid float inaccuracies & use Abs instead of just ==.
		if (Math.Abs((_targetZooms - newZooms).Length()) < 0.01)
			return;
		
		_targetZooms = newZooms;
		
		if (_zoomTween != null && _zoomTween.IsValid())
		{
			_zoomTween.Kill();
		}
		
		_zoomTween = CreateTween();
		_zoomTween.SetTrans(Tween.TransitionType.Quad); // Quad is tbe best looking in my opinion
		_zoomTween.SetEase(Tween.EaseType.InOut);
		
		_zoomTween.TweenProperty(this, "zoom", _targetZooms, duration);
	}
	
	public Vector2 GetTargetZoom()
	{
		return _targetZooms;
	}

	public void SetOffset(Vector2 newOffset, float duration = 0.5f)
	{
		if (_offsetTween != null && _offsetTween.IsValid())
		{
			_offsetTween.Kill();
		}
		
		_offsetTween = CreateTween();
		_offsetTween.SetTrans(Tween.TransitionType.Quad); // Quad is tbe best looking in my opinion
		_offsetTween.SetEase(Tween.EaseType.InOut);
		
		_offsetTween.TweenProperty(this, "offset", newOffset, duration);
	}

	public void CameraShake(int strength = 30, float fade = 5.0f) // TODO fait vibrer la cam mais j'y arrive pas bob stp j'suis sur en 2min tu le fais
	{
		
	}

}