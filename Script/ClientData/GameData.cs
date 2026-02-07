using System;
using System.Collections.Generic;
using Beyondourborders.Script.Entities.Players.Properties;
using Godot;

namespace Beyondourborders.Script.ClientData;

// How this class works:
// Godot has a single object (CurrentGameData)
// which is used to save/load the game.
// If you want to male it be a new one, use SetCurrentGameData
// which copies all of the fields without re making one
// as we NEED the instance to be Godot's one.
public partial class GameData : Node2D {
    public static GameData CurrentGameData { get; private set; } = null!;
    
    // public List<UnlockableAbility> Abilities = new List<UnlockableAbility>();
    
    // to be called ONLY BY GODOT !
    private GameData() {
        CurrentGameData = this;
    }
    public GameData(
        int saveNum, double playTime, string lastCheckpoint,
        bool isLevel2DoorOpen, int weapon, string abilities, bool isLevel4DoorOpen) {
        this.SaveNum = saveNum;
        this.PlayTime = playTime;
        this.LastCheckpoint = lastCheckpoint;

        if (isLevel2DoorOpen) {
            this.IsLevel2DoorOpen = true;
            this.IsLevel2FirstLeverTriggered = true;
            this.IsLevel2SecondLeverTriggered = true;
        } else {
            this.IsLevel2DoorOpen = false;
            this.IsLevel2FirstLeverTriggered = false;
            this.IsLevel2SecondLeverTriggered = false;   
        }

        if (isLevel4DoorOpen) {
            this.IsLevel4DoorOpen = true;
            this.IsLevel4FirstLeverTriggered = true;
            this.IsLevel4SecondLeverTriggered = true;
        }
        else {
            this.IsLevel4DoorOpen = false;
            this.IsLevel4FirstLeverTriggered = false;
            this.IsLevel4SecondLeverTriggered = false;
        }

        this.Weapon = weapon;
        this.Abilities = abilities;
    
        SessionStart = -1;
    }
    
    public static void SetCurrentGameData(GameData data) {
        CurrentGameData.SaveNum = data.SaveNum;
        CurrentGameData.PlayTime = data.PlayTime;

        CurrentGameData.LastCheckpoint = data.LastCheckpoint;
        CurrentGameData.SessionStart = data.SessionStart;

        CurrentGameData.IsLevel2FirstLeverTriggered = data.IsLevel2FirstLeverTriggered;
        CurrentGameData.IsLevel2SecondLeverTriggered = data.IsLevel2SecondLeverTriggered;
        CurrentGameData.IsLevel2DoorOpen = data.IsLevel2DoorOpen;
        
        CurrentGameData.IsLevel4FirstLeverTriggered = data.IsLevel4FirstLeverTriggered;
        CurrentGameData.IsLevel4SecondLeverTriggered = data.IsLevel4SecondLeverTriggered;
        CurrentGameData.IsLevel4DoorOpen = data.IsLevel4DoorOpen;

        CurrentGameData.Weapon = data.Weapon;
        CurrentGameData.Abilities = data.Abilities;
        
        GD.Print($"SaveNum: {CurrentGameData.SaveNum}, PlayTime: {CurrentGameData.PlayTime}, " +
                          $"LastCheckpoint: {CurrentGameData.LastCheckpoint}, SessionStart: {CurrentGameData.SessionStart}, " +
                          $"IsLevel2FirstLeverTriggered: {CurrentGameData.IsLevel2FirstLeverTriggered}, " +
                          $"IsLevel2SecondLeverTriggered: {CurrentGameData.IsLevel2SecondLeverTriggered}, " +
                          $"IsLevel2DoorOpen: {CurrentGameData.IsLevel2DoorOpen}, " +
                          $"Weapon: {CurrentGameData.Weapon}, Abilities: '{CurrentGameData.Abilities}'");
    } 

    [Export] public int SaveNum;
    [Export] public double PlayTime;

    [Export] public string? LastCheckpoint;

    [Export] public double SessionStart;
    
    [Export] public bool IsLevel2FirstLeverTriggered;
    [Export] public bool IsLevel2SecondLeverTriggered;
    [Export] public bool IsLevel2DoorOpen;
    
    [Export] public int Weapon;
    [Export] public string Abilities;
    
    [Export] public bool IsLevel4FirstLeverTriggered;
    [Export] public bool IsLevel4SecondLeverTriggered;
    [Export] public bool IsLevel4DoorOpen;
    [Export] public bool IsLevel3CourseComplete = false;
    
    public void UpdatePlaytime() {
        if (Math.Abs(SessionStart + 1) < 0.1) // == -1 but w float tolerance.
            SessionStart = Time.GetUnixTimeFromSystem();
        else {
            PlayTime += Time.GetUnixTimeFromSystem() - SessionStart;
            SessionStart = Time.GetUnixTimeFromSystem();
        }
    }
}