using Beyondourborders.Script.Triggers;
using Godot;

namespace Beyondourborders.Script.Weapons;

public partial class Bomb : WeaponBase
{
	[Export] private float _speed = 300f;
	[Export] private float _direction = 1; //right = 1 left = -1
	private HitArea _hitzone;
	private Timer _timer;

	public Bomb() {}
	public override void _Ready()
	{
		base._Ready();
		Sprite.Animation = "default";
		_hitzone = GetNode<HitArea>("HitZone");
		_timer = GetNode<Timer>("Timer");
		_timer.OneShot = true;
	}

	public override void _PhysicsProcess(double delta) 
	{
		if (_timer.TimeLeft < 1 && _timer.TimeLeft != 0)
		{
			_hitzone.Show();
		}
		else
		{
			_hitzone.Hide();
		}
		Vector2 velocity = Velocity;
		if (_timer.TimeLeft > 2.75f)
		{
			velocity.Y = -400;
			velocity.X = _direction * _speed;
		}
		else if (!IsOnFloor())
		{
			velocity.Y += GetGravity().Y * (float)delta * 3.5f;
			velocity.X = _direction * _speed;
		}
		else
		{
			velocity.X = 0f;
		}
		Velocity = velocity;
		
		MoveAndSlide();
	}

	public override void Start()
	{
		Visibility = true;
		Position = new Vector2(44,6);
		Sprite.Stop();
		Sprite.Play();
		_timer.Start();
	}
	
}
