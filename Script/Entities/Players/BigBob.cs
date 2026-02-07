using System;
using System.Linq;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players.Properties;
using Beyondourborders.Script.Overlay;
using Beyondourborders.Script.Triggers;
using Godot;

namespace Beyondourborders.Script.Entities.Players;

public partial class BigBob : PlayerBase
{

	public AnimationPlayer Anime;
	public HitArea HitZone;
	public BigBob() : base(
		speed: 500.0f, 
		jumpVelocity: -1050.0f,
		cameraDelay: 0,
		cameraOffset: new Vector2(100, -100)// Bit to the left since player is always going right.
		) 
	{ }
	protected override void SetCameraAndAuthority()
	{
		MultiplayerSynchronizer.SetMultiplayerAuthority(1);
		
		if (Client.Instance.IsHost)
		{
			CameraObject.MakeCurrent();
		}
	}
	
	public override void _Ready() {
		base._Ready();
		UpdateNavAgent();
		NavigationAgentTimer.Timeout += UpdateNavAgent;
		NavigationAgentTimer.Start();
		Anime = GetNode<AnimationPlayer>("AnimationPlayer");
		HitZone = GetNode<HitArea>("HitZone");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		HitZone.Damage = Damage;
	}

	protected override AnimationType AttackAnimation()
	{
		return AnimationType.Anime;
	}
	public override bool AttackCondition()
	{
		return !AttackTimer.IsStopped(); 
	}
	/*
	private DashHandler _dashHandler;
	private bool _canDoubleJump = false;
	private bool _wallJump = false;
	private bool _isWallSliding = false;
	private bool _canRevive = false;

	public Inventory Inventory { get; set; }


	public override void NodeLoaded() {
		_dashHandler = new DashHandler(
			node: GetNode<Timer>("DashTimer"),
			dashVelocity: 2500.0f,
			dashDelayMs: 500,
			unlimitedAirDashes: false
		);
		Inventory = new Inventory();
		MultiplayerSynchronizer.SetMultiplayerAuthority(1);

		if (Client.Instance.IsHost)
		{
			CameraObject.MakeCurrent();
		}

		// Band-aid fix - avoid an unwanted animation (eg dash) playing when the player starts
		PlayerSprite.Play("idle");
		PlayerSprite.Stop();
	}*/



