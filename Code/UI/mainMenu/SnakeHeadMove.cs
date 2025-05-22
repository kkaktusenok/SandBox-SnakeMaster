using Sandbox;
using System.Diagnostics;
using System.Numerics;

public sealed class SnakeHeadMove : Component
{
	protected override void OnUpdate()
	{
		var raty = Scene.Camera.ScreenPixelToRay(Mouse.Position);
		var tr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 450f ).Run();
		LocalRotation = Rotation.LookAt( WorldPosition - tr.EndPosition );
	}
}
