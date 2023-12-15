using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	private ProgressBar healthBar; // This will be our health bar on the HUD.
	private Health playerHealth; // This will hold a reference to the player's health.
	private Gun playerGun;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthBar = GetNode<ProgressBar>("HealthBar"); // Correctly assign the node path for your health bar.
		playerHealth = GetParent().GetNode<Health>("Player/Health"); // Correctly assign the node path to your player's Health node.
		playerGun = GetParent().GetNode<Gun>("Player/Gun");
		healthBar.MaxValue = playerHealth.MaxHealth;
		healthBar.Value = playerHealth.CurrentHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateHealthBar();
		UpdatePowerUpSpriteAndDPS();

	}
	private void UpdateHealthBar()
	{
		if (playerHealth != null && healthBar != null)
		{
			healthBar.Value = playerHealth.CurrentHealth;
		}
	}
	public void ShowMessage(string text)
	{
		Label message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}
	async public void ShowGameOver(int score)
	{
		ShowMessage("Final Score = " + score);

		Timer messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		Label message = GetNode<Label>("Message");
		message.Text = "Shoot with Left Click! Move with WASD or Arrow Keys";
		message.Show();

		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GetNode<Button>("StartButton").Show();
	}
	public void UpdateScore(int score)
	{
		GetNode<Label>("ScoreLabel").Text = "Score: " + score.ToString();
	}
	private void OnStartButtonPressed()
	{
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}

	private void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}

	private void UpdatePowerUpSpriteAndDPS()
	{
		// Iterate through each BabyMode and update the DPS and sprite
		foreach (Gun.BabyMode mode in Enum.GetValues(typeof(Gun.BabyMode)))
		{
			UpdatePowerUpDisplayForMode(mode);
		}
	}
	private void UpdatePowerUpDisplayForMode(Gun.BabyMode mode)
	{
		// Construct the node paths based on the actual node names in the scene
		string modeName = mode.ToString();
		string containerPath = $"HBoxContainer/{modeName}Container";
		string labelPath = $"{modeName}Label";
		string texturePath = $"{modeName}Texture";

		// Retrieve the container using the constructed path
		var container = GetNodeOrNull<HBoxContainer>(containerPath);
		if (container == null)
		{
			GD.Print($"Container not found for mode: {modeName}");
			return;
		}

		// Retrieve the label and texture rect using their constructed paths
		var label = container.GetNodeOrNull<Label>(labelPath);
		var textureRect = container.GetNodeOrNull<TextureRect>(texturePath);

		if (label == null)
		{
			GD.Print($"Label not found for mode: {modeName}");
			return;
		}
		if (textureRect == null)
		{
			GD.Print($"TextureRect not found for mode: {modeName}");
			return;
		}

		if (playerGun == null)
		{
			GD.Print("playerGun is null");
			return;
		}

		// Calculate and update DPS
		float damage = playerGun.GetBulletDamage(mode);
		float fireRate = playerGun.GetFireRate(mode);
		float dps = damage * fireRate;
		label.Text = $"DPS: {dps}";
	}
}
