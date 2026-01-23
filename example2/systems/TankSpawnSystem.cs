using Fixed64;
using Massive;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.example2;

public class TankSpawnSystem : RetroTankSystem, IFirstTick {
	private const string Player1Color = "blue";
	private const string Player2Color = "red";
	
	public void FirstTick() {
		var player1 = World.CreateEntity();
		player1.Set(new Tank {
			InputChannel = 0, 
			Color = Player1Color,
			GunDirection = FVector2.Down
		});
		player1.Set(new Transform2d { Position = new FVector2(Random.NextInt(-256, -0).ToFP(), Random.NextInt(-256, 256).ToFP()) });
		player1.Set(new ViewAsset { PackedScenePath = $"res://example2/assets/tank_{Player1Color}.tscn"});
		
		var player2 = World.CreateEntity();
		player2.Set(new Tank {
			InputChannel = 1, 
			Color = Player2Color,
			GunDirection = FVector2.Down
		});
		player2.Set(new Transform2d { Position = new FVector2(Random.NextInt(0, 256).ToFP(), Random.NextInt(-256, 256).ToFP()) });
		player2.Set(new ViewAsset { PackedScenePath = $"res://example2/assets/tank_{Player2Color}.tscn"});
	}
}