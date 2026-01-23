using Fixed64;
using Massive;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.example2;

public class RetroTankSetup : IGameSetup {
	public MassiveRandom Random { get; private set; }
	
	public void SetupGame(MassiveSystems systems, MassiveWorld world, uint seed, int localInputChannel) {
		Random = new MassiveRandom(128, seed);
		systems
			.New<VelocityUpdateSystem>()
			.New<TankMovementSystem>()
			.New<TankShootingSystem>()
			.New<BulletLifetimeSystem>();

		systems.Inject(this);
		
		var player1 = world.CreateEntity();
		player1.Set(new Tank { InputChannel = 0, GunDirection = FVector2.Down });
		player1.Set(new Transform2d { Position = new FVector2(Random.NextInt(-512, -256).ToFP(), Random.NextInt(-512, 512).ToFP()) });
		player1.Set(new ViewAsset { PackedScenePath = "res://example2/assets/tank.tscn"});
		
		var player2 = world.CreateEntity();
		player2.Set(new Tank { InputChannel = 1, GunDirection = FVector2.Down });
		player2.Set(new Transform2d { Position = new FVector2(Random.NextInt(256, 512).ToFP(), Random.NextInt(-512, 512).ToFP()) });
		player2.Set(new ViewAsset { PackedScenePath = "res://example2/assets/tank.tscn"});
	}
}