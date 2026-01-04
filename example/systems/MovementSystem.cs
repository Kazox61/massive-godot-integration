using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.Components;
using massivegodotintegration.example.components;
using massivegodotintegration.example.input;
using Mathematics.Fixed;

namespace massivegodotintegration.example.systems;

public class MovementSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity entity, ref Player player, ref RigidBody rigidBody) => {
			var playerInput = Inputs.Get<PlayerInput>(player.InputChannel).LastActual();
			var moveDir = new FVector3(playerInput.DirectionX.ToFP(), FP.Zero, playerInput.DirectionY.ToFP());
			if (moveDir != FVector3.Zero) {
				moveDir = FVector3.Normalize(moveDir);
			}

			var moveSpeed = 6.ToFP();
			rigidBody.Velocity = new FVector3(
				moveDir.X * moveSpeed,
				rigidBody.Velocity.Y,
				moveDir.Z * moveSpeed
			);

			if (playerInput.Jump) {
				rigidBody.Velocity += new FVector3(
					FP.Zero,
					7.ToFP(),
					FP.Zero
				);
			}

			if (playerInput.Kill) {
				entity.Destroy();
			}
		});
	}
}