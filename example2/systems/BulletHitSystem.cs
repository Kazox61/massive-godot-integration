using Fixed64;
using Massive;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.example2;

public class BulletHitSystem : RetroTankSystem, IUpdate {
	private static FP _bulletHitBox = 8.ToFP();
	private static FP _tankHitBox = 24.ToFP();
	
	public void Update() {
		World.ForEach((Entity bulletEntity, ref Bullet bullet, ref Transform2d bulletTransform) => {
			var bullet2 = bullet;
			var bulletTransform2 = bulletTransform;

			World.ForEach((Entity targetEntity, ref Tank tank, ref Transform2d targetTransform) => {
				if (targetEntity.Id == bullet2.OwnerId) {
					return;
				}

				var delta = bulletTransform2.Position - targetTransform.Position;
				var radius = _bulletHitBox + _tankHitBox;

				if (FVector2.LengthSqr(delta) <= radius * radius) {
					var owner = World.GetEntity(bullet2.OwnerId);
					ref var ownerTank = ref owner.Get<Tank>();
					ownerTank.Score += 1;
					
					var explosion = World.CreateEntity();
					explosion.Set(new Transform2d {
						Position = bulletTransform2.Position
					});
					explosion.Set(new Animation {
						TickDuration = 60,
						CurrentTick = 0,
						DestroyOnCompletion = true
					});
					explosion.Set(new ViewAsset { PackedScenePath = "res://example2/assets/explosion.tscn" });
					
					bulletEntity.Destroy();
				}
			});
		});
	}
}