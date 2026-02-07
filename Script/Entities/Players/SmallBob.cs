using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players.Properties;
using Beyondourborders.Script.Overlay;
using Beyondourborders.Script.Weapons;
using Godot;

namespace Beyondourborders.Script.Entities.Players;

public partial class SmallBob : PlayerBase
{

	public SmallBob() : base(
		speed: 500.0f, 
		jumpVelocity: -1050.0f, 
		cameraDelay: 0, 
		cameraOffset: new Vector2(100, -100)
		)
	{ }
	
	protected override void SetCameraAndAuthority()
	{
		if (Client.Instance.IsGuest)
		{
			CameraObject.MakeCurrent();
			MultiplayerSynchronizer.SetMultiplayerAuthority(Multiplayer.GetUniqueId());
		}
		else {
			MultiplayerSynchronizer.SetMultiplayerAuthority(1);
		}
	}
	
	protected override AnimationType AttackAnimation()
	{
		return AnimationType.Attack;
	}

	public override bool AttackCondition()
	{
		return !AttackTimer.IsStopped() && AttackTimer.WaitTime - AttackTimer.TimeLeft <= 1f;
	}
	/*
	public override void NodeLoaded()
	{
		if (Client.Instance.IsGuest)
		{
			CameraObject.MakeCurrent();
			MultiplayerSynchronizer.SetMultiplayerAuthority(Multiplayer.GetUniqueId());
		}
		else {
			MultiplayerSynchronizer.SetMultiplayerAuthority(1);
		}
	}
	public override void ProcessControlledPhysics(double delta)
	{
		if (Input.IsActionJustPressed("Pause")) {
			Hud.Instance.PauseMenu.OnEscapePressed();
		}

		if (Client.Instance.IsSingleplayer && Hud.Instance.PauseMenu.IsShown) {
			// Multiplayer = the game continues while the menu is shown
			// Singleplayer = the game pauses
			// (doing it just as it is in Minecraft)
			// NOTE: As of now if in multiplayer you can still control the player while in the menu.
			// For now keeping it like this as I like it.
			PlayerSprite.Stop();
			CurrentAnimation = null;
			return;
		}
		
		if (CurrentHp <= 0) //died
		{
			// Petit truc pr Marine ou Maxence ou jsp qui a fait cette partie:
			// qd on f un truc avec des conditions ya deux moyens de le faire:
			// - Faire un if dans un if dans un if 
			// - Faire la condition inverse d'un if et retourner avant
			// C'est mieux de faire la deuxieme ici pour éviter que ca se deporte trop a gauche.
			return;
		}

		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * 3.5f;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump"))
		{
			if (!IsOnFloor() && !doubleJump)
			{
				doubleJump = true;
				velocity.Y = JumpVelocity;
			}

			if (!doubleJump || IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}
		}

		if (IsOnFloor() && doubleJump)
		{
			doubleJump = false;
		}

		Vector2 direction = Input.GetVector("Left", "Right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			PlayerSprite.FlipH = direction.X > 0;
			if (velocity.X == 0)
			{
				PlayerSprite.Play("idle");
			}
			else if (velocity.Y != 0)
			{
				PlayerSprite.Stop();
			}
			else
			{
				PlayerSprite.Play("run");
			}
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			PlayerSprite.Stop();
		}

		Velocity = velocity;
		MoveAndSlide();

		ProcessCamera();
	
		
		if (!Client.Instance.IsMultiplayer && Input.IsActionJustPressed("Switch Player")) {
			Client.Instance.SwitchPlayers();
			this.CameraHistory.Clear();
		}
	}

	public override void _Ready() {
		base._Ready();
		UpdateNavAgent();
		NavigationAgentTimer.Timeout += UpdateNavAgent;
		NavigationAgentTimer.Start();
	}
*/
	
}
