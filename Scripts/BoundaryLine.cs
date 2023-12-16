using Godot;
using System;

public partial class BoundaryLine : Line2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Vector2[] boundaryPoints = new Vector2[] {
            new Vector2(-2048, -2048),
            new Vector2(2048, -2048),
            new Vector2(2048, 2048),
            new Vector2(-2048, 2048),
            new Vector2(-2048, -2048) // Close the rectangle
        };
        Points = boundaryPoints;
        Width = 2;
        DefaultColor = new Color(1, 0, 0); // Red color for the boundary line

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
