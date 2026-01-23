using Massive;
using massivegodotintegration.addons.massive_godot_integration;

namespace massivegodotintegration.example2;

public class RetroTankSetup : IGameSetup {
	public void SetupGame(MassiveSystems systems, MassiveWorld world, uint seed, int localInputChannel) {
		systems
			.New(() => new MassiveRandom(seed))
			.New<VelocityUpdateSystem>()
			.New<AnimationUpdateSystem>()
			.New<TankSpawnSystem>()
			.New<TankMovementSystem>()
			.New<TankShootingSystem>()
			.New<BulletLifetimeSystem>();

		systems.Inject(this);
	}
}