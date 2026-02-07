using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Godot;

namespace Beyondourborders.Script.NPC.Types;

public partial class GroupNPCDialogue : NpcDialog
{
	// REPLACED
	public void GiveSword()
	{
		GameData.CurrentGameData.Weapon = 2;
	}
}
