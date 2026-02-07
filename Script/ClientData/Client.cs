using System;
using System.Collections.Generic;
using Beyondourborders.Script.Entities.Enemies;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Overlay;
using Beyondourborders.Script.Utils;
using Godot;
using Timer = Godot.Timer;

namespace Beyondourborders.Script.ClientData;

public partial class Client : Node2D
{ 
	public static Client Instance { get; private set; }

	public Client()
	{
		if (Instance != null)
			throw new Exception("Client already exists.");
		Instance = this;
		IsMultiplayer = false;
		IsHost = true;
	}
	
	private Timer _playerSwitchTimer;
	private Timer _zoneSwitchTimer;
	public override void _Ready() {
		_playerSwitchTimer = GetNode<Timer>("BobSwitchTimer");
		_zoneSwitchTimer = GetNode<Timer>("ZoneSwitchTimer");
	}
	
	public bool IsSaving { get; set; }
	public bool IsLoading { get; set; }

	public bool IsMultiplayer { get; set; }
	public bool IsSingleplayer => !IsMultiplayer;
	
	public bool IsHost { get; set; }
	public bool IsGuest => !IsHost;

	public PlayerBase? CurrentPlayer { get; private set; }
	public PlayerBase? OtherPlayer { get; private set; }
	
	public void LoadGameFromGameDataHost() {
		var gameData = GameData.CurrentGameData;
		if (gameData is null) {
			GD.PushError("CurrentGameData is null, can't init game.");
			return;
		}
		var checkpoint = CheckpointList.GetCheckpoint(gameData.LastCheckpoint!);
		if (checkpoint is null) {
			GD.PushError($"Checkpoint of name {gameData.LastCheckpoint} doesn't seem to exist, can't init game.");
			return;
		}
		// Loading order is complicated:
		// the Client needs the bobs which are in the main scene
		// but some components in the Hud need a Client with valid players (DebugComponent)
		// A fix for this is:
		// - first, load the scene (as in just open the scene file, Do NOT run it yet)
		// - from that scene get the bob nodes (which you can even if it hasnt been ran yet)
		// - after that set the players in the client
		// - only then you can add the scene to the root
		// This solves all issues, the client gets its player and the first time it runs the HUD gets a full client.
		SaveHelper.Instance.SaveCurrentGameDataToDiskSkipAllChecks();

		var startTime = Time.GetUnixTimeFromSystem();

		IsLoading = true;
		Hud.Instance.TransitionScreen.ShowScreen($"Loading {checkpoint.Level}...");
		
		// Doing it nested like that is a bit wonky, but it avoids lag on the main thread.
		new AsyncSceneLoader().Load(
			"res://Scene/Meta/Game.tscn", (scene, _) => {
				var big = scene.GetNode<BigBob>("BigBob");
				var small = scene.GetNode<SmallBob>("SmallBob");
				CurrentPlayer = big;
				OtherPlayer = small;
				
				IsMultiplayer = false;
				CallDeferred("RemoveSceneAddScene", "Menu", scene);
				new AsyncSceneLoader().LoadZone(checkpoint.Level, (terrain, _) => {
						WaitUtils.WaitUntil(
							() => startTime + 1 > Time.GetUnixTimeFromSystem(),
							() => {
								CallDeferred("AddTerrainToScene", scene, terrain);
								IsLoading = false;
								CallUtils.CallDeferred(() => {
									big.GlobalPosition = checkpoint.BigBobSpawn;
									small.GlobalPosition = checkpoint.SmallBobSpawn;
									
									GD.Print("AAAA DOING RN!!!");
									GD.Print(checkpoint.Level);
									big.UglyLevelRawDoNotUseCMoiPtnCLaMerde = checkpoint.Level;
									small.UglyLevelRawDoNotUseCMoiPtnCLaMerde = checkpoint.Level;
								

									Hud.Instance.HpBar.Show();
									Hud.Instance.TransitionScreen.RemoveScreen();
								});
							});
					});
			});
	}
	
