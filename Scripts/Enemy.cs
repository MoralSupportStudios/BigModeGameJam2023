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
			LookAt(player.GlobalPosition);
			Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity = direction * Speed;
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
        // Calculate which pickup to drop, if any
        PickupType selectedPickup = SelectRandomWeighted();

        // If 'Nothing' is selected, we don't drop anything
        if (selectedPickup != PickupType.Nothing)
        {
            // Instantiate the selected pickup
            PackedScene pickupScene = PickupScenes[(int)selectedPickup];
            Node2D pickupInstance = (Node2D)pickupScene.Instantiate();
            pickupInstance.Position = Position;
            GetParent().CallDeferred("add_child", pickupInstance);
        }

        QueueFree();
        //Pickup pickupInstance = (Pickup)pickupScene.Instantiate();
        //pickupInstance.Position = Position;
        
        //QueueFree();
    }
    private PickupType SelectRandomWeighted()
    {
        var weights = new Dictionary<PickupType, int>
        {
            { PickupType.Score, 20 },
            { PickupType.Health, 5 },
            { PickupType.Milk, 5 },
            { PickupType.Banana, 2 },
            { PickupType.Stink, 3 },
            { PickupType.Ghost, 1 },
            { PickupType.Nothing, 64 }
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
        var healthComponent = GetNode<Health>("Health");
        float healthPercentage = healthComponent.GetHealthPercentage();

        // As the enemy gets more damaged, we increase the red component and decrease the green and blue ones.
        var image = GetNode<Sprite2D>("GFX"); // Ensure the node name and type match your scene
        image.SelfModulate = new Color(1, healthPercentage, healthPercentage);
    }


}
