using Godot;
using System;
using static System.Formats.Asn1.AsnWriter;

public partial class Pickup : Area2D
{
    [Signal]
    public delegate void CollectedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnAreaEntered(Area2D area)
	{
		if(area.IsInGroup("player"))
		{
            EmitSignal(SignalName.Collected);
			GetTree().Root.GetNode<Main>("Main").Score++;
			var score = GetTree().Root.GetNode<Main>("Main").Score;
            GetTree().Root.GetNode<Main>("Main").GetNode<HUD>("HUD").UpdateScore(score);
            QueueFree();
        }
	}
}
