using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.Components;
using Mathematics.Fixed;

namespace massivegodotintegration.addons.massive_godot_integration.systems;

public class PhysicsGravitySystem : NetSystem, IUpdate {
	private static readonly FVector3 Gravity = new(FP.Zero, (-9.81f).ToFP(), FP.Zero);

	public void Update() {
		World.ForEach((ref RigidBody body) => {
			if (body.InverseMass == FP.Zero) {
				return;
			}

			body.Velocity += Gravity * FP.One / Session.Config.TickRate.ToFP();
		});
	}
}