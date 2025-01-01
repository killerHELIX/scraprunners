using System;
using Sandbox;
using static Sandbox.Component;

namespace scraprunners;

public class TruckController : Component, IPressable
{
	[Property] public GameObject FirstPersonCam;
	[Property] public Vector3 ThirdPersonCamOffsets;

	[Property] private bool IsFirstPerson = true;

	[Property] public float MovementSpeed;

	[Property] public GameObject ExitPosition;

	public bool Waiting = false;

	public CameraComponent Camera;
	public Angles EyeAngles;
	public GameObject Driver;


	public bool HasDriver = false;

	public bool Press( IPressable.Event e )
	{
		Log.Info( e );
		var driver = e.Source.GameObject;
		Drive( driver: driver );
		return true;
	}
	protected override void OnStart()
	{
		Camera = Scene.GetAllComponents<CameraComponent>().First();
	}

	protected override void OnUpdate()
	{
		if ( HasDriver )
		{
			if ( !Waiting )
			{
				HandleUseInput();
				HandleViewInput();
			}

			PositionCamera();
			PositionPlayer();
		}

		// Do nothing without a driver!
	}

	protected async void Wait()
	{
		Waiting = true;
		await GameTask.Delay( 200 );
		Waiting = false;
	}

	protected void HandleUseInput()
	{
		if ( Input.Pressed( "use" ) )
		{
			DebugLog( "Use" );
			DriverExit();
		}
	}

	protected virtual void HandleViewInput()
	{
		if ( Input.Pressed( "view" ) )
		{
			DebugLog( "View" );
			IsFirstPerson = !IsFirstPerson;

			// Wait();
		}
	}

	protected void PositionPlayer()
	{
		var wishPosition = (Input.AnalogMove * EyeAngles.ToRotation()).WithZ( 0f ).Normal;
		WorldPosition += wishPosition * MovementSpeed;
		// WorldRotation = EyeAngles.WithRoll( 0 ).WithPitch( 0 ).ToRotation();
	}

	protected void PositionCamera()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles.roll = 0;
		EyeAngles.pitch = EyeAngles.pitch.Clamp( -89.9f, 89.9f );
		Camera.WorldRotation = Rotation.From( EyeAngles );

		// FirstPersonPosition.WorldPosition = WorldPosition + FirstPersonCam;

		if ( IsFirstPerson )
		{
			Camera.WorldPosition = FirstPersonCam.WorldPosition;
		}
		else
		{
			var camTrace = Camera.WorldRotation.Backward * ThirdPersonCamOffsets.x;
			camTrace += Camera.WorldRotation.Left * ThirdPersonCamOffsets.y;
			camTrace += Camera.WorldRotation.Up * ThirdPersonCamOffsets.z;

			var tr = Scene.Trace.Ray( FirstPersonCam.WorldPosition, FirstPersonCam.WorldPosition + camTrace ).Run();
			if ( tr.Hit )
			{
				Camera.WorldPosition = tr.HitPosition - (camTrace.Normal * 10f);
			}
			else
			{
				Camera.WorldPosition = tr.EndPosition;

			}
		}
	}

	public void Drive( GameObject driver )
	{
		Driver = driver;

		Driver.Enabled = false;

		HasDriver = true;

		// Wait();
	}

	protected void DriverExit()
	{
		Driver.WorldPosition = ExitPosition.WorldPosition;
		Driver.WorldRotation = Camera.WorldRotation;
		Driver.Enabled = true;

		HasDriver = false;
	}

	protected void DebugLog( string msg )
	{
		Log.Info( this + ": " + msg );
	}

}