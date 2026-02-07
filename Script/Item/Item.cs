using Godot;
using System;


public partial class Item : Resource
{
	protected float StartTime = 0;
	protected bool HaveStarted = false;
	
	
	[Export]
	public int ID {get; set;}
	[Export]
	public string Name {get; set;}
	[Export]
	public string ResourcePath {get; set;}
	[Export]
	public Texture2D Icon {get; set;}
	[Export]
	public int Quantity {get; set;}
	[Export]
	public int StackSize {get; set;}
	[Export]
	public bool IsStackable {get; set;}

	public string Chat ;
	
	public virtual string Use(Node2D user)
	{
		return $"{Name} used. No effect defined.";
	}
}
