using System;
using static Sandbox.Component;

namespace scraprunners;

public class TruckController : Component, IPressable
{
	[Property] public GameObject FirstPersonCam;
	[Property] public Vector3 ThirdPersonCamOffsets;

	[Property] private bool IsFirstPerson = true;

	[Property] public GameObject ExitPosition;
	[Property] public List<GameObject> Weapons;

	[Property] public float EnginePower { get; set; } = 100f; // Force applied for acceleration
	[Property] public float MaxSpeed { get; set; } = 500f; // Maximum speed in units per second
	[Property] public float TurnSpeed { get; set; } = 100f; // Base degrees per second
	[Property] public float BrakeForce { get; set; } = 1f; // Braking force
	[Property] public float Friction { get; set; } = 1f; // Road friction
	[Property] public float Drag { get; set; } = 0.975f; // Air resistance
	[Property] public float SuspensionStrength { get; set; } = 100f; // Suspension strength
	[Property] public float SuspensionDamping { get; set; } = 100f; // Suspension damping

	private float currentSpeed = 0f;
	private Vector3 velocity = Vector3.Zero;
	private Vector3 suspensionOffset = Vector3.Zero;

	private float TurnSpeedFactor = 0f;


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
		Wait();
		return true;
	}
	protected override void OnStart()
	{
		Camera = Scene.GetAllComponents<CameraComponent>().First();
	}

	protected override void OnFixedUpdate()
	{
		if ( HasDriver )
		{
			if ( !Waiting )
			{
				HandleUseInput();
				HandleViewInput();
			}

			PositionTruck();
			PositionWeapons();

			AddDebugOverlays();
		}

		// Do nothing without a driver!
	}

	private void AddDebugOverlays()
	{
		var thirdPersonDebugPos = WorldPosition + Vector3.Up * 100;
		var firstPersonDebugPos = FirstPersonCam.WorldPosition
			+ FirstPersonCam.WorldRotation.Forward * 100
			+ FirstPersonCam.WorldRotation.Right * 30
			+ FirstPersonCam.WorldRotation.Down * 20;
		DebugOverlay.Text( thirdPersonDebugPos, $"Speed: {currentSpeed:F2} u/s" );
		DebugOverlay.Text( firstPersonDebugPos, $"Speed: {currentSpeed:F2} u/s" );
	}

	private void PositionWeapons()
	{
		foreach ( GameObject weapon in Weapons )
		{
			var rot = weapon.WorldRotation;
			weapon.WorldRotation = Rotation.Lerp( rot, Camera.WorldRotation, 0.1f, true );

		}
	}

	protected override void OnUpdate()
	{
		PositionCamera();
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

	protected void PositionCamera()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles.roll = 0;
		EyeAngles.pitch = EyeAngles.pitch.Clamp( -89.9f, 89.9f );
		Camera.WorldRotation = Rotation.From( EyeAngles );

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

		// Reset Weapons
		foreach (GameObject weapon in Weapons)
		{
			weapon.WorldRotation = WorldRotation;
		}

		HasDriver = false;
	}

	protected void DebugLog( string msg )
	{
		Log.Info( this + ": " + msg );
	}

	public void PositionTruck()
	{

		// Input handling
		var forwardInput = Input.AnalogMove.x;
		var turnInput = (currentSpeed >= 0) ? Input.AnalogMove.y : Input.AnalogMove.y * -1; // Invert turning when going backwards.
		TurnSpeedFactor = (turnInput == 0) ? TurnSpeedFactor : Math.Clamp( TurnSpeedFactor + 0.02f, 0, 1 );

		// Acceleration and engine power
		if ( forwardInput != 0 )
		{
			currentSpeed += forwardInput * EnginePower * Time.Delta;
		}
		else
		{
			// Natural deceleration due to drag and friction
			currentSpeed *= Drag;
		}

		// Clamp speed
		currentSpeed = currentSpeed.Clamp( -MaxSpeed, MaxSpeed );

		// Only allow turning if moving
		if ( Math.Abs( currentSpeed ) > 10f )
		{
			var turnAngle = turnInput * TurnSpeed * TurnSpeedFactor * Time.Delta;

			WorldRotation *= Rotation.FromYaw( turnAngle );
			EyeAngles += Rotation.FromYaw( turnAngle ).Angles();
		}

		// Simulate suspension
		suspensionOffset = Vector3.Lerp( suspensionOffset, Vector3.Zero, SuspensionDamping * Time.Delta );

		// Update velocity and position
		velocity = WorldRotation.Forward * currentSpeed;
		velocity *= Friction; // Road friction

		WorldPosition += velocity * Time.Delta + suspensionOffset;

		// Apply braking
		if ( Input.Down( "jump" ) ) // Example: Space for braking
		{
			currentSpeed = MathX.Lerp( currentSpeed, 0, BrakeForce * Time.Delta );
		}

		// Always reduce turn input to 0 
		TurnSpeedFactor = Math.Clamp( TurnSpeedFactor - 0.01f, 0, 1 );
	}
}