using Godot;
using System;

public partial class Boomerang : Area2D
{
	[Export] public float damage = 2f;
	[Export] public float orbitSpeed = 300f; // Speed at which the boomerang orbits
	[Export] public float returnSpeed = 200f; // Speed at which the boomerang returns
	[Export] public float orbitRadius = 100f; // Radius of the orbit

	public Vector2 orbitCenter;
	private float orbitAngle = 0f;
	private const float THREE_QUARTERS_TAU = Mathf.Pi * 1.5f; // Represents three-quarters of a full rotation
	private bool isReturning = false;

	public override void _Ready()
	{
		//orbitCenter = GetTree().Root.GetNode("Main").GetNode("Player").GetNode<Node2D>("Gun").GlobalPosition; // Set the orbit center to the gun's position
	}

	public override void _Process(double delta)
	{
		if (!isReturning)
		{
			orbitAngle += orbitSpeed * (float)delta / orbitRadius;

			Vector2 offset = new Vector2(Mathf.Sin(orbitAngle), Mathf.Cos(orbitAngle)) * orbitRadius;
			GlobalPosition = orbitCenter + offset;

			if (orbitAngle >= THREE_QUARTERS_TAU)
			{
				isReturning = true; // Signal that the Boomerang should start returning
			}
		}
		else
		{
			// Return to the gun's position
			Vector2 directionToGun = (orbitCenter - GlobalPosition).Normalized();
			GlobalPosition += directionToGun * returnSpeed * (float)delta;

			// Check if the Boomerang has reached the Gun's position (within a small threshold)
			if (GlobalPosition.DistanceTo(orbitCenter) < returnSpeed * (float)delta)
			{
				QueueFree(); // Remove the Boomerang from the scene once it has returned
			}
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("enemy"))
		{
			Enemy enemy = (Enemy)body;
			enemy.GetNode<Health>("Health").Damage(damage);
		}
	}
}
