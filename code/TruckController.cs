using System;
using Sandbox;
using static Sandbox.Component;

namespace scraprunners;

public sealed class TruckController : PlayerController
{

	public PlayerController Driver;

	public void Drive( PlayerController pc )
	{
		Driver = pc;

		this.Enabled = true;
		Driver.Enabled = false;
	}

	protected override void OnUpdate()
	{

		base.OnUpdate();

		if (Input.Pressed("use"))
		{
			Log.Info("TRUCK USE!!");
			Driver.Enabled = true;
			this.Enabled = false;
		}
	}

}