using Godot;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.example.input;

namespace massivegodotintegration.example2;

public class InputCollector : IInputCollector {
	public PlayerInput CollectInput() {
		var inputDirection = Input.GetVector("left", "right", "up", "down").Normalized();
		var aimDirection = Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down").Normalized();
		var attack = Input.IsActionJustPressed("jump");
		return new PlayerInput {
			DirectionX = inputDirection.X, 
			DirectionY = inputDirection.Y, 
			AimX = aimDirection.X,
			AimY = aimDirection.Y,
			Attack = attack
		};
	}
}