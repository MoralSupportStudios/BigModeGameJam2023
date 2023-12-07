using Godot;
using System;

public partial class Boomerang : RigidBody2D
{
    [Export] public float damage = 2f;
    [Export] public float speed = 300f; // Speed of the boomerang
    [Export] public float range = 500f; // Range before returning
    [Export] public float returnTime = 1.5f; // Time before the boomerang returns

    private Vector2 startPosition;
    private Player player;
    private bool isReturning = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        startPosition = Position;
        player = GetTree().Root.GetNodeOrNull<Player>("Main/Player"); // Corrected the scope of the 'player' variable.

        // Assuming you have a Timer node named "Timer" as part of the scene.
        Timer returnTimer = GetNode<Timer>("Timer"); // Corrected to use the existing Timer node.
        returnTimer.WaitTime = returnTime;
        returnTimer.OneShot = true;
        returnTimer.Start(); // The connection is now made in the editor, so no need to connect in code.

        // Apply initial impulse to the boomerang
        ApplyCentralImpulse(Transform.X.Normalized() * speed);

        returnTimer.Timeout += () => QueueFree();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
        if (isReturning)
        {
            // Calculate the direction from the bullet to the player
            Vector2 directionToPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
            LinearVelocity = directionToPlayer * speed;
        }
        else if (Position.DistanceTo(startPosition) > range)
        {
            // If the bullet has reached its max range and isn't returning, start the return
            StartReturn();
        }
    }
    private void StartReturn()
    {
        isReturning = true;
        // You can add additional effects or changes when the boomerang starts returning
    }

    private void OnReturnTimerTimeout()
    {
        StartReturn();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (!isReturning && body.IsInGroup("enemy"))
        {
            // Deal damage to the enemy
            body.GetNode<Health>("Health").Damage(damage);
            // Start returning after hitting an enemy
            StartReturn();
        }
    }
}
