using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.components;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;
using massivegodotintegration.addons.massive_godot_integration.systems;
using massivegodotintegration.example.components;
using massivegodotintegration.example.systems;
using Mathematics.Fixed;

namespace massivegodotintegration.example;

public partial class TestWorld : Node3D {
	public Session Session;

	public int TargetTick;
	public GodotEntitySynchronization GodotEntitySynchronization;
	public readonly SimulationTickTracker TickTracker = new();

	public override void _Ready() {
		Session = new Session();

		Session.Systems
			.New<PhysicsGravitySystem>()
			.New<PhysicsIntegrationSystem>()
			.New<PhysicsBroadPhaseSystem>()
			.New<PhysicsNarrowPhaseSystem>()
			.New<PhysicsSolveSystem>()
			.New<MovementSystem>()
			.New<CameraFollowSystem>()
			.New<PlayerAttackSystem>()
			.Build(Session.World)
			.Inject(Session);

		Session.Simulations.Add(new SystemsSimulation(Session.Systems));
		Session.Simulations.Add(TickTracker);

		var floor = Session.World.CreateEntity();
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

		var player = Session.World.CreateEntity(new Player { InputChannel = 0 });
		player.Set(new Transform { Position = new FVector3(6.ToFP(), 10.ToFP(), 0.ToFP()) });
		player.Set(new ViewAsset { PackedScenePath = "res://example/player.tscn" });
		player.Set(new RigidBody {
			Velocity = FVector3.Zero,
			InverseMass = FP.One,
			Restitution = FP.Zero,
			Friction = 0.5f.ToFP(),
			UseGravity = true
		});
		player.Set(new BoxCollider {
			HalfExtents = new FVector3(0.5f.ToFP(), 0.8f.ToFP(), 0.5f.ToFP())
		});
		
		var enemy = Session.World.CreateEntity();
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
		
		var camera = Session.World.CreateEntity(new Camera());
		camera.Set(new CameraTarget {
			TargetEntity = player,
			Offset = new FVector3(FP.Zero, 8.ToFP(), 8.ToFP())
		});
		camera.Set(new ViewAsset { PackedScenePath = "res://example/camera.tscn" });
		camera.Set(new Transform { Rotation = new FVector3(-45.ToFP() * FP.Deg2Rad, FP.Zero, FP.Zero) });
		
		Session.World.SaveFrame();

		GodotEntitySynchronization = new GodotEntitySynchronization(Session.World);
		GodotEntitySynchronization.SubscribeViews();
	}

	public override void _PhysicsProcess(double delta) {
		// Randomize Rollback Ticks
		// Session.ChangeTracker.NotifyChange(MathUtils.Max(0, TargetTick - Random.Shared.Next(0, 10)));
		TickTracker.Restart();
		Session.Loop.FastForwardToTick(TargetTick);
		// GD.Print($"Ticks Processed This Frame: {TickTracker.TicksAmount}");
		GodotEntitySynchronization.SynchronizeViews();
		TargetTick++;
	}
}