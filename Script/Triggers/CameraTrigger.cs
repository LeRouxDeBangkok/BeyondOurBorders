using Beyondourborders.Script.Camera;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Triggers;

// [Tool]
public partial class CameraTrigger : SimpleResizableArea {
	[Export] public float ZoomInside { get; set; } = 1.0f;
	[Export] public float ZoomInsideDuration { get; set; } = 0.5f;
	[Export] public Vector2 CameraInsideOffset { get; set; } = Vector2.Zero;
	
	[Export] public float ZoomLeft { get; set; } = 1.0f;
	[Export] float ZoomLeftDuration { get; set; } = 0.5f;
	[Export] public Vector2 CameraLeftOffset { get; set; } = Vector2.Zero;
	
	[Export] public float ZoomRight { get; set; } = 1.0f;
	[Export] public float ZoomRightDuration { get; set; } = 0.5f;
	[Export] public Vector2 CameraRightOffset { get; set; } = Vector2.Zero;
	
	private float middleX;
	// TODO? : Allow different on leave bottom/top (?)
	// private float middleY;
	
	public CameraTrigger() : base("CameraZoomAreaTrigger") {}
	
	public override void _Ready() {
		middleX = GlobalPosition.X;
		
		BodyEntered += TriggerCameraZoom;
		BodyExited += TriggerCameraUnZoom;
	}

	// Wide putin effect: new Vector2(4f, 2f)
	
	public void TriggerCameraZoom(Node2D body) {
		if (!IsFromThisClient(body))
			return;
		
		var camera = body.GetNode<BetterCamera>("Camera2D");
		camera.SetZoom(ZoomInside, ZoomInsideDuration);
		camera.SetOffset(CameraInsideOffset, ZoomInsideDuration);
	}
	public void TriggerCameraUnZoom(Node2D body)
	{
		if (!IsFromThisClient(body))
			return;

		var camera = body.GetNode<BetterCamera>("Camera2D");

		if (body.GlobalPosition.X > middleX)
		{
			camera.SetZoom(ZoomRight, ZoomRightDuration);
			camera.SetOffset(CameraRightOffset, ZoomRightDuration);
		}
		else
		{
			camera.SetZoom(ZoomLeft, ZoomLeftDuration);
			camera.SetOffset(CameraLeftOffset, ZoomLeftDuration);
		}
		
	}
}
