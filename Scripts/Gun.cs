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
	[Export] float defaultBulletDamage = 1f;
	Dictionary<BabyMode, float> bulletDamages;
	Dictionary<BabyMode, float> babyModeFireRates = new Dictionary<BabyMode, float>();

	private BabyMode currentBabyMode = BabyMode.MilkBullet;
	float fireRate;
	float timeUntilFire = 0f;
	Dictionary<BabyMode, PackedScene> babyModeBullet;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize the dictionary with fire rates for each BabyMode
		babyModeFireRates[BabyMode.MilkBullet] = 5f; 
		babyModeFireRates[BabyMode.BananaBoomerang] = 2f;
		babyModeFireRates[BabyMode.StinkTrail] = 4f; 
		babyModeFireRates[BabyMode.GhostPower] = 1f; 

		// Set the initial fire rate based on the currentBabyMode
		fireRate = 1f / babyModeFireRates[currentBabyMode];


		babyModeBullet = new Dictionary<BabyMode, PackedScene>()
		{
			{ BabyMode.MilkBullet, bulletScenes[0] },
			{ BabyMode.BananaBoomerang, bulletScenes[1] },
			{ BabyMode.StinkTrail, bulletScenes[2] },
			{ BabyMode.GhostPower, bulletScenes[3] },
		};
		bulletDamages = new Dictionary<BabyMode, float>
		{
			{ BabyMode.MilkBullet, defaultBulletDamage },
			{ BabyMode.BananaBoomerang, defaultBulletDamage },
			{ BabyMode.StinkTrail, defaultBulletDamage },
			{ BabyMode.GhostPower, defaultBulletDamage }
		};
	}

	public void SwitchBabyMode(BabyMode mode)
	{
		if (Enum.IsDefined(typeof(BabyMode), mode) && babyModeFireRates.ContainsKey(mode))
		{
			currentBabyMode = mode;
			fireRate = 1f / babyModeFireRates[mode]; // Update the fire rate for the new mode
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

			if (Input.IsActionPressed("click")) // Changed from IsActionJustPressed to IsActionPressed
			{
				timeUntilFire -= (float)delta;
				if (timeUntilFire <= 0f)
				{
					FireBullet();
					timeUntilFire = fireRate; // Reset the time until the next fire based on the fire rate
				}
			}
		}
	}
	public void IncreaseDamage(BabyMode mode)
	{
		if (bulletDamages.ContainsKey(mode))
		{
			bulletDamages[mode] += 1f; // Adjust this value as needed
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
						RigidBody2D ghostBullet = (RigidBody2D)bulletScene.Instantiate();
						if (ghostBullet is Bullet ghostShotgun)
						{
							ghostShotgun.damage = bulletDamages[currentBabyMode];
							ghostShotgun.Rotation = currentAngle;
							ghostShotgun.GlobalPosition = GlobalPosition;
							ghostShotgun.LinearVelocity = new Vector2(bulletSpeed, 0).Rotated(currentAngle);
						GetTree().Root.AddChild(ghostShotgun);

							// Increment the angle for the next bullet
							currentAngle += Mathf.DegToRad(angleStep);
						}
					}
					break;
				case BabyMode.StinkTrail:
					// For StinkTrail, instance it at the player's position
					Area2D stinkTrailInstance = bulletScene.Instantiate<Area2D>();
					if (stinkTrailInstance is StinkTrail stinkTrail)
					{
						stinkTrail.GlobalPosition = GlobalPosition;
						FGetNode<AudioStreamPlayer>("Shot").Play();
						stinkTrail.damage = bulletDamages[currentBabyMode];
						GetTree().Root.AddChild(stinkTrail);
					}
					break;
				case BabyMode.BananaBoomerang:
					Area2D boomerangInstance = bulletScene.Instantiate<Area2D>();
					if(boomerangInstance is Boomerang bananarang)
					{
						bananarang.orbitCenter = GlobalPosition;
						bananarang.GlobalPosition = GlobalPosition;
						GetTree().Root.AddChild(boomerangInstance);
					}
					break;
				default:
					RigidBody2D bulletInstance = (RigidBody2D)bulletScene.Instantiate();
					if (bulletInstance is Bullet bullet)
					{
						bullet.damage = bulletDamages[currentBabyMode];
						bullet.Rotation = GlobalRotation;
						bullet.GlobalPosition = GlobalPosition;
						bullet.LinearVelocity = new Vector2(bulletSpeed, 0).Rotated(bullet.Rotation);
						GetTree().Root.AddChild(bullet);
					}
					break;
			}
		}
	}

}
