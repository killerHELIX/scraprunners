using System;
using Sandbox;
using Sandbox.ModelEditor;
using static Sandbox.Component;

namespace scraprunners;

public class TruckController : Component, IPressable
{
	[Property] public GameObject FirstPersonCam;
	[Property] public Vector3 ThirdPersonCamOffsets;

	[Property] private bool IsFirstPerson = true;

	[Property] public float MovementSpeed;

	[Property] public GameObject ExitPosition;
	[Property] public GameObject TurnAxis;

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

			// PositionCamera();
			// PositionPlayer();

			PositionTruck();

		}

		// Do nothing without a driver!
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

	protected void PositionPlayer()
	{
		DebugLog( "" + Input.AnalogMove );
		// var wishPosition = (Input.AnalogMove * EyeAngles.ToRotation()).WithZ( 0f ).Normal;
		// WorldPosition += wishPosition * MovementSpeed;
		// WorldRotation = EyeAngles.WithRoll( 0 ).WithPitch( 0 ).ToRotation();

		var forward = WorldRotation.Forward * Input.AnalogMove.x;
		// var left = WorldRotation.Left * Input.AnalogMove.y;

		WorldPosition += forward.Normal * MovementSpeed;
		// WorldRotation = WorldRotation.RotateAroundAxis(Vector3.Up, Input.AnalogMove.y * 1.0f);
		WorldRotation = TurnAxis.WorldRotation.RotateAroundAxis( Vector3.Up, Input.AnalogMove.y * 1.0f );
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




		// Camera.WorldRotation = WorldRotation;
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

	// Define properties for advanced vehicle physics
	[Property] public float EnginePower { get; set; } = 1500f; // Force applied for acceleration
	[Property] public float MaxSpeed { get; set; } = 2200f; // Maximum speed in units per second
	[Property] public float TurnSpeed { get; set; } = 70f; // Base degrees per second
	[Property] public float BrakeForce { get; set; } = 6000f; // Braking force
	[Property] public float Friction { get; set; } = 0.96f; // Road friction
	[Property] public float Drag { get; set; } = 0.99f; // Air resistance
	[Property] public float SuspensionStrength { get; set; } = 300f; // Suspension strength
	[Property] public float SuspensionDamping { get; set; } = 50f; // Suspension damping

	private float currentSpeed = 0f;
	private Vector3 velocity = Vector3.Zero;
	private Vector3 angularVelocity = Vector3.Zero;
	private Vector3 suspensionOffset = Vector3.Zero;

	public void PositionTruck()
	{

		// Input handling
		var forwardInput = Input.AnalogMove.x;
		var turnInput = (currentSpeed >= 0) ? Input.AnalogMove.y : Input.AnalogMove.y * -1; // Invert turning when going backwards.

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

		// Apply speed-dependent turning
		if ( Math.Abs( currentSpeed ) > 10f )
		{
			float speedFactor = (1 - (Math.Abs( currentSpeed ) / MaxSpeed)).Clamp( 0.2f, 1f ); // Turn efficiency based on speed
			var turnAngle = turnInput * TurnSpeed * speedFactor * Time.Delta;
			angularVelocity = new Vector3( 0, turnAngle, 0 );
			WorldRotation *= Rotation.FromYaw( turnAngle );
			// Camera.WorldRotation *= Rotation.FromYaw(turnAngle);
		}

		// Simulate suspension
		suspensionOffset = Vector3.Lerp( suspensionOffset, Vector3.Zero, SuspensionDamping * Time.Delta );

		// Update velocity and position
		velocity = WorldRotation.Forward * currentSpeed;
		velocity *= Friction; // Road friction

		// Apply gravity
		// velocity -= Vector3.Up * Gravity * Time.Delta;

		WorldPosition += velocity * Time.Delta + suspensionOffset;

		// Apply braking
		if ( Input.Pressed( "jump" ) ) // Example: Space for braking
		{
			currentSpeed = MathX.Lerp( currentSpeed, 0, BrakeForce * Time.Delta );
		}

	}
}