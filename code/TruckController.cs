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
	protected void DriverExit()
	{
		this.Enabled = false;
		Driver.Enabled = true;
		Driver.WorldTransform = ExitPosition.WorldTransform;
		Driver.WorldRotation = EyeAngles.ToRotation();
	}

}