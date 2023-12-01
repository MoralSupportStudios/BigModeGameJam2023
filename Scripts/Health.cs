using Godot;
using System;

public partial class Health : Node2D
{
    [Export] public float MaxHealth = 100f;
    float health;

    public override void _Ready()
    {
        health = MaxHealth;
    }

    public void Damage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            GetParent().QueueFree();
        }
    }
}
