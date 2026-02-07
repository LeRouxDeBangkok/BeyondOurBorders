using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beyondourborders.Script.Camera;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players.Properties;
using Beyondourborders.Script.Weapons;
using Beyondourborders.Script.Overlay;
using Beyondourborders.Script.Triggers;
using Beyondourborders.Script.Utils;
using Beyondourborders.Script.ClientData;
using Godot;

using Godot.Collections;

namespace Beyondourborders.Script.Entities.Players;

public abstract partial class PlayerBase : EntityBase {
	public string UglyLevelRawDoNotUseCMoiPtnCLaMerde;
	
	private void HandleNewZoneDeferred(string newValue) {
		if (this == Client.Instance.CurrentPlayer) {
			if (newValue == Client.Instance.OtherPlayer.Zone)
				Client.Instance.OtherPlayer.EnablePlayerOnThisClient();
			else {
				Client.Instance.OtherPlayer.DisablePlayerOnThisClient();
			}
		}
		else {
			if (newValue == Client.Instance.CurrentPlayer.Zone)
				Client.Instance.OtherPlayer.EnablePlayerOnThisClient();
			else
				Client.Instance.OtherPlayer.DisablePlayerOnThisClient();
		}
		GD.Print($"Setting zone for player {Name} to {newValue} (Client: {Client.Instance.CurrentPlayer?.Name ?? "Unknown"})");
	}
	public override string Zone {
		get => RawZone;
		set {
			if (value == RawZone)
				return;
			if (Client.Instance.CurrentPlayer is null) {
				GD.Print($"Initializing zone to {value}");
				RawZone = value;
				return;
			}

			if (Client.Instance.IsMultiplayer) {
				if (this == Client.Instance.CurrentPlayer) {
					WaitUtils.WaitUntil(
						() => Client.Instance.IsLoading,
						() => CallDeferred("HandleNewZoneDeferred", value)
					);
				}
				else {
					CallDeferred("HandleNewZoneDeferred", value);
				}
			}
			if (Client.Instance.IsMultiplayer) {
				try {
					GhettoMobSynchronizerManager.Instance.Rpc("RightBeforePlayerChangeZone", this, value);
				}
				catch {
					GD.Print("LOL THING FAILED");
				}
			}
			
			GD.Print("RawZone change succeedded!!");
			
			RawZone = value;
		}
	}
	
	
	public RayCast2D RayCastLeft;
	public RayCast2D RayCastRight;

	public int Damage
	{
		get
		{
			if (GameData.CurrentGameData.Weapon == 1) return 15;
			if (GameData.CurrentGameData.Weapon == 2) return 25;
			return 0;
		}
	}
	

	public DashHandler DashHandler;
	protected PhyisqueHandler PhyisqueHandler;
	protected bool CanRevive = false;
	
	public Vector2 CameraOffset { get; set; }
	public BetterCamera CameraObject;
	protected readonly Queue<Vector2> CameraHistory;
	protected int CameraDelay;
	
	protected MultiplayerSynchronizer MultiplayerSynchronizer;
	protected NavigationAgent2D NavigationAgent;
	protected Timer NavigationAgentTimer;

	public InteractiveAreaTrigger CurrentInteractArea;
	
	public TextureProgressBar HpBar;
	public Inventory Inventory { get; set; }
	
	
	public bool OnWall
	{
		get
		{
			return RayCastRight.IsColliding() || RayCastLeft.IsColliding();
		}
	}


	protected abstract void SetCameraAndAuthority();
	public virtual void NodeLoaded() 
	{
		DashHandler = new DashHandler(
			node: GetNode<Timer>("DashTimer"),
			dashVelocity: 2500.0f,
			dashDelayMs: 500,
			unlimitedAirDashes: false
		);
		PhyisqueHandler = new PhyisqueHandler(this);

					SetCameraAndAuthority();
		
		PlayerSprite.Play("idle");
		PlayerSprite.Stop();

		UglyLevelRawDoNotUseCMoiPtnCLaMerde = "";
	}
	public PlayerBase(float speed, float jumpVelocity, int cameraDelay, Vector2 cameraOffset) 
		: base (speed : speed,jumpVelocity:jumpVelocity) 
	{
		this.CameraHistory = new Queue<Vector2>();
		this.CameraDelay = cameraDelay;
		this.CameraOffset = cameraOffset;
	}

	public PlayerBase() {
		throw new Exception(
			"This constructor only exists to avoid warnings. Please call the PlayerBase constructor with arguments.");
	}

