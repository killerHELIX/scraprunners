using System;
using Sandbox;
using static Sandbox.Component;

namespace scraprunners;

public class TruckController : PlayerController
{

	public GameObject Driver;

	[Property] public GameObject ExitPosition;

	public void Drive( GameObject driver )
	{
		Driver = driver;

		Driver.Enabled = false;
		this.Enabled = true;

		// Wait();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	protected override void HandleUseInput()
	{
		if ( Input.Pressed( "use" ) )
		{
			Log.Info( "Truck -- Use" );
			DriverExit();
		}
	}

	// protected override void PositionPlayer()
	// {
	// 	var wishPosition = (Input.AnalogMove * EyeAngles.ToRotation()).WithZ( 0f ).Normal;
	// 	WorldPosition += wishPosition * MovementSpeed;
	// 	// WorldRotation = EyeAngles.WithRoll( 0 ).WithPitch( 0 ).ToRotation();
	// }

	protected void DriverExit()
	{
		this.Enabled = false;
		Driver.WorldPosition = ExitPosition.WorldPosition;
		Driver.WorldRotation = Camera.WorldRotation;
		Driver.Enabled = true;
	}

}