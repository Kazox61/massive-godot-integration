using Godot;

namespace massivegodotintegration.Example.Input;

public partial class InputCollector : Node {
	[Export] public TestWorld TestWorld;
	
	public override void _PhysicsProcess(double delta) {
		var inputDirection = Godot.Input.GetVector("left", "right", "up", "down").Normalized();
		var kill = Godot.Input.IsActionJustPressed("ui_cancel");
		TestWorld.Session.Inputs.SetAt(TestWorld.TargetTick, 0, new PlayerInput { DirectionX = inputDirection.X, DirectionY = inputDirection.Y, Kill = kill});
	}
}