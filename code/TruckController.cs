using System;
using static Sandbox.Component;

namespace scraprunners;

public class TruckController : Component, IPressable
{
	[Property, Feature( "General" )] public Vector3 ThirdPersonCamOffsets;
	[Property, Feature( "General" )] private bool IsFirstPerson = true;

	[Property, Feature( "Stats" )] public float EnginePower { get; set; } = 100f; // Force applied for acceleration
	[Property, Feature( "Stats" )] public float MaxSpeed { get; set; } = 500f; // Maximum speed in units per second
	[Property, Feature( "Stats" )] public float TurnSpeed { get; set; } = 100f; // Base degrees per second
	[Property, Feature( "Stats" )] public float TurnHandling { get; set; } = 0.05f; // Base degrees per second
	[Property, Feature( "Stats" )] public float TurnCentering { get; set; } = 0.05f; // Base degrees per second
	[Property, Feature( "Stats" )] public float BrakeForce { get; set; } = 1f; // Braking force
	[Property, Feature( "Stats" )] public float Friction { get; set; } = 1f; // Road friction
	[Property, Feature( "Stats" )] public float Drag { get; set; } = 0.975f; // Air resistance
	[Property, Feature( "Stats" )] public float SuspensionStrength { get; set; } = 100f; // Suspension strength
	[Property, Feature( "Stats" )] public float SuspensionDamping { get; set; } = 100f; // Suspension damping

	[Property, Feature( "References" )] public GameObject FirstPersonCam;
	[Property, Feature( "References" )] public GameObject ExitPosition;
	[Property, Feature( "References" )] public List<GameObject> Weapons;
	[Property, Feature( "References" )] public PanelComponent Hud;

	private float currentSpeed = 0f;
	private Vector3 Velocity = Vector3.Zero;

	private float TurnDirection = 0f;


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
				HandleFireInput();
				HandleUseInput();
				HandleViewInput();
			}

			PositionTruck();
			PositionWeapons();

			AddDebugOverlays();
		}

		// Do nothing without a driver!
	}

	private void HandleFireInput()
	{
		if ( Input.Pressed( "attack1" ) )
		{
			DebugLog( "Fire" );
		}
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

		thirdPersonDebugPos = thirdPersonDebugPos + WorldRotation.Forward * 200;
		DebugOverlay.Line( thirdPersonDebugPos + WorldRotation.Left * TurnSpeed, thirdPersonDebugPos + WorldRotation.Right * TurnSpeed, Color.White );
		DebugOverlay.Sphere( new Sphere( thirdPersonDebugPos + WorldRotation.Left * TurnDirection, 2f ) );
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

		Hud.Enabled = true;

		// Wait();
	}

	protected void DriverExit()
	{
		Driver.WorldPosition = ExitPosition.WorldPosition;
		Driver.WorldRotation = Camera.WorldRotation;
		Driver.Enabled = true;

		// Reset Weapons
		foreach ( GameObject weapon in Weapons )
		{
			weapon.WorldRotation = WorldRotation;
		}

		Hud.Enabled = false;

		HasDriver = false;
	}

	protected void DebugLog( string msg )
	{
		Log.Info( this + ": " + msg );
	}

	public void PositionTruck()
	{
		DebugLog($"{TurnDirection}");

		// Get inputs from AnalogMove for controller support.
		var forwardInput = Input.AnalogMove.x;
		var turnInput = (currentSpeed >= 0) ? Input.AnalogMove.y : Input.AnalogMove.y * -1; // Invert turning when going backwards.  

		// Calculate the "turn direction", a moving scale of how hard the player is turning left or right.
		TurnDirection = Math.Clamp( TurnDirection + turnInput * TurnHandling, -1 * TurnSpeed, TurnSpeed ); // Invert turning when going backwards.



		// Simulate forward speed. 
		// Forward/backward based on engine power if pressing foward or backward.
		// Slowly decelerate based on drag otherwise.
		if ( forwardInput != 0 )
		{
			currentSpeed += forwardInput * EnginePower * Time.Delta;
		}
		else
		{
			// Natural deceleration due to drag and friction
			currentSpeed *= Drag;
		}

		// Clamp simulated speed to maximum value.
		currentSpeed = currentSpeed.Clamp( -MaxSpeed, MaxSpeed );

		// Update velocity/position based on current speed.
		Velocity = WorldRotation.Forward * currentSpeed;
		Velocity *= Friction; // Road friction
		WorldPosition += Velocity * Time.Delta;

		// Now simulate turning. Don't allow turning while stationary (under 10.0 speed)
		if ( Math.Abs( currentSpeed ) > 10f )
		{
			var turnAngle = TurnDirection * Time.Delta;

			WorldRotation *= Rotation.FromYaw( turnAngle );
			EyeAngles += Rotation.FromYaw( turnAngle ).Angles();
		}
		else // not moving
		{
			TurnDirection = MathX.Lerp( TurnDirection, 0f, 0.3f, true );
		}

		// Simulate braking.
		if ( Input.Down( "jump" ) ) // Example: Space for braking
		{
			currentSpeed = MathX.Lerp( currentSpeed, 0, BrakeForce * Time.Delta );
		}

		// Simulate turn direction always leveling out back to zero like real steering.

		// If the turn centering overshoots 0 then it will infinitely bounce between the lowest non-zero left/right turn direction value.
		// If this is detected, just clamp the value to 0 and be done with it.
		if (turnInput == 0) // Not actively turning left/right
		{
			if ( TurnDirection > 0 ) // Turning Left
			{

				var turnCentered = TurnDirection - TurnCentering;
				bool calcWouldReverseTurn = turnCentered < 0;
				TurnDirection = calcWouldReverseTurn ? 0 : turnCentered;

			}
			else // Turning Right
			{
				var turnCentered = TurnDirection + TurnCentering;
				bool calcWouldReverseTurn = turnCentered > 0;
				TurnDirection = calcWouldReverseTurn ? 0 : turnCentered;
			}
		}
	}
}