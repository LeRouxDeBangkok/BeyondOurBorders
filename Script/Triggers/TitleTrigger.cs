using Beyondourborders.Script.Overlay;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Triggers;

public enum Orientation
{
	LEFT,
	RIGHT,
	BOTH
}

// [Tool]
public partial class TitleTrigger : SimpleResizableArea
{
	[Export] public Orientation DisplayTitleOnEnterFrom { get; set; } = Orientation.LEFT;
	[Export] public float DisplayTime { get; set; } = 5.0f;
	[Export] public string TitleImagePath { get; set; }
	private float _middleX;
	
	public TitleTrigger() : base("TitleAreaTrigger") {}
	
	public override void _Ready() {
		_middleX = GlobalPosition.X;

		BodyEntered += TriggerTitle;
	}
	
	public void TriggerTitle(Node2D body)
	{
		if (!IsCurrentActivePlayer(body))
			return;

		bool enteringLeft = body.GlobalPosition.X < _middleX;
		if (DisplayTitleOnEnterFrom != Orientation.BOTH &&
			((DisplayTitleOnEnterFrom == Orientation.LEFT && !enteringLeft) ||
			 (DisplayTitleOnEnterFrom == Orientation.RIGHT && enteringLeft)))
			return;
		
		Hud.Instance.AreaTitle.ShowAreaTitle(TitleImagePath, DisplayTime);
	}
}
