using Godot;
using System;

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    private ProgressBar healthBar; // This will be our health bar on the HUD.
    private Health playerHealth; // This will hold a reference to the player's health.

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        healthBar = GetNode<ProgressBar>("HealthBar"); // Correctly assign the node path for your health bar.
        playerHealth = GetParent().GetNode<Health>("Player/Health"); // Correctly assign the node path to your player's Health node.
        healthBar.MaxValue = playerHealth.MaxHealth;
        healthBar.Value = playerHealth.CurrentHealth;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        UpdateHealthBar();
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
    async public void ShowGameOver()
    {
        ShowMessage("Game Over");

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
}
