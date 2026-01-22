using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using Massive.Physics;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;
using massivegodotintegration.example.components;
using massivegodotintegration.example.input;
using Fixed64;

namespace massivegodotintegration.example.systems;

public class PlayerAttackSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity entity, ref Player player, ref Transform transform) => {
			var playerInput = Inputs.Get<PlayerInput>(player.InputChannel).LastActual();
			
			/*
			if (playerInput.Attack) {
				var projectile = World.CreateEntity();
				projectile.Set(new Transform {
					Position = transform.Position + new FVector3(0.ToFP(), 0.5f.ToFP(), 0.ToFP()),
				});
				
				var yaw = transform.Rotation.Y;
				var pitch = transform.Rotation.X;
				var cosPitch = FMath.Cos(pitch);
				var direction = FVector3.Normalize(new FVector3(
					FMath.Sin(yaw) * cosPitch,
					-FMath.Sin(pitch),
					FMath.Cos(yaw) * cosPitch
				));
				
				projectile.Set(new RigidBody {
					Velocity = direction * 20.ToFP(),
					InverseMass = 1.ToFP()
				});
				projectile.Set(new ViewAsset { PackedScenePath = "res://example/projectile.tscn" });
			}
			*/
		});
	}
}