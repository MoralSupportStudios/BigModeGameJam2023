using Godot;
using Godot.Collections;
using System;

public partial class Gun : Node2D
{
    public enum BabyMode
    {
        MilkBullet,
        BananaBoomerang,
        StinkTrail,
        GhostPower,
    }
    [Export] PackedScene[] bulletScenes;
    [Export] float bulletSpeed = 600f;
    [Export] float bulletPerSecond = 5f;
    [Export] float bulletDamage = 1f;

    private BabyMode currentBabyMode = BabyMode.MilkBullet;
    float fireRate;
    float timeUntilFire = 0f;
    Dictionary<BabyMode, PackedScene> babyModeBullet;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        fireRate = 1f / bulletPerSecond;

        babyModeBullet = new Dictionary<BabyMode, PackedScene>()
        {
            { BabyMode.MilkBullet, bulletScenes[0] },
            { BabyMode.BananaBoomerang, bulletScenes[1] },
            { BabyMode.StinkTrail, bulletScenes[2] },
            { BabyMode.GhostPower, bulletScenes[3] },
        };
    }

    public void SwitchBabyMode(BabyMode mode)
    {
        if (Enum.IsDefined(typeof(BabyMode), mode) && (int)mode < bulletScenes.Length)
        {
            currentBabyMode = mode;
        }
    }

    public void CycleBabyMode()
    {
        // Get the next mode by incrementing the current mode and then taking the modulus
        int nextMode = (((int)currentBabyMode + 1) % bulletScenes.Length);
        SwitchBabyMode((BabyMode)nextMode);
    }


    public override void _Process(double delta)
    {
        Player player = GetTree().Root.GetNodeOrNull<Player>("Main/Player");
        LookAt(GetGlobalMousePosition());
        if (player != null && player.IsVisibleInTree())
        {
            if (Input.IsActionJustPressed("RightClick"))
            {
                CycleBabyMode();
            }

            if (Input.IsActionJustPressed("click") && timeUntilFire >= fireRate)
            {
                FireBullet();
                timeUntilFire = 0f;
            }
            else
            {
                timeUntilFire += (float)delta;
            }
        }
    }

    private void FireBullet()
    {
        if ((int)currentBabyMode >= bulletScenes.Length || bulletScenes[(int)currentBabyMode] == null)
        {
            GD.Print("No bullet scene assigned for this BabyMode or scene is null.");
            return;
        }

        PackedScene bulletScene = bulletScenes[(int)currentBabyMode];
        if (bulletScene != null)
        {
            switch (currentBabyMode)
            {
                case BabyMode.GhostPower:

                    // Define the spread and number of bullets
                    int numberOfBullets = 5; // Number of bullets in the spread
                    float spreadAngle = 45.0f; // Total angle of spread

                    // Calculate the angle between each bullet
                    float angleStep = spreadAngle / (numberOfBullets - 1);
                    float currentAngle = GlobalRotation - Mathf.DegToRad(spreadAngle) / 2;

                    for (int i = 0; i < numberOfBullets; i++)
                    {
                        // Instantiate a new bullet
                        RigidBody2D ghostBullet = bulletScene.Instantiate<RigidBody2D>();
                        ghostBullet.Rotation = currentAngle;
                        ghostBullet.GlobalPosition = GlobalPosition;
                        ghostBullet.LinearVelocity = new Vector2(bulletSpeed, 0).Rotated(currentAngle);

                        GetTree().Root.AddChild(ghostBullet);

                        // Increment the angle for the next bullet
                        currentAngle += Mathf.DegToRad(angleStep);
                    }
                    break;
                case BabyMode.StinkTrail:
                    // For StinkTrail, instance it at the player's position
                    Area2D stinkTrailInstance = bulletScene.Instantiate<Area2D>();
                    stinkTrailInstance.GlobalPosition = GlobalPosition; // Or some offset from the player's position
                    GetTree().Root.AddChild(stinkTrailInstance);
                    break;
                default:
                    // For other bullet types, use the existing logic
                    RigidBody2D bulletInstance = bulletScene.Instantiate<RigidBody2D>();
                    bulletInstance.Rotation = GlobalRotation;
                    bulletInstance.GlobalPosition = GlobalPosition;
                    bulletInstance.LinearVelocity = new Vector2(bulletSpeed, 0).Rotated(bulletInstance.Rotation);
                    GetTree().Root.AddChild(bulletInstance);

                    break;
            }
        }
    }

}
