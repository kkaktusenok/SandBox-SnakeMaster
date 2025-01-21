using Sandbox;

public sealed class TailScript : Component, Component.ITriggerListener
{
	[Property]public SnakeScript SnakeScript { get; set; }
	public void OnTriggerEnter( Collider other )
	{
		if ( other.Tags.Has("food") )
		{
			Log.Warning( "workingaaaaaaaaaaaaaa" );
			SnakeScript.SpawnFood();
			other.GameObject.Destroy();
			
			
		}
	}
}
