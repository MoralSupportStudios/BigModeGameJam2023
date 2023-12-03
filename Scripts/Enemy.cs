using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
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
		//GD.Print("withinAttackRange " + withinAttackRange + " and timeUntilAttack " + timeUntilAttack);
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

		GD.Print("Attacked player for " + damage + " damage");
    }

	public void OnAttackRangeBodyEnter(Area2D body)
	{
        if(body.IsInGroup("player"))
		{
            withinAttackRange = true;
           // GD.Print("withinAttackRange" + withinAttackRange);
        }
    }

	public void OnAttackRangeBodyExit(Area2D body)
	{
		if(body.IsInGroup("player"))
		{
            withinAttackRange = false;
            //GD.Print("withinAttackRange" + withinAttackRange);
            timeUntilAttack = attackSpeed;
        }
	}
}
