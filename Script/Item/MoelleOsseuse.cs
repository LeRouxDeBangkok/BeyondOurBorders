using Beyondourborders.Script.Entities.Players;

namespace Beyondourborders.Script.Item;

using Godot;
using System;

[GlobalClass]
public partial class MoelleOsseuse : global::Item
{
	[Export]
	public int AttackAmount { get; set; } = 10;
	
	public override string Use(Node2D user)
	{
		return $"{Name} used: Attack increased by {AttackAmount}";
	}
	
}
