using Fixed64;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.example.input;

namespace massivegodotintegration.example2;

public class TankMovementSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((ref Tank tank, ref Transform2d transform) => {
			var playerInput = Inputs.Get<PlayerInput>(tank.InputChannel).LastActual();
			transform.Position += new FVector2(playerInput.DirectionX.ToFP(), playerInput.DirectionY.ToFP()) * 2.ToFP();
			tank.MovementDirection = new FVector2(playerInput.DirectionX.ToFP(), playerInput.DirectionY.ToFP());
		});
	}
}