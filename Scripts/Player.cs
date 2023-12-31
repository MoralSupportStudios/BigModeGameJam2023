using Godot;
using System;

public partial class Player : Area2D
{

	[Signal]
	public delegate void HitEventHandler();


	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).
	public bool IsAlive { get; set; } = false;

	//public Vector2 ScreenSize; // Size of the game window.
	public override void _Ready()
	{
	   // ScreenSize = GetViewportRect().Size;
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsInstanceValid(this))
			return;

		Vector2 velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("ui_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("ui_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("ui_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("ui_up"))
		{
			velocity.Y -= 1;
		}

		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}
		Position += velocity * (float)delta;
		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;
			// See the note below about boolean assignment.
			animatedSprite2D.FlipH = velocity.X < 0;
		}
	}
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		IsAlive = true;
	}

	public void Die()
	{
		IsAlive = false;
		EmitSignal(SignalName.Hit);
		Hide();
	}
}
