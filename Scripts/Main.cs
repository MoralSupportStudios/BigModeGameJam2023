using Godot;
using System;
using static Gun;

public partial class Main : Node
{
	private float healthIncrement = 0.1f;
	private float spawnIncrement = 0.1f;
	public int Score;
	[Export] AudioStreamPlayer title;
	[Export] AudioStreamPlayer overworld;
	[Export] AudioStreamPlayer end;

	public override void _Ready()
	{
		title = GetNode<AudioStreamPlayer>("title");
		overworld = GetNode<AudioStreamPlayer>("overworld");
		end = GetNode<AudioStreamPlayer>("end");

		end.Stop();
		title.Play();
		overworld.Stop();
	}

	public void GameOver()
	{
		Timer scoreTimer = GetNodeOrNull<Timer>("ScoreTimer");
		if (scoreTimer != null)
		{
			scoreTimer.Stop();
		}

		GetNode<HUD>("HUD").ShowGameOver(Score);
		end.Play();
		title.Stop();
		overworld.Stop();
	}


	public void NewGame()
	{
		GetTree().CallGroup("enemy", Node.MethodName.QueueFree);
		GetTree().CallGroup("pickup", Node.MethodName.QueueFree);
		Score = 0;
		healthIncrement = 0f;
		spawnIncrement = 2f;

		Player player = GetNode<Player>("Player");
		
		Gun playerGun = player.GetNode<Gun>("Gun");
		playerGun.SwitchBabyMode(BabyMode.MilkBullet);


		Marker2D startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);
		player.GetNode<Health>("Health").CurrentHealth = player.GetNode<Health>("Health").MaxHealth;

		GetNode<Timer>("StartTimer").Start();
		HUD hud = GetNode<HUD>("HUD");
		hud.UpdateScore(Score);
		hud.ShowMessage("Get Ready!");
		//hud.UpdatePowerUpSpriteAndDPS(BabyMode.MilkBullet, playerGun);

		GetNode<Timer>("HealthIncreaseTimer").Start();

		end.Stop();
		title.Stop();
		overworld.Play();
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
