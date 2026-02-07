using Beyondourborders.Script.Entities.Enemies;
using Beyondourborders.Script.Weapons;
using WeaponBase = Beyondourborders.Script.Weapons.WeaponBase;

using Godot;


public partial class Goblin : EnemyBase
{

	public Goblin() : base(speed: 0f, jumpVelocity: 0f, hp:50)
	{
	}
	public override void _Ready()
	{
		base._Ready();
		Weapon = GetNode<WeaponBase>("Weapon");
	}
	

	public override bool AttackCondition()
	{
		return (SmallBobOnDetectionArea || BigBobOnDetectionArea) && AttackTimer.WaitTime - AttackTimer.TimeLeft <= 2;
	}

	public override void _PhysicsProcess(double delta)
	{
		//bomb 
		if(CurrentHp > 0)
		{
			if (!AttackTimer.IsStopped() && AttackTimer.TimeLeft <= 3.05f && AttackTimer.TimeLeft >= 3f)
			{
				Weapon.Visibility = true;
				Weapon.Show();
				Weapon.Start();
			}
			else if (AttackTimer.IsStopped() || AttackTimer.TimeLeft > 3f)
			{
				Weapon.Visibility = false;
				Weapon.Hide();
			}

			//timer
			if (SmallBobOnDetectionArea || BigBobOnDetectionArea)
			{
				if (AttackTimer.IsStopped())
				{
					AttackTimer.Start();
				}
			}
			else
			{
				AttackTimer.Stop();
			}

			// ===== Physique ===== 

			Vector2 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				velocity += GetGravity() * (float)delta;
			}

			Velocity = velocity;

			MoveAndSlide();

			UpdateAnimation();
		}
		else
		{
			Weapon.Visibility = false;
			Weapon.Hide();
		}
	}
	
}
