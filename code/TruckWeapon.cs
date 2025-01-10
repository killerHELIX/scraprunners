using Sandbox;

namespace scraprunners;
public abstract class TruckWeapon : Component
{
	[Property] public abstract string Name { get; set; }
	[Property] public abstract float FireRate { get; set; }
	public bool Waiting { get; set; } = false;
	public float SEC_TO_MS { get; set; } = 1000.0f;

	public abstract void Fire();
	protected override void OnUpdate()
	{

	}

	protected async void Wait(float ms)
	{
		Waiting = true;
		await GameTask.Delay( MathX.CeilToInt(ms) );
		Waiting = false;
	}
}
