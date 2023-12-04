using Godot;
using System;

public partial class EnemySpawner : Node2D
{
	[Export] public PackedScene EnemyScene;
	[Export] public Node2D[] SpawnPoints;
	[Export] public float EnemyPerSeconds = 1f;
    float spawnRate;
	float timeUntilSpawn = 0f;
	public override void _Ready()
	{
		spawnRate = 1f / EnemyPerSeconds;
    }

    public override void _Process(double delta)
    {
        Player player = GetTree().Root.GetNodeOrNull<Player>("Main/Player");

        if (player != null && player.IsVisibleInTree())
        {
            if (timeUntilSpawn > spawnRate)
            {
                Spawn();
                timeUntilSpawn = 0f;
            }
            else
            {
                timeUntilSpawn += (float)delta;
            }
        }
        else
        {
            // Even if the player is not visible, we should still increment the time
            // This ensures that enemies will spawn immediately when the player is visible again
            timeUntilSpawn += (float)delta;

            //Remove all enemies from the scene
            GetTree().CallGroup("enemy", Node.MethodName.QueueFree);
            GetTree().CallGroup("pickup", Node.MethodName.QueueFree);
        }
    }


    public void Spawn()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
        Vector2 location = SpawnPoints[rng.Randi() % SpawnPoints.Length].GlobalPosition;
        Enemy enemy = (Enemy)EnemyScene.Instantiate();
		enemy.GlobalPosition = location;
		GetTree().Root.AddChild(enemy);
    }
}
