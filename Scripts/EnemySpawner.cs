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
		if(timeUntilSpawn > spawnRate)
		{
            Spawn();
            timeUntilSpawn = 0;
        }
        else
		{
            timeUntilSpawn += (float)delta;
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
