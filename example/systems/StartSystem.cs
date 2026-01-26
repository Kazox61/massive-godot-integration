using Fixed64;
using Massive;
using Massive.Netcode;
using Massive.Physics;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;
using massivegodotintegration.example.components;

namespace massivegodotintegration.example.systems;

public class StartSystem : NetSystem, IFirstTick {
	public void FirstTick() {
		var floor = World.CreateEntity();
		floor.Set(new Transform {
			Position = new FVector3(0.ToFP(), -1.ToFP(), 0.ToFP())
		});
		floor.Set(new RigidBody {
			InverseMass = FP.Zero,
			Restitution = FP.Zero,
			Friction = FP.One
		});
		floor.Set(new BoxCollider {
			HalfExtents = new FVector3(10.ToFP(), 1.ToFP(), 10.ToFP())
		});

		var player1 = World.CreateEntity(new Player { InputChannel = 0 });
		player1.Set(new Transform { Position = new FVector3(6.ToFP(), 10.ToFP(), 0.ToFP()) });
		player1.Set(new ViewAsset { PackedScenePath = "res://example/player.tscn" });
		player1.Set(new RigidBody {
			Velocity = FVector3.Zero,
			InverseMass = FP.One,
			Restitution = FP.Zero,
			Friction = 0.5f.ToFP(),
			UseGravity = true
		});
		player1.Set(new BoxCollider {
			HalfExtents = new FVector3(0.5f.ToFP(), 0.8f.ToFP(), 0.5f.ToFP())
		});
		
		var player2 = World.CreateEntity(new Player { InputChannel = 1 });
		player2.Set(new Transform { Position = new FVector3(-6.ToFP(), 10.ToFP(), 0.ToFP()) });
		player2.Set(new ViewAsset { PackedScenePath = "res://example/player.tscn" });
		player2.Set(new RigidBody {
			Velocity = FVector3.Zero,
			InverseMass = FP.One,
			Restitution = FP.Zero,
			Friction = 0.5f.ToFP(),
			UseGravity = true
		});
		player2.Set(new BoxCollider {
			HalfExtents = new FVector3(0.5f.ToFP(), 0.8f.ToFP(), 0.5f.ToFP())
		});
		
		var enemy = World.CreateEntity();
		enemy.Set(new Transform { Position = new FVector3(FP.Zero, 1.ToFP(), FP.Zero) });
		enemy.Set(new ViewAsset { PackedScenePath = "res://example/enemy.tscn" });
		enemy.Set(new RigidBody {
			Velocity = FVector3.Zero,
			InverseMass = FP.Zero,
			Restitution = FP.Zero,
			Friction = 0.5f.ToFP()
		});
		enemy.Set(new BoxCollider {
			HalfExtents = new FVector3(0.5f.ToFP(), 1f.ToFP(), 0.5f.ToFP())
		});
		
		var camera = World.CreateEntity(new Camera());
		camera.Set(new CameraTarget {
			TargetEntity = player1,
			Offset = new FVector3(FP.Zero, 8.ToFP(), 8.ToFP())
		});
		camera.Set(new ViewAsset { PackedScenePath = "res://example/camera.tscn" });
		camera.Set(new Transform { Rotation = new FVector3(-45.ToFP() * FP.Deg2Rad, FP.Zero, FP.Zero) });
	}
}