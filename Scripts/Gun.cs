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
        GhostPower
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
			//{ BabyMode.StinkTrail, bulletScene[2] },
			//{ BabyMode.GhostPower, bulletScene[3] },
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
        // Ensure the current mode has an assigned scene.
        if ((int)currentBabyMode >= bulletScenes.Length || bulletScenes[(int)currentBabyMode] == null)
        {
            GD.Print("No bullet scene assigned for this BabyMode or scene is null.");
            return;
        }

        PackedScene bulletScene = bulletScenes[(int)currentBabyMode];
        if (bulletScene != null)
        {
            RigidBody2D bulletInstance = bulletScene.Instantiate<RigidBody2D>();

            bulletInstance.Rotation = GlobalRotation;
            bulletInstance.GlobalPosition = GlobalPosition;
            bulletInstance.LinearVelocity = new Vector2(bulletSpeed, 0).Rotated(bulletInstance.Rotation);

            // Set common properties for all bullets, if any
            // e.g., damage, bullet owner, etc.

            GetTree().Root.AddChild(bulletInstance);
        }
        else
        {
            GD.Print("Bullet scene not found for mode: " + currentBabyMode.ToString());
        }
    }
}
