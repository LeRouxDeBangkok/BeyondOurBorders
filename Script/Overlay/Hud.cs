using Beyondourborders.Script.Overlay.Components;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Overlay;

public partial class Hud : CanvasLayer
{ 
	public AreaTitleComponent AreaTitle { get; private set; }
	public DebugComponent Debug { get; private set; }
	public TransitionScreenComponent TransitionScreen { get; private set; }
	public TextureProgressBar HpBar { get; private set; }
	public ColorRect HitEffect { get; private set; }
	public PauseMenu PauseMenu { get; private set; }
	public Inventory Inventory { get; private set; }
	
	public TextGenerique Credit { get; private set; }

	public bool End = false;
	public Label InventoryChat { get; private set; }
	protected float StartTime = 0;
	protected bool HaveStarted = false;
	private Timer _chatClearTimer;
	
	private static Hud _instance;
	public static Hud Instance
	{
		get
		{
			if (_instance == null) 
				GD.PushWarning("Hud instance is null !");
			
			return _instance;
		}
		private set
		{
			if (_instance != null)
				GD.PushWarning("HUD instance got set while not being null !");
			_instance = value;
		}
	}

	public override void _Ready()
	{
		Instance = this;
		AreaTitle = GetNode<AreaTitleComponent>("AreaTitle");
		Debug = GetNode<DebugComponent>("Debug");
		TransitionScreen = GetNode<TransitionScreenComponent>("TransitionScreen");
		HpBar = GetNode<TextureProgressBar>("HPBar");
		HitEffect = GetNode<ColorRect>("HitEffect");
		PauseMenu = GetNode<PauseMenu>("PauseMenu");
		Inventory = GetNode<Inventory>("Inventory");
		InventoryChat = GetNode<Label>("InventoryChat");
		Credit = GetNode<TextGenerique>("Generique");
		_chatClearTimer = new Timer();
		_chatClearTimer.OneShot = true;
		_chatClearTimer.WaitTime = 4.0;
		AddChild(_chatClearTimer);
		_chatClearTimer.Timeout += () => InventoryChat.Text = "";
	}

	public void OnInventoryChatUpdated(string message)
	{
		InventoryChat.Text = message;
		_chatClearTimer.Stop();
		_chatClearTimer.Start();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (End)
		{
			Credit.Start = true;
			Credit.Show();
		}
	}
}
