using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    public int Score;
    public void GameOver()
    {
        // Correct the node path as necessary.
        var mobTimer = GetNodeOrNull<Timer>("MobTimer");

        if (mobTimer != null)
        {
            mobTimer.Stop();
        }
        else
        {
            GD.Print("MobTimer node not found!");
        }

        var scoreTimer = GetNodeOrNull<Timer>("ScoreTimer");
        if (scoreTimer != null)
        {
            scoreTimer.Stop();
        }

        GetNode<HUD>("HUD").ShowGameOver();
    }


    public void NewGame()
    {
        GetTree().CallGroup("enemy", Node.MethodName.QueueFree);
        Score = 0;

        Player player = GetNode<Player>("Player");
        Marker2D startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);
        player.GetNode<Health>("Health").CurrentHealth = player.GetNode<Health>("Health").MaxHealth;

        GetNode<Timer>("StartTimer").Start();
        HUD hud = GetNode<HUD>("HUD");
        hud.UpdateScore(Score);
        hud.ShowMessage("Get Ready!");
    }
    private void OnScoreTimerTimeout()
    {
        Score++;
        GetNode<HUD>("HUD").UpdateScore(Score);
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("ScoreTimer").Start();
    }
}
