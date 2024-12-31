using System;

namespace scraprunners;

public class PlayerController : Component
{


	[Property]
	public GameObject FirstPersonCam;
	[Property]
	public Vector3 ThirdPersonCamOffsets;

	[Property]
	private bool IsFirstPerson = true;

	[Property]
	public float MovementSpeed;

	public CameraComponent Camera;
	public Angles EyeAngles;
	protected override void OnStart()
	{
		Camera = Scene.GetAllComponents<CameraComponent>().First();
	}

	protected override void OnUpdate()
	{
		HandleUseInput();
		HandleViewInput();

		PositionCamera();
		PositionPlayer();
	}

	private void HandleUseInput()
	{
		if ( Input.Pressed( "use" ) )
		{
			Log.Info( "use" );
			Gizmo.Draw.Line( FirstPersonCam.WorldPosition, FirstPersonCam.WorldPosition + Camera.WorldRotation.Forward * 50f );
			var tr = Scene.Trace.Ray( FirstPersonCam.WorldPosition, FirstPersonCam.WorldPosition + Camera.WorldRotation.Forward * 50f ).WithoutTags( "player" ).Run();
			if ( tr.Hit )
			{
				Log.Info( tr.GameObject );
				var ctrl = tr.GameObject.GetComponents<TruckController>( includeDisabled: true );
				if ( ctrl.Any() )
				{
					var truck = ctrl.First();
					Log.Info( "TRUCK DETECTED!!!" );
					truck.Drive(this);

				}
			}
		}
	}

	private void HandleViewInput()
	{
		if ( Input.Pressed( "view" ) )
		{
			Log.Info( "view" );
			IsFirstPerson = !IsFirstPerson;
		}
	}

	private void PositionPlayer()
	{
		var wishPosition = (Input.AnalogMove * EyeAngles.ToRotation()).WithZ( 0f ).Normal;
		WorldPosition += wishPosition * MovementSpeed;
		WorldRotation = EyeAngles.WithRoll( 0 ).WithPitch( 0 ).ToRotation();
	}

	private void PositionCamera()
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

			var tr = Scene.Trace.Ray( FirstPersonCam.WorldPosition, WorldPosition + camTrace ).Run();
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
}