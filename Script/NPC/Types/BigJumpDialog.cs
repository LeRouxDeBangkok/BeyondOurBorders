using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Entities.Players.Properties;
using Godot;

namespace Beyondourborders.Script.NPC.Types;

public partial class BigJumpDialog : NpcDialog
{
    // REPLACED
    public void GiveAbility()
    {
        GD.Print("GiveAbility");
        GameData.CurrentGameData.Abilities += (UnlockableAbility.BigJump.ToString());
    }
}