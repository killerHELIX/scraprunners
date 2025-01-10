using Sandbox;

namespace scraprunners;
public class TruckCannon : TruckWeapon
{
	[Property] public override string Name { get; set; } = "Cannon";
	[Property] public override float FireRate { get; set; } = 1.0f; // Fire rate, in seconds

	public override void Fire()
	{

		if ( !Waiting )
		{
			Log.Info( $"{Name} fired!" );
			Wait(FireRate * SEC_TO_MS);
		}
		else
		{
			Log.Info("Waiting...");
		}
	}

	protected override void OnUpdate()
	{

	}
}
