using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Overlay;
using Godot;

namespace Beyondourborders.Script.Entities.Players.Properties;

public class PhyisqueHandler
{
	
	public bool CanDoubleJump = false;
	public bool IsWallJumping = false;
	protected bool IsWallSliding = false;
	protected PlayerBase Body;
	protected bool NoDirection = false;
	protected float StartTime = 0;
	protected bool GoLeft = false;
	private double _duration = 250;

	
	public PhyisqueHandler(PlayerBase body)
	{
		Body = body;
	}

    protected Vector2 AddGravity(double delta, Vector2 velocity,float propotion)
	{
		velocity += Body.GetGravity() * (float)delta * propotion;
		return velocity;
	}
	protected void HandleAttack()
	{
		if (GameData.CurrentGameData.Weapon != 0)
		{
			Body.AttackTimer.Start();
			if (Body is BigBob b)
			{
				if (b.PlayerSprite.FlipH)
				{
					if (GameData.CurrentGameData.Weapon == 1)
					{
						b.HitZone.Damage = b.Damage;
						b.Anime.Play("attack1-Flip");
					}
					else if (GameData.CurrentGameData.Weapon == 2)
					{
						b.HitZone.Damage = b.Damage;
						b.Anime.Play("attack2-Flip");
					}
				}
				else
				{
					if (GameData.CurrentGameData.Weapon == 1)
					{
						b.HitZone.Damage = b.Damage;
						b.Anime.Play("attack1");
					}
					else if (GameData.CurrentGameData.Weapon == 2)
					{
						b.HitZone.Damage = b.Damage;
						b.Anime.Play("attack2");
					}
				}
			}
		}
	}
	protected void HandleDash()
	{
		var (canDash, dashCheckTime) = Body.DashHandler.CanDash(Body.IsOnFloor());
		if (canDash) Body.DashHandler.PerformDash(dashCheckTime, Body.PlayerSprite.FlipH);
	}
	public Vector2 HandleDashPhysic() 
	{
		return Body.DashHandler.GetDashVelocityVector();
	}
	protected Vector2 HandleJump(double delta, Vector2 velocity)
	{
		if (Body.OnWall && GameData.CurrentGameData.Abilities.Contains(UnlockableAbility.DoubleJump.ToString()))
		{
			IsWallJumping = true;
			velocity.Y = Body.JumpVelocity * 2/3; 
			if (Body.PlayerSprite.FlipH)
			{
				velocity.X = HandleWalk(delta, velocity, -2).X;
			}
			else
			{
				velocity.X = HandleWalk(delta, velocity, 2).X;
			}
			StartTime = Time.GetTicksMsec();
			NoDirection = true;
			_duration = 250;
		}
		else if (Body.IsOnFloor())
		{
			velocity.Y = Body.JumpVelocity;
		}
		else if (CanDoubleJump && GameData.CurrentGameData.Abilities.Contains(UnlockableAbility.DoubleJump.ToString()))
		{
			CanDoubleJump = false;
			velocity.Y = Body.JumpVelocity;
		}
		return velocity;
	}
	protected Vector2 HandleWalk(double delta, Vector2 velocity, int direction)
	{
		if(direction != 0)
		{
			velocity.X = direction * Body.Speed;
			Body.PlayerSprite.FlipH = direction > 0;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Body.Speed * (float)delta* 3.5f);
			
		}
		return velocity;
	}

	public Vector2 HandleBigJump(Vector2 velocity)
	{
		if (Body.IsOnFloor())
		{
			velocity.Y = Body.JumpVelocity * 150;
			NoDirection = true;
			StartTime = Time.GetTicksMsec();
			_duration = 500;
		}

		return velocity;
	}
	protected Vector2 AddOnWallGravity(double delta, Vector2 velocity)
	{
		IsWallJumping = false;
		if (!IsWallSliding && !IsWallJumping)
		{
			velocity.Y = 0;
			IsWallSliding = true;
		}
		else if (IsWallSliding && !IsWallJumping)
		{
			velocity.Y = AddGravity( delta, velocity,0.2f).Y;
		}

		return velocity;
	}
	public void HandleNormalPhysic(double delta)
	{
		if (Body.IsOnFloor())
		{
			IsWallJumping = false;
			IsWallSliding = false;
			Body.DashHandler.ResetAirStatus();
			CanDoubleJump = true;
		}
		
		Vector2 velocity = Body.Velocity;
		if (NoDirection && Time.GetTicksMsec() - StartTime >= _duration)
		{
			NoDirection = false;
		}
		else if(NoDirection)
		{
			velocity.Y = Body.JumpVelocity * 2/3;
		}
		if (!Body.IsOnFloor() && (!GameData.CurrentGameData.Abilities.Contains(UnlockableAbility.DoubleJump.ToString()) || !Body.OnWall))
		{
			velocity = AddGravity(delta, velocity,3.5f);
		}
		if (Body.OnWall && GameData.CurrentGameData.Abilities.Contains(UnlockableAbility.DoubleJump.ToString()))
		{
			// If is going up, stop that when touching wall.
			if (velocity.Y < 0)
				velocity.Y = 0;
			velocity = AddOnWallGravity(delta, velocity);
		}

		velocity = InputHandler(delta, velocity);
		Body.Velocity = velocity;
	}

	private Vector2 InputHandler(double delta, Vector2 velocity)
	{

		if(!NoDirection)
		{
			if (Input.IsActionPressed("Left"))
			{
				velocity = HandleWalk(delta, velocity, -1);
			}
			else if (Input.IsActionPressed("Right"))
			{
				velocity = HandleWalk(delta, velocity, 1);
			}
			else
			{
				velocity = HandleWalk(delta, velocity, 0);
			}
		}
		else
		{
			velocity = HandleWalk(delta, velocity, 0);
		}
		if (Input.IsActionJustPressed("Jump"))
		{
			velocity = HandleJump(delta, velocity);
		}

		if (Input.IsActionJustPressed("BigJump") && GameData.CurrentGameData.Abilities.Contains(UnlockableAbility.BigJump.ToString()))
		{
			velocity = HandleBigJump(velocity);
		}
		if (Input.IsActionJustPressed("Dash") && GameData.CurrentGameData.Abilities.Contains(UnlockableAbility.Dash.ToString()))
		{
			HandleDash();
		}
		if (Input.IsActionJustPressed("Attack") && Body.AttackTimer.IsStopped())
		{
			HandleAttack();
		}

		return velocity;
	}
}