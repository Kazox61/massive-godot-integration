using Massive;
using massivegodotintegration.addons.massive_godot_integration;

namespace massivegodotintegration.example2;

public class VelocityUpdateSystem : RetroTankSystem, IUpdate {
	public void Update() {
		World.ForEach((ref Transform2d transform, ref Velocity2d velocity) => {
			transform.Position += velocity.Linear;
		});
	}
}