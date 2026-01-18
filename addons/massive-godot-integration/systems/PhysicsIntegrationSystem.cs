using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.components;
using Fixed64;

namespace massivegodotintegration.addons.massive_godot_integration.systems;

public class PhysicsIntegrationSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((ref Transform transform, ref RigidBody body) => {
			if (body.InverseMass == FP.Zero) {
				return;
			}

			transform.Position += body.Velocity * (FP.One / Session.Config.TickRate.ToFP());
		});
	}
}