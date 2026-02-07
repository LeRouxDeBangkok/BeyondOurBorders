using Beyondourborders.Script.Entities.Players;

namespace Beyondourborders.Script.Item;

using Godot;
using System;

[GlobalClass]
public partial class JambeDeGobelin : global::Item
{
	[Export]
	public int IncreaseSpeedAmout { get; set; } = 10;
	public override string Use(Node2D user)
	{
		return $"{Name} used: Speed increased by {IncreaseSpeedAmout}.";
	}
	
}
