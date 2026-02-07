using Beyondourborders.Script.Entities.Players;

namespace Beyondourborders.Script.Item;

using Godot;
using System;

[GlobalClass]

public partial class BatWing : global::Item
{
	[Export]
	public int HealAmount { get; set; } = -1;
	
	public override string Use(Node2D user)
	{
		return $"{Name} used: You lost {HealAmount} HP, what did you expect?";
	}
}