	// Commentaire de comment ça marche ici pour les gens qui essayeront de comprendre apres:
	// Cette classe c'est une classe abstracte, elle peut définir soit des fonctions deja faites soit des fonctions vides (abstraites)
	// que les classes qui heritent de celles-ci doivent implementer.
	// Ici par exemple, la fonction _Ready() est une fonction abstraite (a implémenter par nous) de CharacterBody2D (ou d'une des classes dont elle-meme herite).
	// Ce qui est fait ici est:
	// - On implémente la fonction _Ready de godot, qui fait les trucs essentiels a tous les joueurs (pr l'instant recuperer les sous node)
	// - On définit une fonction abstraite NodeLoaded()
	// - On appelle cette fonction abstraite a la fin de l'implémentation de _Ready
	//
	// Ca peut paraitre inutile mais ca fait qu'au final, dans les classes des joueurs, on va simplement definir NodeLoaded au lieu de
	// Ready, et vu qu'elle est appelee apres avoir fait tous les trucs essentiels pour un joueur, on n'aura pas a les refaire
	// pour chaque joueur (la ou avant on aurait du reecrire tout le contenu de Ready dans chaque classe joueur)
	// 
	// O final ca rend le tt un peu + clean

	public override void _Ready() 
	{
		HitDelay = 2.0f;

		base._Ready();
		
		Inventory = Hud.Instance?.GetNodeOrNull<Inventory>("Inventory");
		
		RayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		RayCastRight = GetNode<RayCast2D>("RayCastRight");
		
		this.MultiplayerSynchronizer = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");
		
		this.NavigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		this.NavigationAgentTimer = NavigationAgent.GetNode<Timer>("NavigationAgentTimer");

		this.HpBar = Hud.Instance.HpBar;
		
		this.CameraObject = GetNode<BetterCamera>("Camera2D");
		
		NodeLoaded();
	}

	public void Collect(global::Item item)
	{
		var inventory = Inventory;
	
		if (inventory == null)
		{
			GD.PrintErr("Inventory node not found.");
			return;
		}

		if (!inventory.IsInsideTree())
		{
			GD.Print("Waiting for inventory to be inside tree...");
			CallDeferred(nameof(Collect), item);
			return;
		}

		if (!inventory.IsInitialized())
		{
			GD.Print("Inventory not initialized, deferring collect...");
			CallDeferred(nameof(Collect), item);
			return;
		}

		inventory.Add(item);
	}



	//======== Physique =======
	
	
	// Pareil qu'au dessus, mais pour juste ne pas exécuter ProcessPhysics si sur le mauvais client.
	public override void _PhysicsProcess(double delta) 
	{
		
		if (Client.Instance.IsLoading || Client.Instance.IsSaving)
			return;

		if (!Client.Instance.IsMultiplayer && Input.IsActionJustPressed("Switch Player")) {
			Client.Instance.SwitchPlayers();
			this.CameraHistory.Clear();
		}
		
		
		if (Client.Instance.IsMultiplayer) {
			if (HasAuthority())
				ProcessControlledPhysics(delta);
		}
		else {
			if (IsCurrentPlayer())
				ProcessControlledPhysics(delta);
			else
				ProcessBotPhysics(delta);
		}
	}
	
	public void ProcessControlledPhysics(double delta)
	{
		var myGDScript = GetTree().Root.GetNode<Client>("Main").GetNode("SoundManager").GetNode("Node");
		myGDScript.Call("_changeCombatValueEvent", Client.Instance.AggroLevel);
		if (Client.Instance.IsSingleplayer && (Hud.Instance.PauseMenu.IsShown || Hud.Instance.Inventory.Visible)) 
		{
			PlayerSprite.Stop();
			CurrentAnimation = null;
			return;
		}
		
		// if (Client.Instance.OtherPlayer.CurrentHp <= 0)
		// {
		// 	// EveryoneIsDead();
		// }
		// else
		// {
		// 	JustOneBobDied();
		// }

		// PlayerSprite.RotationDegrees = 0;

		
		if (DashHandler.State != Dashing.Not) {
			Velocity = PhyisqueHandler.HandleDashPhysic();
		} else {
			PhyisqueHandler.HandleNormalPhysic(delta);
		}
		
		MoveAndSlide();

		ProcessCamera();
	
		
		UpdateAnimation();
	}

