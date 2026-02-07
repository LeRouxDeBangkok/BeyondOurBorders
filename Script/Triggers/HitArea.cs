using System.Collections.Generic;
using Beyondourborders.Script.Entities;
using Beyondourborders.Script.Entities.Enemies;
using Beyondourborders.Script.Entities.Players;
using Godot;

namespace Beyondourborders.Script.Triggers;

public partial class HitArea : Area2D
{
	[Export] public int Damage = 10;
	[Export] public bool DoDamageToEnemies = true;
	[Export] public bool DoDamageToPlayers = true;
	[Export] public bool Active = true;

	public bool EnemyTouch
	{
		get
		{
			foreach (var body in GetOverlappingBodies()) //pour tout les body dans l'area
			{
				if ( body is EnemyBase e)
				{
					return true;
				}
			}

			return false;
		}
	}

	public override void _Process(double delta)
	{
		if (Active)
		{
			foreach (var body in GetOverlappingBodies()) //pour tout les body dans l'area
			{
				if (DoDamageToEnemies && body is EnemyBase e)
				{
					e.TakeDamage(Damage);
				}

				if (DoDamageToPlayers && body is PlayerBase p)
				{
					p.TakeDamage(Damage);
				}
			}
		}
	}
}
