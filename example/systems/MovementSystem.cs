using System;
using Fixed64;
using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using Massive.Physics;
using massivegodotintegration.example.components;
using massivegodotintegration.example.input;
using Vector3 = System.Numerics.Vector3;

namespace massivegodotintegration.example.systems;

public class MovementSystem : PhysicsSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity entity, ref Player player, ref Transform transform, ref PhysicsBody physicsBody) => {
			var playerInput = Inputs.Get<PlayerInput>(player.InputChannel).LastActual();
			var moveDir = new FVector3(playerInput.DirectionX.ToFP(), FP.Zero, playerInput.DirectionY.ToFP());

			if (moveDir != FVector3.Zero) {
				moveDir = FVector3.Normalize(moveDir);
			}
			
			var bodyReference = PhysicsWorld.Simulation.Bodies[physicsBody.BodyHandle];
			
			if (FVector3.LengthSqr(moveDir) > 0.0001f.ToFP()) {
				var targetRotation = System.Numerics.Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.Atan2(moveDir.X.ToFloat(), moveDir.Z.ToFloat()));
				bodyReference.Pose.Orientation = targetRotation;
			}

			var moveSpeed = 2.ToFP();

			// Calculate target velocity (horizontal only)
			var targetVelocity = new Vector3(
				(moveDir.X * moveSpeed).ToFloat(),
				0f,
				(moveDir.Z * moveSpeed).ToFloat()
			);

			// Get current horizontal velocity
			var currentHorizontalVelocity = bodyReference.Velocity.Linear with { Y = 0f };

			// Calculate velocity change needed
			var velocityChange = targetVelocity - currentHorizontalVelocity;

			// Wake up the body if needed
			if (!bodyReference.Awake && velocityChange != Vector3.Zero) {
				PhysicsWorld.Simulation.Awakener.AwakenBody(physicsBody.BodyHandle);
			}

			// Apply impulse to achieve target velocity (preserves Y velocity for gravity)
			bodyReference.ApplyLinearImpulse(velocityChange / bodyReference.LocalInertia.InverseMass);
			
			if (moveDir != FVector3.Zero) {
				//Rotation is 
				transform.Rotation = System.Numerics.Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), MathF.Atan2(moveDir.X.ToFloat(), moveDir.Z.ToFloat()));
			}
			
			if (playerInput.Jump) {
				// Simple jump impulse
				bodyReference.ApplyLinearImpulse(new Vector3(0f, 5f, 0f) / bodyReference.LocalInertia.InverseMass);
			}

			if (playerInput.Kill) {
				entity.Destroy();
			}
		});
	}
}