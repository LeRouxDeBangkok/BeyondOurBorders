/*using System;
using Godot;

namespace Beyondourborders.Script.Utils;

// Why is this here with this name?
// Basically this class works entirely, it technically has no problems, it doesn't do
// anything that shouldn't be normal for Godot.
// Unfortunately, Tool scripts seem to be VERY buggy with godot.
// Eg when you recompile something after an error, you'd get an "ObjectDisposedException"
// which forces you to fully reload Godot.
// Sometimes you'd also get random errors and the whole thing wasn't stable anyway...
//
// This seems to be a regression, as Godot 4.0.3 doesn't seem to have that issue.
// I doubt this'll get fixed soon, making a proper PR would take too long and
// making a band aid fix would be annoying + would need rebuilding for all platforms
// since we use Windows, Linux and MacOS.
// So for now not using that fancy thing.
enum Handle
{
	TOPRIGHT,
	TOPLEFT,
	BOTTOMRIGHT,
	BOTTOMLEFT,
	RIGHT,
	LEFT,
	TOP,
	BOTTOM
}

[Tool]
public partial class GodotBuggedResizableArea : Area2D
{
	// Used to not re call the EnterTree function after the first time 
	// Which happens if you switch back and forth between multiple scenes in the editor.
	private bool _firstTreeEnterDone;
	
	// Used to prevent EnterTree from adding child CollisionShapes to the scene files themselves,
	// If this wasn't there, every time you opened the eg TitleTrigger scene, it would add a CollisionShape there
	// and consequentely add one in ALL instances of it (+it would be the same instance of it everywhere).
	// This isn't the most elegant but works well.
	private string _rootSceneName;
	private bool ShouldAddChild => Engine.IsEditorHint() && EditorInterface.Singleton.GetEditedSceneRoot().Name != _rootSceneName;
	
	protected CollisionShape2D CollisionShape;
	protected RectangleShape2D RectangleShape;

	private const float HandleSize = 5f;
	private const float HandleOffset = -8f;
	private const float MinSize = 50f;
	private bool _isResizing = false;
	private Vector2 _resizeStart;
	private Handle _currentHandle;
	private Vector2 _currentPosition;
	
	private bool _isSelected;
	
	private bool ShouldRenderHandles => _isSelected || HandleOffset > 1 || !ShouldAddChild;

	// Save & reload rectangle size.
	[Export]
	public Vector2 RectangleSize
	{
		get => RectangleShape.Size;
		set
		{
			RectangleShape.Size = value;
			OnSizeChange();
		}
	}

	private Callable _onSelectionChanged;

	public GodotBuggedResizableArea(string rootSceneName)
	{
		_rootSceneName = rootSceneName;

		_onSelectionChanged = Callable.From(OnSelectionChanged);
		CollisionShape = new CollisionShape2D();
		RectangleShape = new RectangleShape2D();
		CollisionShape.Shape = RectangleShape;
		// last line in EnterTree (AddChild)
		//
		// Replaces the following:
		// _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		// _rectangleShape = (RectangleShape2D)_collisionShape.GetShape();
		// Makes it so that we're not tied to inner nodes.
	}
	
	public override void _EnterTree()
	{
		// Playing - only add the child for the collision checks, NOTHING ELSE
		if (!Engine.IsEditorHint())
		{
			AddChild(CollisionShape);
			return;
		}

		if (CollisionShape == null)
		{
			GD.Print("null CollissionShaspe? (this happens sometimes)");
			GD.Print(_rootSceneName);
			return;
		}
		// GD.Print("Object entered tree: " + GetType());
			

		if (ShouldAddChild && !_firstTreeEnterDone)
		{
			AddChild(CollisionShape);
			CollisionShape.Owner = this;
		}

		if (!_firstTreeEnterDone)
		{
			var selectedNodes = EditorInterface.Singleton.GetSelection().Connect("selection_changed", _onSelectionChanged);
			_firstTreeEnterDone = true;
		}
		
		OnSelectionChanged();
	}

	public override void _ExitTree()
	{
		GD.Print("exitesd area: " + GetType());
		EditorInterface.Singleton.GetSelection().Disconnect("selection_changed", _onSelectionChanged);
		base._ExitTree();
	}


	protected override void Dispose(bool disposing) {
		GD.Print("Dissposed of area: " + GetType());
		EditorInterface.Singleton.GetSelection().Disconnect("selection_changed", _onSelectionChanged);
		
		base.Dispose(true);
	}

	public void OnSizeChange()
	{
		// Avoid error at godot init.
		if (CollisionShape == null)
			return;
		RectangleShape = (RectangleShape2D)CollisionShape.GetShape();
		QueueRedraw();
	}
	
	public void OnSelectionChanged()
	{
		_isSelected = EditorInterface.Singleton.GetSelection().GetSelectedNodes().Contains(this);
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (!Engine.IsEditorHint())
			return;
		
		//GD.Print("Draw 17");
		//GetViewport().GlobalCanvasTransform.Y.Y;
		
		Vector2 size = RectangleShape.Size;
		
		DrawRect(new Rect2(- size / 2, size), new Color(0, .6f, .7f, .42f));
		
		if (ShouldRenderHandles)
		{
			DrawHandle(new Vector2(-size.X / 2 - HandleOffset, -size.Y / 2 - HandleOffset), "Top Left");
			DrawHandle(new Vector2(size.X / 2 + HandleOffset, -size.Y / 2 - HandleOffset), "Top Right");
			DrawHandle(new Vector2(-size.X / 2 - HandleOffset, size.Y / 2 + HandleOffset), "Bottom Left");
			DrawHandle(new Vector2(size.X / 2 + HandleOffset, size.Y / 2 + HandleOffset), "Bottom Right");
		
			DrawHandle(new Vector2(0, size.Y / 2 + HandleOffset), "Top");
			DrawHandle(new Vector2(0, -size.Y / 2 - HandleOffset), "Bottom");
			DrawHandle(new Vector2(-size.X / 2 - HandleOffset, 0), "Left");
			DrawHandle(new Vector2(size.X / 2 + HandleOffset, 0), "Right");
		}
	}
	
	private void DrawHandle(Vector2 position, string label)
	{
		DrawCircle(position, HandleSize, new Color(1, 1, 1, 1f));
		DrawCircle(position, HandleSize-0.5f, new Color(1, 0, 0, 1f));
		DrawArc(position, HandleSize+0.3f, 0, (float)Math.Tau, 30, new Color(0, 0, 0, 1f));
	}
	
	public override void _Process(double delta)
	{
		if (!Engine.IsEditorHint())
			return;


		if (!ShouldRenderHandles)
		{
			return;
		}
			
		if (_isResizing)
		{
			// Why is that here?
			// Tldr; 1.5h long debugging session thinking something was wrong with my math.
			// Turns out, if the HandleOffset is negative, the handles are inside of the element.
			// This has the amazing feature of making the element move WHILE resizing.
			// This makes the rectangle get distorded weirdly.
			// To fix, capture the position, then on next iteration check if it changed before doing anyting
			// (it shouldn't have) and if it did just snap it back to its previous posision.
			if (_currentPosition != GlobalPosition)
				GlobalPosition = _currentPosition;
			
			Vector2 mousePos = GetGlobalMousePosition();
			Vector2 offsetFromDir = (mousePos - _resizeStart);
			Vector2 initialSize = RectangleShape.Size;
			
			Vector2? newSize = null;
			
			switch (_currentHandle)
			{
				case Handle.TOPLEFT:
					newSize = new Vector2(Math.Max(MinSize, initialSize.X - offsetFromDir.X), Math.Max(MinSize, initialSize.Y - offsetFromDir.Y));
					Position += new Vector2(offsetFromDir.X/2, offsetFromDir.Y/2);
					break;

				case Handle.TOPRIGHT:
					newSize = new Vector2(Math.Max(MinSize, initialSize.X + offsetFromDir.X), Math.Max(MinSize, initialSize.Y - offsetFromDir.Y));
					Position += new Vector2(offsetFromDir.X/2, offsetFromDir.Y/2);
					break;

				case Handle.BOTTOMLEFT:
					newSize = new Vector2(Math.Max(MinSize, initialSize.X - offsetFromDir.X), Math.Max(MinSize, initialSize.Y + offsetFromDir.Y));
					Position += new Vector2(offsetFromDir.X/2, offsetFromDir.Y/2);
					break;

				case Handle.BOTTOMRIGHT:
					newSize = new Vector2(Math.Max(MinSize, initialSize.X + offsetFromDir.X), Math.Max(MinSize, initialSize.Y + offsetFromDir.Y));
					Position += new Vector2(offsetFromDir.X/2, offsetFromDir.Y/2);
					break;

				case Handle.TOP:
					newSize = new Vector2(initialSize.X, Math.Max(MinSize, initialSize.Y - offsetFromDir.Y));
					Position += new Vector2(0, offsetFromDir.Y/2);
					break;

				case Handle.BOTTOM:
					newSize = new Vector2(initialSize.X, Math.Max(MinSize, initialSize.Y + offsetFromDir.Y));
					Position += new Vector2(0, offsetFromDir.Y/2);
					break;

				case Handle.LEFT:
					newSize = new Vector2(Math.Max(MinSize, initialSize.X - offsetFromDir.X), initialSize.Y);
					Position += new Vector2(offsetFromDir.X/2, 0);
					break;

				case Handle.RIGHT:
					newSize = new Vector2(Math.Max(MinSize, initialSize.X + offsetFromDir.X), initialSize.Y);
					Position += new Vector2(offsetFromDir.X/2, 0);
					break;
			}

			RectangleShape.Size = newSize!.Value;
			_currentPosition = GlobalPosition;

			_resizeStart = mousePos;
			OnSizeChange();
		}
		
		bool mousePressed = Input.IsMouseButtonPressed(MouseButton.Left);

		if (!mousePressed)
		{
			_isResizing = false;
		} else if (ShouldRenderHandles) {
			Vector2 mousePos = GetGlobalMousePosition();
			Vector2 position = GlobalPosition;
			Vector2 size = RectangleShape.Size;

			(Handle handle, Vector2 vector)[] collisionList =
			{
				// Note: bottom and top are inverted because the thing is relative to the middle, so negative is top and positive is bottom
				(Handle.TOPLEFT, new Vector2(-size.X / 2 - HandleOffset, -size.Y / 2 - HandleOffset)), // Top Left
				(Handle.TOPRIGHT, new Vector2(size.X / 2 + HandleOffset, -size.Y / 2 - HandleOffset)), // Top Right
				(Handle.BOTTOMLEFT, new Vector2(-size.X / 2 - HandleOffset, size.Y / 2 + HandleOffset)), // Bottom Left
				(Handle.BOTTOMRIGHT, new Vector2(size.X / 2 + HandleOffset, size.Y / 2 + HandleOffset)), // Bottom Right
				(Handle.BOTTOM, new Vector2(0, size.Y / 2 + HandleOffset)), // Bottom
				(Handle.TOP, new Vector2(0, -size.Y / 2 - HandleOffset)), // Top 
				(Handle.LEFT, new Vector2(-size.X / 2 - HandleOffset, 0)), // Left
				(Handle.RIGHT, new Vector2(size.X / 2 + HandleOffset, 0)) // Right
			};

			foreach (var (handle, vector) in collisionList)
			{
				if (mousePos.DistanceTo(position + vector) < HandleSize)
				{
					_isResizing = true;
					_resizeStart = mousePos;
					_currentHandle = handle;
					_currentPosition = position;
					break;
				}
			}
		}
	}
}*/