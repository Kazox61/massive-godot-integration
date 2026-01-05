using Godot;

namespace massivegodotintegration.example.input;

public partial class InputCollector : Node {
	[Export] public TestWorld TestWorld;
	
	public override void _PhysicsProcess(double delta) {
		var inputDirection = Input.GetVector("left", "right", "up", "down").Normalized();
		var kill = Input.IsActionJustPressed("ui_cancel");
		var jump = Input.IsActionJustPressed("jump");
		var attack = Input.IsActionJustPressed("attack");
		TestWorld.Session.Inputs.SetAt(
			TestWorld.TargetTick, 
			0, 
			new PlayerInput {
				DirectionX = inputDirection.X, 
				DirectionY = inputDirection.Y, 
				Kill = kill,
				Jump = jump,
				Attack = attack
			}
		);
	}
}