	// ====== Physic branches =====
	/*
	public Vector2 InputHandler(double delta, Vector2 velocity)
	{
		if (Input.IsActionJustPressed("Pause")) 
		{
			Hud.Instance.PauseMenu.OnEscapePressed();
		}
		
		if (Input.IsActionJustPressed("Jump"))
		{
			velocity = HandleJump(delta, velocity);
		}
		
		if (Input.IsActionPressed("Left"))
		{
			velocity = HandleWalk(delta, velocity, -1);
		}
		else if (Input.IsActionPressed("Right"))
		{
			velocity = HandleWalk(delta, velocity, 1);
		}
		else
		{
			velocity = HandleWalk(delta, velocity, 0);
		}
		
		if (Input.IsActionJustPressed("Dash"))
		{
			HandleDash();
		}
		if (Input.IsActionJustPressed("Attack") && AttackCondition())
		{
			HandleAttack();
		}

		return velocity;
	}

	protected Vector2 AddGravity(double delta, Vector2 velocity)
	{
		velocity += GetGravity() * (float)delta * 3.5f;
		return velocity;
	}

	protected void HandleAttack()
	{
		AttackTimer.Start();
	}

	protected void HandleDash()
	{
		var (canDash, dashCheckTime) = DashHandler.CanDash(IsOnFloor());
		if (canDash) DashHandler.PerformDash(dashCheckTime, PlayerSprite.FlipH);
	}
	protected Vector2 HandleJump(double delta, Vector2 velocity)
	{
		if (IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}
		else if (IsOnWall() && Abilities.Contains(UnlockableAbility.WallJump) && !IsWallJumping)
		{
			IsWallJumping = true;
			velocity.Y = JumpVelocity;
		}
		else if (CanDoubleJump && Abilities.Contains(UnlockableAbility.DoubleJump)) 
		{
			CanDoubleJump = false;
			velocity.Y = JumpVelocity;
		}
		return velocity;
	}

	protected Vector2 HandleWalk(double delta, Vector2 velocity, int direction)
	{
		if(direction != 0)
		{
			velocity.X = direction * Speed;
			PlayerSprite.FlipH = direction > 0;
			return velocity;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			return velocity;
		}
	}

	protected Vector2 AddOnWallGravity(double delta, Vector2 velocity)
	{
		if (!IsWallSliding && !IsWallJumping)
		{
			velocity.Y = 0;
			IsWallSliding = true;
		}
		if (IsWallSliding && !IsWallJumping)
		{
			velocity.Y += GetGravity().Y * (float)delta * 0.1f;
		}
		else
		{
			velocity.Y += GetGravity().Y * (float)delta * 3.5f;
		}

		return velocity;
	}

	private void HandleNormalPhysic(double delta)
	{
		Vector2 velocity = Velocity;
		if (!IsOnFloor() && !IsOnWall())
		{
			velocity = AddGravity(delta, velocity);
		}
		if (IsOnFloor())
		{
			IsWallJumping = false;
			IsWallSliding = false;
			DashHandler.ResetAirStatus();
		}
		if (IsOnWall() && Abilities.Contains(UnlockableAbility.WallJump))
		{
			velocity = AddOnWallGravity(delta, velocity);
		}
		if (IsOnFloor() && !CanDoubleJump) // Double jump reset
		{
			CanDoubleJump = true;
		}

		velocity = InputHandler(delta, velocity);
		Velocity = velocity;
	}
	
	private void HandleDashPhysic() 
	{
		Velocity = DashHandler.GetDashVelocityVector();
	}
	
	
	// ===== DEATH =====
	
	private void EveryoneIsDead() 
	{
		var scene = ResourceLoader.Load<PackedScene>("res://scene/Menu/Accueil.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		this.Hide();
	}

	private void JustOneBobDied()
	{
		PlayerSprite.RotationDegrees = 90;
		if (CanRevive )  //l'autre bob est dans un rayon de 10pixel du premier bob
		{
			CurrentHp = 25;
		}
	}

	private void _on_detection_body_entered(Node2D bob)
	{
		if (bob is SmallBob)
		{
			CanRevive = true;
		}
	}
	
	// ===== Animation =====
	public override void UpdateAnimation() {
		AnimationType newAnimation;

		if (CurrentHp <= 0)
		{
			newAnimation = AnimationType.Death;
		}
		else if (DashHandler.State != Dashing.Not) {
			newAnimation = AnimationType.Dash;
		}
		else if (Velocity.X == 0 && Velocity.Y == 0) // Not moving
		{
			newAnimation = AnimationType.Idle;
		}
		else if (Velocity.Y == 0) { // Moving only horizontally
			newAnimation = AnimationType.Run;
		}
		else { // Moving either only vertically or vertically + horizontally
			newAnimation = AnimationType.None; // Should be AnimationType.Jump - NOT YET IMPLEMENTED
		}
			
		// Apply new animation state
		if (newAnimation == AnimationType.None && CurrentAnimation != AnimationType.None)
		{
			// GD.Print("Set animation to None");
			CurrentAnimation = AnimationType.None;
			PlayerSprite.Stop();
		}
		else if (newAnimation != CurrentAnimation)
		{
			// GD.Print($"Stopped animation {CurrentAnimation} & set it to {newAnimation}");
			PlayerSprite.Stop();
			CurrentAnimation = newAnimation;
			PlayerSprite.Play(CurrentAnimation.ToString().ToLower());
		}
	}*/
}
