using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;

namespace massivegodotintegration.example2;

public class VelocityUpdateSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity entity, ref Transform2d transform, ref Velocity2d velocity) => {
			transform.Position += velocity.Linear;
		});
	}
}