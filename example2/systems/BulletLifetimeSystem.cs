using Massive;
using massivegodotintegration.addons.massive_godot_integration;

namespace massivegodotintegration.example2;

public class BulletLifetimeSystem: RetroTankSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity entity, ref Bullet bullet) => {
			bullet.Lifetime--;
			if (bullet.Lifetime <= 0) {
				entity.Destroy();
			}
		});
	}
}