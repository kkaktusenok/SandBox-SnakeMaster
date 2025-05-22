using Sandbox;
using Sandbox.Movement;
using System;
using System.Diagnostics;
using static Sandbox.PhysicsGroupDescription.BodyPart;

public sealed class SnakeScript : Component, Component.ITriggerListener
{
	public int applesEaten { get; set; } = 0;
	public bool isAlive {  get; set; }

	private string lastMove;

	
	private Vector3 direction = Vector3.Zero;
	public int Gap = 6;
	
	[Property] public int Speed;
	[Property] public GameObject TailPart;
	[Property] public GameObject Food;
	[Property] public GameObject TailEnd; 
	[Property] public GameObject Epty;
	[Property] public SoundFile BiteSound;

	private List<GameObject> Tails = new List<GameObject> ();
	private List<Vector3> headPosition = new List<Vector3> ();
	private List<Rotation> headRotation = new List<Rotation>();

	protected override void OnStart()
	{
		//we make emmty like first elent of list otherwise first elemetn(Tails[0]) overlap with player
		base.OnStart();
		Tails.Insert( 0, Epty );
		isAlive = true;
		SpawnFood();
		var te = TailEnd.Clone();
		te.Transform.ClearInterpolation();
		Tails.Add( te );
	}

	public void SpawnFood()
	{
		//make sure that we have only one food on scene
		var parent = GameObject.Parent.Children;
		foreach ( var t in parent )
		{
			if ( t.Name.Contains( "food" ) )
			{
				t.Destroy();
			}
		}
		//create new food in random position
		var newFood = Food.Clone();
		var randomX = new Random().Next( -719, 719 ) ;
		var randomY = new Random().Next( -1014, 1010 );
		newFood.WorldPosition = new Vector3( randomX, randomY, 40 );
		newFood.Transform.ClearInterpolation();
	}

	public void OnTriggerEnter( Collider other )
	{

		if(other.Tags.Has( "food" ) )
		{
			Sound.PlayFile( BiteSound,30);
			applesEaten++;
			//remove food from scene 
			other.GameObject.Destroy();
			//clone new tail part and add it to list as. As well set up an position for it
			var clonedIt = TailPart.Clone();
			clonedIt.WorldPosition = headPosition[Tails.Count - 1];
			//Tails.Add(clonedIt );
			clonedIt.Transform.ClearInterpolation();
			Tails.Insert( 1, clonedIt );
			//creating new food
			SpawnFood();
		}
		if ( other.Tags.Has( "tail" ) && applesEaten > 1 || other.Tags.Has("Fence"))
		{
			Sandbox.Services.Stats.SetValue( "FoodEaten", applesEaten );
			isAlive = false;
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
				WorldRotation = Rotation.FromYaw( 90 );
			}
		}

		if ( Input.Pressed( "Left" ) )
		{
			if ( lastMove != "Right" )
			{
				lastMove = "Left";
				direction = new Vector3( 0, Speed, 0 );
				WorldRotation = Rotation.FromYaw(-90);
			}
		}
		if ( Input.Pressed( "Backward" ) )
		{
			if ( lastMove != "Forward" )
			{
				lastMove = "Backward";
				direction = new Vector3( -Speed, 0, 0 );
				WorldRotation = Rotation.FromYaw(0 );
			}
		}
		if ( Input.Pressed( "Forward" ) )
		{
			if ( lastMove != "Backward" )
			{
				lastMove = "Forward";
				direction = new Vector3( Speed, 0, 0 );
				WorldRotation = Rotation.FromYaw( 180 );
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

		var index = 0;
		foreach (var bd in Tails)
		{
			if ( index * Gap < headPosition.Count )
			{
				bd.WorldPosition = headPosition[index * Gap];
				bd.WorldRotation = headRotation[index * Gap];

				index++;
			}
			
		}
	}
}
