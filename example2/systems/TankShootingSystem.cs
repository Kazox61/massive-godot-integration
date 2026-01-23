using Fixed64;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;
using massivegodotintegration.example.input;

namespace massivegodotintegration.example2;

public class TankShootingSystem : RetroTankSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity entity, ref Tank tank) => {
			var playerInput = Inputs.Get<PlayerInput>(tank.InputChannel).LastActual();
			var direction = new FVector2(playerInput.AimX.ToFP(), playerInput.AimY.ToFP());
			if (FVector2.LengthSqr(direction) > FP.Zero) {
				tank.GunDirection = new FVector2(playerInput.AimX.ToFP(), playerInput.AimY.ToFP());
			}
			
			if (playerInput.Attack) {
				var bullet = World.CreateEntity();
				bullet.Set(new Bullet {
					OwnerId = entity.Id,
					Lifetime = 120
				});
				bullet.Set(new Transform2d {
					Position = entity.Get<Transform2d>().Position,
					Rotation = FMath.Atan2(tank.GunDirection.X, -tank.GunDirection.Y)
				});
				bullet.Set(new Velocity2d {
					Linear = FVector2.Normalize(tank.GunDirection) * 10.ToFP()
				});
				bullet.Set(new ViewAsset { PackedScenePath = $"res://example2/assets/bullet_{tank.Color}.tscn"});
			}
		});
	}
}