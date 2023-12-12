using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }
	private float healthIncrement = 0f;
	private float spawnIncrement = 2f;
	public int Score;
	public void GameOver()
	{
		Timer scoreTimer = GetNodeOrNull<Timer>("ScoreTimer");
		if (scoreTimer != null)
		{
			scoreTimer.Stop();
		}

		GetNode<HUD>("HUD").ShowGameOver(Score);
	}


	public void NewGame()
	{
		GetTree().CallGroup("enemy", Node.MethodName.QueueFree);
		GetTree().CallGroup("pickup", Node.MethodName.QueueFree);
		Score = 0;

		Player player = GetNode<Player>("Player");
		Marker2D startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);
		player.GetNode<Health>("Health").CurrentHealth = player.GetNode<Health>("Health").MaxHealth;

		GetNode<Timer>("StartTimer").Start();
		HUD hud = GetNode<HUD>("HUD");
		hud.UpdateScore(Score);
		hud.ShowMessage("Get Ready!");

		GetNode<Timer>("HealthIncreaseTimer").Start();
	}
	private void OnScoreTimerTimeout()
	{
		//Score++;
		GetNode<HUD>("HUD").UpdateScore(Score);
	}

	private void OnStartTimerTimeout()
	{
		GetNode<Timer>("ScoreTimer").Start();
	}
	private void OnHealthIncreaseTimerTimeout()
	{
		// Increase the health increment
		healthIncrement ++; // Increase by 10 or whatever value you choose
		spawnIncrement++;

		GD.Print("Max health is now " +  healthIncrement);
		GD.Print("Spawn Increment is now " + spawnIncrement);
	}
	public float GetHealthIncrement()
	{
		return healthIncrement;
	}
	public float GetSpawnIncrement()
	{
		return spawnIncrement;
	}
}
