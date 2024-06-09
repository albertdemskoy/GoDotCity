using Godot;

public partial class View : Node3D {
	public Vector3 CameraPosition {get; set;}
	public Vector3 CameraRotation {get; set;}
	public Camera3D Camera { get; set;}

	public override void _Ready()
	{
		Camera = GetNode<Camera3D>("Camera");
		CameraRotation = RotationDegrees;
	}

	public override void _Process(double delta)
	{
		var fDelta = (float)delta;
	   	Position = Position.Lerp(CameraPosition, (float)(fDelta * 8));
		RotationDegrees = RotationDegrees.Lerp(CameraRotation, (float)(fDelta * 6));
		HandleInput(fDelta);
	}


	public void HandleInput(float delta) {
		// Rotation	
		var userInput = Vector3.Zero;
			
		userInput.X = Input.GetAxis("camera_left", "camera_right");
		userInput.Z = Input.GetAxis("camera_forward", "camera_back");
		userInput = userInput.Rotated(Vector3.Up, Rotation.Y).Normalized();
			
		CameraPosition += userInput / 4;
			
		// 	# Back to center
		if (Input.IsActionPressed("camera_center"))
		{
			CameraPosition = new Vector3();
		}
	}
	
	public override void _Input(InputEvent inputEvent)
	{	
		if (inputEvent is InputEventMouseMotion motion && Input.IsActionPressed("camera_rotate")) {

			CameraRotation += new Vector3(0, -motion.Relative.X / 10, 0);
		}       
	}
}
