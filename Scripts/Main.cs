using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private int _score;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //NewGame();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    //public void GameOver()
    //{
    //    GetNode<Timer>("MobTimer").Stop();
    //    GetNode<Timer>("ScoreTimer").Stop();
    //    GetNode<HUD>("HUD").ShowGameOver();
    //}

    public void NewGame()
    {
        GetTree().CallGroup("enemy", Node.MethodName.QueueFree);
        _score = 0;

        Player player = GetNode<Player>("Player");
        Marker2D startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();
        HUD hud = GetNode<HUD>("HUD");
        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");
    }
    private void OnScoreTimerTimeout()
    {
        _score++;
        GetNode<HUD>("HUD").UpdateScore(_score);
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }
}
