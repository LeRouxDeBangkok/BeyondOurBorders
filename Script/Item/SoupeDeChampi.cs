using Beyondourborders.Script.Entities.Players;

namespace Beyondourborders.Script.Item;

using Godot;
using System;

[GlobalClass]

public partial class SoupeDeChampi : global::Item
{
    [Export]
    public int HealAmount { get; set; } = 10;
    
    public override string Use(Node2D user)
    {
        return $"{Name} used: Recovered {HealAmount} HP";
    }
}