	public void LoadGameFromGameDataGuest() {
		Hud.Instance.TransitionScreen.ShowScreen($"Waiting for sync...");
		WaitUtils.WaitUntil(
			() => GameData.CurrentGameData.LastCheckpoint is null,
			LoadGameFromGameDataGuestInner
		);
	}
	public void LoadGameFromGameDataGuestInner() {
		var gameData = GameData.CurrentGameData;
		var checkpoint = CheckpointList.GetCheckpoint(gameData.LastCheckpoint!)!;
		
		var startTime = Time.GetUnixTimeFromSystem();

		IsLoading = true;
		Hud.Instance.TransitionScreen.CallDeferred("SetMessage", $"Loading {checkpoint.Level}...");
		
		// Doing it nested like that is a bit wonky, but it avoids lag on the main thread.
		new AsyncSceneLoader().Load(
			"res://Scene/Meta/Game.tscn", (scene, _) => {
				var big = scene.GetNode<BigBob>("BigBob");
				var small = scene.GetNode<SmallBob>("SmallBob");
				CurrentPlayer = small;
				OtherPlayer = big;
				
				CallDeferred("RemoveSceneAddScene", "Menu", scene);
				new AsyncSceneLoader().LoadZone(checkpoint.Level, (terrain, _) => {
						WaitUtils.WaitUntil(
							() => startTime + 1 > Time.GetUnixTimeFromSystem(),
							() => {
								CallDeferred("AddTerrainToScene", scene, terrain);
								IsLoading = false;
								Hud.Instance.TransitionScreen.CallDeferred("RemoveScreen");
								Hud.Instance.HpBar.Show();
								CallUtils.CallDeferred(() => Rpc("GuestDoneLoading"));
								CallUtils.CallDeferred(() => Rpc("NotifyMobSynchronizersOnceGuestDoneLoading"));
							});
					});
			});	
	}

