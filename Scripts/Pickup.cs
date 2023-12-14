using Godot;
using static Gun;
public enum PickupType
{
    Score,
    Health,
    Milk,
    Banana,
    Stink,
    Ghost,
    Nothing // Represents no pickup
}
public partial class Pickup : Area2D
{
    [Signal]
    public delegate void CollectedEventHandler();
    [Export]
    public PickupType TypeOfPickup;

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

            // Handle the pickup effect based on the type
            HandlePickupEffect(TypeOfPickup, area);


            QueueFree();
        }
	}
    private void HandlePickupEffect(PickupType pickupType, Area2D playerArea)
    {
        var player = GetTree().Root.GetNode("Main").GetNode("Player");
        Gun gun = GetTree().Root.GetNode("Main").GetNode("Player").GetNode<Gun>("Gun");
        switch (pickupType)
        {
            case PickupType.Score:
                GetTree().Root.GetNode<Main>("Main").Score++;
                var score = GetTree().Root.GetNode<Main>("Main").Score;
                GetTree().Root.GetNode<Main>("Main").GetNode<HUD>("HUD").UpdateScore(score);
                break;
            case PickupType.Health:
                player.GetNode<Health>("Health").Heal(1);
                break;
            case PickupType.Milk:
                gun.IncreaseDamage(BabyMode.MilkBullet);
                gun.SwitchBabyMode(BabyMode.MilkBullet);
                break;
            case PickupType.Banana:
                gun.IncreaseDamage(BabyMode.BananaBoomerang);
                gun.SwitchBabyMode(BabyMode.BananaBoomerang);
                break;
            case PickupType.Stink:
                gun.IncreaseDamage(BabyMode.StinkTrail);
                gun.SwitchBabyMode(BabyMode.StinkTrail);
                break;
            case PickupType.Ghost:
                gun.IncreaseDamage(BabyMode.GhostPower);
                gun.SwitchBabyMode(BabyMode.GhostPower);
                break;
            default:
                // Do nothing or some default action
                break;
        }
    }
}
