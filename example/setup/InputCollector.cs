using Godot;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.example.input;

namespace massivegodotintegration.example;

public class InputCollector : IInputCollector {
	public PlayerInput CollectInput() {
		var inputDirection = Input.GetVector("left", "right", "up", "down").Normalized();
		var kill = Input.IsActionJustPressed("ui_cancel");
		var jump = Input.IsActionJustPressed("jump");
		var attack = Input.IsActionJustPressed("attack");
		return new PlayerInput {
			DirectionX = inputDirection.X,
			DirectionY = inputDirection.Y,
			Kill = kill,
			Jump = jump,
			Attack = attack
		};
	}
}