	public bool checkWhetherShouldTp(Vector2 otherPlayDiff) {
		var zone11 = Client.Instance.CurrentPlayer.UglyLevelRawDoNotUseCMoiPtnCLaMerde;
		var zone12 = Client.Instance.CurrentPlayer.Zone;
		var zone21 = Client.Instance.OtherPlayer.UglyLevelRawDoNotUseCMoiPtnCLaMerde;
		var zone22 = Client.Instance.OtherPlayer.Zone;
		
		if ((zone11 == "Level3" || zone12 == "Level3" || zone21 == "Level3" || zone22 == "Level3" )) {
			GD.Print("Level 3: IA Disabled");
			return false;
		}
		
		if (Math.Abs(otherPlayDiff.X) < 1000 && Math.Abs(otherPlayDiff.Y) < 1000)
			return false;
			
		var p = Client.Instance.CurrentPlayer;
		if (!p.IsOnFloor())
			return false;

		if ((zone11 == "Level2" || zone12 == "Level2" || zone21 == "Level2" || zone22 == "Level2" ) && p.GlobalPosition.X is > 4300 and < 6300 &&
		    p.GlobalPosition.Y is > -1000 and < -600) {
			GD.Print("TUNNEL - NO TP 1");
			return false;
		}
		
		if ((zone11 == "Level4" || zone12 == "Level4" || zone21 == "Level4" || zone22 == "Level4" ) && p.GlobalPosition.X > -300) {
			GD.Print("TUNNEL - NO TP 2");
			return false;
		}
		// TODO: PAreil pour petit passage niveau 4.

		var offset = (p.PlayerSprite.FlipH) ? -30 : 30;
		var pos = new Vector2(p.GlobalPosition.X + offset, p.GlobalPosition.Y);
		GlobalPosition = pos;
		GD.Print("TELEPORTED !");
		GD.Print(Client.Instance.CurrentPlayer!.Zone);
		GD.Print(Client.Instance.CurrentPlayer!.UglyLevelRawDoNotUseCMoiPtnCLaMerde);
		GD.Print(Client.Instance.CurrentPlayer!.RawZone);
		GD.Print(Client.Instance.OtherPlayer!.Zone);
		GD.Print(Client.Instance.OtherPlayer!.UglyLevelRawDoNotUseCMoiPtnCLaMerde);
		GD.Print(Client.Instance.OtherPlayer!.RawZone);
		GD.Print(p.GlobalPosition.X);
		GD.Print(p.GlobalPosition.Y);
		return true;
	}
	public void ProcessBotPhysics(double delta) 
	{
		if (Client.Instance.IsSingleplayer && Hud.Instance.PauseMenu.IsShown)
			return;
 
		Vector2 velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * 3.5f;
		}
 
		// var JUMP_HEIGHT_TEMP = 169;
		if (!NavigationAgent.IsNavigationFinished()) {
			var nextPos = NavigationAgent.GetNextPathPosition();
			var nextPosDiff = nextPos - GlobalPosition;
			var otherPlayDiff = Client.Instance.CurrentPlayer.GlobalPosition - GlobalPosition;
 
			// GD.Print($"Target: {NavigationAgent.TargetPosition}, diff: {otherPlayDiff},  dir: {nextPosDiff}, reached: {NavigationAgent.IsNavigationFinished()}, distance: {NavigationAgent.DistanceToTarget()}");

			if (checkWhetherShouldTp(otherPlayDiff))
				return;
 
			PlayerSprite.FlipH = nextPosDiff.X > 0;
 
 
			if (Math.Abs(otherPlayDiff.X) > 100) {
				if (Math.Abs(nextPosDiff.X) > Speed) {
					velocity.X = (nextPosDiff.X > 200) ? Speed : -Speed;
				} else {
					velocity.X = otherPlayDiff.X;
				}
			}
 
			//
			// if (nextPosDiff.Y < -(JUMP_HEIGHT_TEMP/2) && IsOnFloor())
			// 	GD.Print(nextPosDiff.Y);
			// 	GD.Print(-(JUMP_HEIGHT_TEMP/2));
			// 	GD.Print(IsOnFloor());
			// 	velocity.Y = JumpVelocity;
			//
			// 	// PlayerSprite.FlipH = direction > 0;
			// 	velocity.X = Mathf.MoveToward(velocity.X, 0, Speed * (float)delta* 3.5f);
 
			if (velocity.Y < -100 && IsOnFloor() )
			{
				GD.Print(velocity.Y);
				GD.Print("Should've jumped");
				velocity.Y = JumpVelocity;
			}
			var velo = new Vector2(velocity.X, velocity.Y);
			if (velo.X > 0) {
				// Overwrite previous flip if moving ONLY
				PlayerSprite.FlipH = velo.X > 0;
			}
			// GD.Print($"Final velo: {velo} (vs)");
			Velocity = velo;
		}
		else {
			Velocity = new Vector2(0, velocity.Y);
		}
 
