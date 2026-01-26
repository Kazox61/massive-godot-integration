using Massive;
using Massive.Physics;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.example.systems;

namespace massivegodotintegration.example;

public class GameSetup : IGameSetup {
	public void SetupGame(MassiveSystems systems, MassiveWorld world, uint seed, int localInputChannel) {
		systems
			.New<PhysicsGravitySystem>()
			.New<PhysicsIntegrationSystem>()
			.New<PhysicsBroadPhaseSystem>()
			.New<PhysicsNarrowPhaseSystem>()
			.New<PhysicsSolveSystem>()
			.New<StartSystem>()
			.New<MovementSystem>()
			.New<CameraFollowSystem>()
			.New<PlayerAttackSystem>();
	}
}