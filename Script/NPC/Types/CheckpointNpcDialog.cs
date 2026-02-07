using Beyondourborders.Script.ClientData;
using Godot;

namespace Beyondourborders.Script.NPC.Types;

public partial class CheckpointNpcDialog : NpcDialog {
    // REPLACED
    
    public CheckpointNpc CheckpointNpc = null!;
    public override void _Ready() {
        base._Ready();
        CheckpointNpc = GetParent<CheckpointNpc>();
    }

    public bool SaveGame() {
        GD.Print("SaveGame called.");
        if (Client.Instance.IsGuest) {
            SetCurrentLineFully("[center]Only the host can save.");
            return true;
        }

        GameData.CurrentGameData!.LastCheckpoint = CheckpointNpc.CheckpointName;
        SaveHelper.Instance.Save();

        return false;
    }
}