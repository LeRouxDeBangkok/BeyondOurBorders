using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Entities.Players.Properties;
using Godot;

namespace Beyondourborders.Script.NPC.Types;

public partial class DashDialog : NpcDialog {
    // REPLACED
    public void GiveAbility()
    {
        GD.Print("GiveAbility");
        GameData.CurrentGameData.Abilities += (UnlockableAbility.Dash.ToString());
    }
}