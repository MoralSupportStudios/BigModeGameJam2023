using Godot;
using System.Collections.Generic;


public partial class StinkTrail : Area2D
{
    [Export] public float damage = 1f;
    [Export] public float damageInterval = 1.0f; // Duration of the stink trail

    private List<Node> enemiesInTrail = new List<Node>();
    private Timer damageTimer;
    public override void _Ready()
	{
        // Set up the particles
        var particles = GetNode<CpuParticles2D>("CPUParticles2D");
        particles.Emitting = true; // Start emitting particles immediately

        Timer damageTimer = GetNode<Timer>("Timer"); // Corrected to use the existing Timer node.
        damageTimer.WaitTime = damageInterval;
        damageTimer.Autostart = true;
        damageTimer.OneShot = false;
        damageTimer.Start();

        Timer lifetimeTimer = GetNode<Timer>("LifetimeTimer"); // Corrected to use the existing Timer node.
        lifetimeTimer.OneShot = true;
        lifetimeTimer.Start();

    }
    private void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("enemy") && !enemiesInTrail.Contains(body))
        {
            enemiesInTrail.Add(body);
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body.IsInGroup("enemy"))
        {
            enemiesInTrail.Remove(body);
        }
    }

    private void OnDamageTimeout()
    {
        // Apply damage to all enemies currently in the trail
        foreach (Node enemyNode in enemiesInTrail)
        {
            if (enemyNode != null && enemyNode is Enemy)
            {
                Enemy enemy = enemyNode as Enemy;
                enemy.GetNode<Health>("Health").Damage(damage);
            }
        }
    }
    private void OnLifetimeTimeout()
    {
        QueueFree(); // This will remove the StinkTrail from the scene
    }


    public override void _ExitTree()
    {
        // Clean up
        enemiesInTrail.Clear();
    }
}