		MoveAndSlide();
	}
	
	//======= DEATH ========
	private void OnBobAroundForRevive(Node2D bob) {
		if (CurrentHp > 0)
			return;
		
		if ((this is SmallBob && bob is BigBob) || (this is BigBob && bob is SmallBob))
		{
			// if (ReviveTimer.IsStopped()) {
			// 	GD.Print("Started revive timer.");
			// 	ReviveTimer.Start();
			// }
		}
	}
	private void OnBobExitReviveArea(Node2D bob) {
		// if ((this is SmallBob && bob is BigBob) || (this is BigBob && bob is SmallBob))
		// {
		// 	GD.Print("Cancelled revive timer.");
		// 	ReviveTimer.Stop();
		// }
	}

	private void OnReviveTimerDone() {
		GD.Print("Revive timer done !");
		CurrentHp = 25;
	}


	
	//======== HEALTH =======
	public override async void TakeDamage(int damage)
	{
		if(HitDelayTimer.IsStopped())
		{
			base.TakeDamage(damage);
			if (CurrentHp <= 0) {
				Client.Instance.PlayerDied(this);
			}
			UpdateHpBar();
			Hud.Instance.HitEffect.Show();
			await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
			Hud.Instance.HitEffect.Hide();
		}
	}
	public void GainHealth(int health) {
		this.CurrentHp = Math.Min(CurrentHp + health, HPMax);
		UpdateHpBar();
	}
	private void UpdateHpBar()
	{
		if (HpBar != null)
		{
			HpBar.Value = CurrentHp;
		}
	}

	protected bool HasAuthority() {
		return Multiplayer.GetUniqueId() == MultiplayerSynchronizer.GetMultiplayerAuthority();
	}

	protected bool IsCurrentPlayer() {
		return this.Name == Client.Instance.CurrentPlayer.Name;
	}
	
	protected void ProcessCamera() {
		var v = GlobalPosition;
		v.X += CameraOffset.X;
		v.Y += CameraOffset.Y;
		CameraHistory.Enqueue(v);
		Vector2? cameraPos = null;
		while (CameraHistory.Count > CameraDelay)
		{
			cameraPos = CameraHistory.Dequeue().Round();
		}
		if (cameraPos != null) {
			CameraObject.GlobalPosition = cameraPos.Value;
		}
	}

	

	public void DisablePlayerOnThisClient() {
		Hide();
		CollisionShape.SetDisabled(true);
	}

	public void EnablePlayerOnThisClient() {
		Visible = true;
		CollisionShape.SetDisabled(false);
	}
	
	public void SetMultiplayerAuthority(int id) {
		MultiplayerSynchronizer.SetMultiplayerAuthority(id);
	}

	public void ShowSynchronizer() {
		MultiplayerSynchronizer.PublicVisibility = true;
	}

	public void HideSynchronizer() {
		MultiplayerSynchronizer.PublicVisibility = false;
	}
	
	public void UpdateNavAgent() {
		// GD.Print("Updating nav agent.");
		NavigationAgent.TargetPosition = Client.Instance.CurrentPlayer.GlobalPosition;
	}

	public static bool operator ==(PlayerBase playerA, PlayerBase playerB) {
		if (playerA is null || playerB is null)
			return false;
			// throw new ArgumentException("Comparing at least one null player.");
		
		return playerA.Name == playerB.Name;
	}
	public static bool operator !=(PlayerBase playerA, PlayerBase playerB) {
		return !(playerA == playerB);
	}
	
	private bool Equals(PlayerBase other) {
		return Equals(Name, other.Name);
	}

	public override bool Equals(object obj) {
		if (obj is null) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((PlayerBase)obj);
	}
	public override int GetHashCode() {
		var hashCode = new HashCode();
		hashCode.Add(Name);
		return hashCode.ToHashCode();
	}

	
	public override void UpdateAnimation() {
		AnimationType newAnimation;

		if (CurrentHp <= 0)
		{
			newAnimation = AnimationType.Death;
		}
		else if (AttackCondition())
		{
			newAnimation = AttackAnimation(); //handle in AttackHandler
		}
		else if (DashHandler.State != Dashing.Not) {
			newAnimation = AnimationType.Dash;
		}
		else if(OnWall)
		{
			newAnimation = AnimationType.Wall; 
		}
		else if (Velocity.X == 0 && Velocity.Y == 0)
		{
			newAnimation = AnimationType.Idle;
		}
		else if (Velocity.Y == 0) 
			newAnimation = AnimationType.Run;
		}
		else
		{
			newAnimation = AnimationType.Jump;
		}
			
		if (newAnimation == AnimationType.None && CurrentAnimation != AnimationType.None)
		{
			// GD.Print("Set animation to None");
			CurrentAnimation = AnimationType.None;
			PlayerSprite.Stop();
		}
		else if (newAnimation != CurrentAnimation && newAnimation != AnimationType.Anime)
		{
			//GD.Print($"Stopped animation {CurrentAnimation} & set it to {newAnimation}");
			PlayerSprite.Stop();
			CurrentAnimation = newAnimation;
			PlayerSprite.Play(CurrentAnimation.ToString().ToLower());
		}
	}

	protected abstract AnimationType AttackAnimation();
}
