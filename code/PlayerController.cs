using System;
using System.Diagnostics;

namespace scraprunners;

public class PlayerController : Component
{


	[Property] public GameObject FirstPersonCam;
	[Property] public Vector3 ThirdPersonCamOffsets;

	[Property] private bool IsFirstPerson = true;

	[Property] public float MovementSpeed;

	[Property] public Rigidbody Rigidbody;

	public bool Waiting = false;

	public CameraComponent Camera;
	public Angles EyeAngles;
	protected override void OnStart()
	{
		Camera = Scene.GetAllComponents<CameraComponent>().First();
	}

	protected override void OnUpdate()
	{

		if ( !Waiting )
		{
			HandleUseInput();
			HandleViewInput();
		}

		PositionCamera();
		PositionPlayer();
	}

	protected async void Wait()
	{
		Waiting = true;
		await GameTask.Delay( 200 );
		Waiting = false;
	}

	protected virtual void HandleUseInput()
	{
		if ( Input.Pressed( "use" ) )
		{
			var tr = Scene.Trace.Ray( FirstPersonCam.WorldPosition, FirstPersonCam.WorldPosition + Camera.WorldRotation.Forward * 100f ).WithoutTags( "player" ).Run();
			if ( tr.Hit )
			{
				Log.Info( tr.GameObject );
				var ctrl = tr.GameObject.GetComponents<TruckController>( includeDisabled: true );
				if ( ctrl.Any() )
				{
					var truck = ctrl.First();
					Log.Info( "Player -- Using Truck" );
					truck.Drive( GameObject );

				}
			}
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

	protected virtual void PositionPlayer()
	{
		// var wishPosition = (Input.AnalogMove * EyeAngles.ToRotation()).WithZ( 0f ).Normal;
		// WorldPosition += wishPosition * MovementSpeed;
		// WorldRotation = EyeAngles.WithRoll( 0 ).WithPitch( 0 ).ToRotation();

		if (Input.Down("jump"))
		{
			DebugLog("jump");
			Rigidbody.ApplyForce(Vector3.Up * MovementSpeed * 10);
		}

		var wishPosition = (Input.AnalogMove * EyeAngles.ToRotation()).WithZ( 0f ).Normal;
		Rigidbody.SmoothMove(new Transform(WorldPosition + wishPosition * MovementSpeed), 0.1f, Time.Delta);
		Rigidbody.SmoothRotate(Camera.WorldRotation, 0.1f, Time.Delta);

	}

	protected virtual void PositionCamera()
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
				Camera.WorldPosition = tr.HitPosition + camTrace.Normal;
			}
			else
			{
				Camera.WorldPosition = tr.EndPosition;

			}
		}
	}

	protected void DebugLog( string msg )
	{
		Log.Error( this + ": " + msg );
	}
}