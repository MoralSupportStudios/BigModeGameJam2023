using Godot;
using System;

public partial class Health : Node2D
{
    [Export] public float MaxHealth = 100f;
    [Export] public float CurrentHealth;

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;
    }

    public void Damage(float damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            if(GetParent().HasMethod("Die"))
            {
                GetParent().Call("Die");
            }
            //GetParent().QueueFree();
        }
        else
        {
            if(GetParent().HasMethod("Damaged"))
            {
                GetParent().Call("Damaged");
                
            }
        }
    }
    public float GetHealthPercentage()
    {
        return CurrentHealth / MaxHealth;
    }

}
