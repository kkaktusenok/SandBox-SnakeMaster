using Sandbox;
using Sandbox.Movement;
using System;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.InteropServices.Swift;

public sealed class SnakeScript : Component, Component.ITriggerListener, Component.ICollisionListener
{
	private string lastMove;

	
	private Vector3 direction = Vector3.Zero;
	public int Gap = 5;
	
	[Property] public int Speed;
	[Property] public GameObject TailPart;
	[Property] public GameObject Food;
	[Property] public GameObject TailEnd; 
	[Property] public GameObject Epty;

	private List<GameObject> Tails = new List<GameObject> ();
	private List<Vector3> headPosition = new List<Vector3> ();
	private List<Rotation> headRotation = new List<Rotation>();

	protected override void OnStart()
	{
		//we make emmty like first elent of list otherwise first elemetn(Tails[0]) overlap with player
		base.OnStart();
		Tails.Insert(0, Epty);
	}
	public void OnTriggerEnter( Collider other )
	{

		if(other.Tags.Has( "food" ) )
		{
			//remove food from scene 
			other.GameObject.Destroy();
			//clone new tail part and add it to list as. As well set up an position for it
			var clonedIt = TailPart.Clone();
			//clonedIt.WorldPosition = this.WorldPosition;
			clonedIt.WorldPosition = headPosition[Tails.Count - 1];
			//Tails.Add(clonedIt );
			Tails.Insert(1,clonedIt );
			//creating new food
			var newFood = Food.Clone();
			var randomX = new Random().Next(-521,348);
			var randomY = new Random().Next(-990,990);
			newFood.WorldPosition = new Vector3 (randomX,randomY,30);
		}
		if(other.Tags.Has( "tail" ) )
		{
			Log.Error( "Hit" );
			this.GameObject.Destroy();
		}
	}
	protected override void OnFixedUpdate()
	{
		//Movement
		if ( Input.Pressed( "Right" ) )
		{
			if ( lastMove != "Left" )
			{
				lastMove = "Right";
				direction = new Vector3( 0, -Speed, 0 );
				WorldRotation = Rotation.FromYaw( 0 );
			}
		}

		if ( Input.Pressed( "Left" ) )
		{
			if ( lastMove != "Right" )
			{
				lastMove = "Left";
				direction = new Vector3( 0, Speed, 0 );
				WorldRotation = Rotation.FromYaw( 180 );
			}
		}
		if ( Input.Pressed( "Backward" ) )
		{
			if ( lastMove != "Forward" )
			{
				lastMove = "Backward";
				direction = new Vector3( -Speed, 0, 0 );
				WorldRotation = Rotation.FromYaw( -90 );
			}
		}
		if ( Input.Pressed( "Forward" ) )
		{
			if ( lastMove != "Backward" )
			{
				lastMove = "Forward";
				direction = new Vector3( Speed, 0, 0 );
				WorldRotation = Rotation.FromYaw( 90 );
			}
		}


		WorldPosition += direction * Time.Delta;

		headPosition.Insert( 0, WorldPosition );
		headRotation.Insert( 0, WorldRotation );
		int maxPositions = Tails.Count * Gap;
		if ( headPosition.Count > maxPositions )
		{
			headPosition.RemoveAt( headPosition.Count - 1 );
			headRotation.RemoveAt( headRotation.Count - 1 );
		}

		Log.Error( headPosition.Count );
		var index = 0;
		foreach (var bd in Tails)
		{
			bd.WorldPosition = headPosition[index * Gap];
			bd.WorldRotation = headRotation[index * Gap];

			index++;
		}
	}

}
