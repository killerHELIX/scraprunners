namespace scraprunners;

public sealed class PlayerController : Component
{

	protected override void OnUpdate()
	{
		HandleUseInput();

	}

	private void HandleUseInput()
	{

		if ( Input.Down( "use" ) )
		{
			// Log.Info( SandboxPlayerController.Pressed );
		}

	}

}