	[Rpc]
	private void FromHostSetSmallBobCoordinates(Vector2 v) {
		WaitUtils.WaitUntil(
			() => Instance.CurrentPlayer is null,
			CallUtils.CallDeferredA(() => CurrentPlayer!.GlobalPosition = v)
			);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void GuestDoneLoading() {
		Hud.Instance.TransitionScreen.RemoveScreen();
		Instance.CurrentPlayer!.ShowSynchronizer();
		Instance.OtherPlayer!.ShowSynchronizer();
	}
	
	private void RemoveSceneAddScene(string toRemove, Node2D scene) {
		GD.Print(toRemove);
		GD.Print(scene.Name);
		RemoveChild(GetNode(toRemove));
		AddChild(scene);
	}
	private void AddTerrainToScene(Node2D scene, Node2D terrain) {
		terrain.Name = "terrain";
		scene.AddChild(terrain);
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void NotifyMobSynchronizersOnceGuestDoneLoading() {
		GD.Print("Waiting for GhettoMobSynchronizerManager instance");
		WaitUtils.WaitUntil(
			() => GhettoMobSynchronizerManager.Instance is null,
			CallUtils.CallDeferredA(() => GhettoMobSynchronizerManager.Instance.RightAfterPlayerDoneChangingZone(Instance.CurrentPlayer))
		);
	}
	
	public void LeaveGameToMenu() {
		AllMobs.Clear();
		if (IsHost) {
			SaveHelper.Instance.SaveCurrentGameDataToDiskSkipAllChecks();
		}
		var startTime = Time.GetUnixTimeFromSystem();
		Hud.Instance.TransitionScreen.ShowScreen($"Back to menu...");
		
		new AsyncSceneLoader().Load(
			"res://Scene/Menu/accueil.tscn", (scene, _) => {
				WaitUtils.WaitUntil(
					() => startTime + 1 > Time.GetUnixTimeFromSystem(),
					() => {
						CallDeferred("RemoveSceneAddScene", "Game", scene);
						Hud.Instance.TransitionScreen.CallDeferred("RemoveScreen");
						Hud.Instance.HpBar.Hide();
						CurrentPlayer = null;
						OtherPlayer = null;
					});
			});
	}
	
	
	public void SwitchPlayers(bool force = false) {
		if (Instance.IsMultiplayer)
			throw new Exception("Tried to switch players in multiplayer !");
		// If no delay:
		// - Players can abuse this
		// - Mainly, since one bob gets called after the other, the "IsActionJustPressed" is 
		//   triggered on both at the same frame, making it switch back instantly
		if (!force && _playerSwitchTimer.TimeLeft > 0)
			return;
		
		_playerSwitchTimer.Start();

		(CurrentPlayer, OtherPlayer) = (OtherPlayer, CurrentPlayer);

		CurrentPlayer.CameraObject.MakeCurrent();
		
		CurrentPlayer.UpdateAnimation();
		OtherPlayer.UpdateAnimation();
	}
	
	// === Functions to be called with deferred as cant be updated in a physics callback ===
	// Called when the not main player enters a zone in multiplayer
	private void SwitchTerrain(Node2D terrain) {
		var g = GetNode("Game");
		g.RemoveChild(g.GetNode("terrain"));
		g.AddChild(terrain);
	}

	// private void _lastLoadZone
	private void HandlePlayerHidingMultiplayer(string zoneName) {
		if (OtherPlayer.Zone == zoneName)
			OtherPlayer.EnablePlayerOnThisClient();
		else
			OtherPlayer.DisablePlayerOnThisClient();
	}
	// === End deferred methods ===
	
	private void OnDoneZoneLoading(Node2D terrain, object[]? data = null) {
		GD.Print("Done loading zone, now waiting for loading screen to finish.");
		WaitUtils.WaitUntil(
			() => !Hud.Instance.TransitionScreen.IsDoneTransitioning,
			() => CallDeferred("OnDoneZoneLoadingAndBlackScreen", terrain, (Vector2)data![0], (Vector2)data![1])
			);
	}

	private void OnDoneZoneLoadingAndBlackScreen(Node2D terrain, Vector2 positionInNewZoneMain,
		Vector2 positionInNewZoneOther) {
		
		GD.Print("Done loading zone and waiting for loading screen.");

		terrain.Name = "terrain";
		SwitchTerrain(terrain);
		Instance.CurrentPlayer.GlobalPosition = positionInNewZoneMain;
		if (!Instance.IsMultiplayer) {
			Instance.OtherPlayer.GlobalPosition = positionInNewZoneOther;
			Instance.OtherPlayer.UglyLevelRawDoNotUseCMoiPtnCLaMerde = Instance.CurrentPlayer.Zone;
		}

		GhettoMobSynchronizerManager.Instance.Rpc("RightAfterPlayerDoneChangingZoneRpc", Instance.CurrentPlayer);
		
		Hud.Instance.TransitionScreen.RemoveScreen();
		IsLoading = false;
		// TODO: Make camera move instantly if possible.
	}

	public void LoadZone(string zoneName, Vector2 positionInNewZoneMain, Vector2 positionInNewZoneOther) {
		if (_zoneSwitchTimer.TimeLeft > 0)
			return;
		
		_zoneSwitchTimer.Start();
		
		Instance.CurrentPlayer.UglyLevelRawDoNotUseCMoiPtnCLaMerde = zoneName;
		AllMobs.Clear();
		GD.Print($"Loading zone {zoneName} (Client: {Instance.CurrentPlayer.Name})");
		Hud.Instance.TransitionScreen.ShowScreen($"Loading {zoneName}");
		IsLoading = true;
		new AsyncSceneLoader().LoadZone(zoneName, OnDoneZoneLoading, new object[]{positionInNewZoneMain, positionInNewZoneOther});
	}

	public void PlayerDied(PlayerBase p) {
		Hud.Instance.TransitionScreen.ShowScreen($"Died, back to checkpoint...");
		IsLoading = true;
		var checkpoint = CheckpointList.GetCheckpoint(GameData.CurrentGameData.LastCheckpoint!)!;
		new AsyncSceneLoader().LoadZone(checkpoint.Level, (terrain, _) => {
			WaitUtils.WaitUntil(
				() => !Hud.Instance.TransitionScreen.IsDoneTransitioning,
				() => {
					p.Zone = checkpoint.Level;
					p.UglyLevelRawDoNotUseCMoiPtnCLaMerde = checkpoint.Level;
					CallUtils.CallDeferred(() => {
						var gameNode = GetNode<Node2D>("Game");
						gameNode.RemoveChild(gameNode.GetNode("terrain"));
						AddTerrainToScene(gameNode, terrain);
					});
					WaitUtils.WaitFor(2, () => {
						CallUtils.CallDeferred(() => {
							p.CurrentHp = p.HPMax;
							p.HpBar.Value = p.CurrentHp;
							p.GlobalPosition = CurrentPlayer is BigBob ? checkpoint.BigBobSpawn : checkpoint.SmallBobSpawn;
							IsLoading = false;
							Hud.Instance.TransitionScreen.RemoveScreen();
						});
					});
				});
		});
	}
	
	public float AggroLevel {
		get {
			var playerPos = CurrentPlayer is BigBob ? CurrentPlayer.GlobalPosition : OtherPlayer.GlobalPosition;
			float min = float.MaxValue;
            
			foreach (var mob in AllMobs) {
				if (CurrentPlayer is BigBob && mob.BigBobOnDetectionArea)
					return 100;
				if (CurrentPlayer is SmallBob && mob.SmallBobOnDetectionArea)
					return 100;
 
				if (Math.Abs(mob.GlobalPosition.Y - playerPos.Y) > 300) {
					continue;
				}
                
				// Backup:
				var diff = playerPos.DistanceTo(mob.GlobalPosition);
				//var diff = Math.Abs(mob.GlobalPosition.X - playerPos.X);
                
				if (diff < min)
					min = diff;
			}
        
			var ghettoManaged = 1 / min * 33000;
            
			// GD.Print($"mobCount: {AllMobs.Count}, ghettoManaged: {ghettoManaged}, min: {min}");
			return Math.Min(100, ghettoManaged);
		}
	}
	public List<EnemyBase> AllMobs = new ();
}
