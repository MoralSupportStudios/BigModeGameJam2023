using Godot;
using System;

public partial class Gun : Node2D
{
	[Export] PackedScene bulletScene;
	[Export] float bulletSpeed = 600f;
	[Export] float bulletPerSecond = 5f;
	[Export] float bulletDamage = 1f;

	float fireRate;
	float timeUntilFire = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fireRate = 1f / bulletPerSecond;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("click") && timeUntilFire > fireRate)
		{
			RigidBody2D bullet = (RigidBody2D)bulletScene.Instantiate<RigidBody2D>();
            
			bullet.Rotation = GlobalRotation;
			bullet.GlobalPosition = GlobalPosition;
			bullet.LinearVelocity = bullet.Transform.X * bulletSpeed;
            
			GetTree().Root.AddChild(bullet);

			timeUntilFire = 0f;
			//GD.Print("Fire");
        }
		else
		{
            timeUntilFire += (float)delta;
        }
	}
}
