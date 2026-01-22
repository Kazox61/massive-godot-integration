using massivegodotintegration.addons.massive_godot_integration;

namespace Massive.Physics;

public class PhysicsStepSystem : PhysicsSystem, IUpdate {
	private const float DeltaTime = 1f / 60f;

	public void Update() {
		PhysicsWorld.Step(DeltaTime);
	}
}