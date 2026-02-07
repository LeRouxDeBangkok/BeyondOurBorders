using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Godot;

namespace Beyondourborders.Script.NPC.Types;

public partial class SkeletonLevel1Dialog : NpcDialog
{
	// REPLACED
	public void GiveSword()
	{
		GameData.CurrentGameData.Weapon = 1;
	}
}
