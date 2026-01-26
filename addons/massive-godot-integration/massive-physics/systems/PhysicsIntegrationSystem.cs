using Massive;
using Massive.Netcode;
using Massive.Physics;
using Fixed64;
using massivegodotintegration.addons.massive_godot_integration;

namespace Massive.Physics;

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