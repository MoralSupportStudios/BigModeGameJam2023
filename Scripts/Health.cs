using Godot;

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
        }
        else
        {
            if(GetParent().HasMethod("Damaged"))
            {
                GetParent().Call("Damaged");
                GD.Print(damage + " taken");
            }
        }
    }

    public void Heal(float heal)
    {
        if(CurrentHealth <= MaxHealth)
        {
            CurrentHealth += heal;
        }
    }

    public float GetHealthPercentage()
    {
        return CurrentHealth / MaxHealth;
    }

}
