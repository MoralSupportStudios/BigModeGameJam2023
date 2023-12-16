using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
	[Export]
	public PackedScene[] PickupScenes; // Holds the different pickup scenes

	//[Export] PackedScene pickupScene;
	Player player;
	[Export] public float Speed { get; set; } = 200f;
	[Export] float damage = 1f;
	[Export] float attacksPerSecond = 2f;

	float attackSpeed;
	float timeUntilAttack;
	bool withinAttackRange = false;

    // Define the boundaries for the enemy
    private Vector2 _boundaryTopLeft = new Vector2(-2048, -2048);
    private Vector2 _boundaryBottomRight = new Vector2(2048, 2048);


    public override void _Ready()
	{
		player = (Player)GetTree().Root.GetNode("Main").GetNode("Player");

		attackSpeed = 1f / attacksPerSecond;
		timeUntilAttack = attackSpeed;
	}

	public override void _Process(double delta)
	{
		if(withinAttackRange && timeUntilAttack <= 0)
		{
			Attack();
			timeUntilAttack = attackSpeed;
		}
		else
		{
			timeUntilAttack -= (float)delta;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player != null)
		{
			AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

			//LookAt(player.GlobalPosition);
			Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
			animatedSprite2D.Play();

		}
		else
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
	}

	public void Attack()
	{
		player.GetNode<Health>("Health").Damage(damage);
	}

	public void OnAttackRangeBodyEnter(Area2D body)
	{
		if(body.IsInGroup("player"))
		{
			withinAttackRange = true;
		}
	}

	public void OnAttackRangeBodyExit(Area2D body)
	{
		if(body.IsInGroup("player"))
		{
			withinAttackRange = false;
			timeUntilAttack = attackSpeed;
		}
	}
    public void Die()
    {
        if (IsWithinBounds(Position))
        {
            // Calculate which pickup to drop, if any
            PickupType selectedPickup = SelectRandomWeighted();

            // If 'Nothing' is selected, we don't drop anything
            if (selectedPickup != PickupType.Nothing)
            {
                // Instantiate the selected pickup
                PackedScene pickupScene = PickupScenes[(int)selectedPickup];
                if (pickupScene == null)
                {
                    GD.Print("Selected pickup scene is null.");
                    return;
                }
                Node2D pickupInstance = (Node2D)pickupScene.Instantiate();
                pickupInstance.Position = Position;
                GetParent().CallDeferred("add_child", pickupInstance);
            }
        }

        QueueFree();
    }
    private bool IsWithinBounds(Vector2 position)
    {
        return position.X >= _boundaryTopLeft.X && position.X <= _boundaryBottomRight.X &&
               position.Y >= _boundaryTopLeft.Y && position.Y <= _boundaryBottomRight.Y;
    }
    private PickupType SelectRandomWeighted()
	{
		var weights = new Dictionary<PickupType, int>
		{
			{ PickupType.Score, 20 },
			{ PickupType.Health, 5 },
			{ PickupType.Milk, 3 },
			{ PickupType.Banana, 2 },
			{ PickupType.Stink, 2 },
			{ PickupType.Ghost, 2 },
			{ PickupType.Nothing, 66 }
		};

		int totalWeight = 0;
		foreach (var weight in weights.Values)
		{
			totalWeight += weight;
		}

		int randomValue = new Random().Next(0, totalWeight);
		foreach (var kvp in weights)
		{
			if (randomValue < kvp.Value)
			{
				return kvp.Key;
			}
			randomValue -= kvp.Value;
		}

		return PickupType.Nothing;
	}
	public void Damaged()
	{
		Health healthComponent = GetNode<Health>("Health");
		float healthPercentage = healthComponent.GetHealthPercentage();

		// As the enemy gets more damaged, we increase the red component and decrease the green and blue ones.
		AnimatedSprite2D image = GetNode<AnimatedSprite2D>("AnimatedSprite2D"); // Ensure the node name and type match your scene
		image.SelfModulate = new Color(1, healthPercentage, healthPercentage);
	}


}
