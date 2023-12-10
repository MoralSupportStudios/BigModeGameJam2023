using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export] PackedScene pickupScene;
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
        Pickup pickupInstance = (Pickup)pickupScene.Instantiate();
        pickupInstance.Position = Position;
        GetParent().CallDeferred("add_child", pickupInstance);
        QueueFree();
    }
    public void Damaged()
    {
        var healthComponent = GetNode<Health>("Health");
        float healthPercentage = healthComponent.GetHealthPercentage();

        var image = GetNode<Sprite2D>("GFX");
        image.SelfModulate = new Color(1, 1 - healthPercentage, 1 - healthPercentage);
    }

}
