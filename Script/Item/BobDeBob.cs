using Beyondourborders.Script.Entities.Players;

namespace Beyondourborders.Script.Item;

using Godot;
using System;

[GlobalClass]
public partial class BobDeBob : global::Item
{ 
    public string Use(Node user)
    {
        Chat = $"";
        return Chat;
    }
}