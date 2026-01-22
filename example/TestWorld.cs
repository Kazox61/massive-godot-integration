using System;
using BepuPhysics;
using BepuPhysics.Collidables;
using Fixed64;
using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;
using Massive.Physics;
using massivegodotintegration.example.components;
using massivegodotintegration.example.systems;
using Transform = Massive.Physics.Transform;
using Vector3 = System.Numerics.Vector3;

namespace massivegodotintegration.example;

public partial class TestWorld : Node3D {
	[Export] private MassiveStats _massiveStats;
	
	public Session Session;
	public PhysicsWorld PhysicsWorld;

	public int TargetTick;
	public GodotEntitySynchronization GodotEntitySynchronization;
	public readonly SimulationTickTracker TickTracker = new();

	public override void _Ready() {
		Session = new Session();
		PhysicsWorld = new PhysicsWorld();
		
		/*
		Session.World.DataSet<PhysicsBody>().AfterAdded += (entityId) => {
			var entity = Session.World.GetEntity(entityId);
			ref var physicsBody = ref entity.Get<PhysicsBody>();

			var sphere = new Sphere(1);
			var sphereInertia = sphere.ComputeInertia(1);
			var bodyHandle = PhysicsWorld.Simulation.Bodies.Add(
				BodyDescription.CreateDynamic(
					new Vector3(0, 10, 0), 
					sphereInertia,
					PhysicsWorld.Simulation.Shapes.Add(sphere), 
					0.01f
				)
			);
			
			physicsBody.Handle = bodyHandle;
		};
		*/
		Session.World.DataSet<PhysicsBody>().BeforeRemoved += (entityId) => {
			var entity = Session.World.GetEntity(entityId);
			var physicsBody = entity.Get<PhysicsBody>();
			if (physicsBody.IsStatic) {
				PhysicsWorld.Simulation.Statics.Remove(physicsBody.StaticHandle);
			}
			else {
				PhysicsWorld.Simulation.Bodies.Remove(physicsBody.BodyHandle);
			}
		};
		
		_massiveStats.Initialize(Session.World);

		Session.Systems
			.New<PhysicsStepSystem>()
			.New<SyncPhysicsWorldSystem>()
			.New<MovementSystem>()
			.New<CameraFollowSystem>()
			.New<PlayerAttackSystem>()
			.Build(Session.World)
			.Inject(Session)
			.Inject(PhysicsWorld);

		Session.Simulations.Add(new SystemsSimulation(Session.Systems));
		Session.Simulations.Add(TickTracker);
		
		
		var floorHandle = PhysicsWorld.Simulation.Statics.Add(
			new StaticDescription(
				new Vector3(0, -1, 0), 
				PhysicsWorld.Simulation.Shapes.Add(new Box(20f, 2f, 20f))
			)
		);

		var floorReference = PhysicsWorld.Simulation.Statics[floorHandle];
		
		var floor = Session.World.CreateEntity();
		floor.Set(new Transform {
			Position = floorReference.Pose.Position
		});
		floor.Set(new PhysicsBody {
			IsStatic = true, 
			StaticHandle = floorHandle
		});
		
		var playerBox = new Box(1f, 1.6f, 1f);
		var playerBoxInertia = playerBox.ComputeInertia(1);
		var playerHandle = PhysicsWorld.Simulation.Bodies.Add(
			BodyDescription.CreateDynamic(
				new Vector3(6, 20, 0), 
				playerBoxInertia,
				PhysicsWorld.Simulation.Shapes.Add(playerBox), 
				0.01f
			)
		);
		
		var playerReference = PhysicsWorld.Simulation.Bodies[playerHandle];
		
		var player = Session.World.CreateEntity(new Player { InputChannel = 0 });
		player.Set(new Transform { Position = playerReference.Pose.Position });
		player.Set(new ViewAsset { PackedScenePath = "res://example/player.tscn" });
		player.Set(new PhysicsBody { BodyHandle = playerHandle });
		
		var enemyHandle = PhysicsWorld.Simulation.Statics.Add(
			new StaticDescription(
				new Vector3(0, 1, 0), 
				PhysicsWorld.Simulation.Shapes.Add(new Box(1f, 2f, 1f))
			)
		);
		
		var enemyReference = PhysicsWorld.Simulation.Statics[enemyHandle];
		
		var enemy = Session.World.CreateEntity();
		enemy.Set(new Transform { Position = enemyReference.Pose.Position });
		enemy.Set(new ViewAsset { PackedScenePath = "res://example/enemy.tscn" });
		enemy.Set(new PhysicsBody {
			IsStatic = true,
			StaticHandle = enemyHandle
		});
		
		var camera = Session.World.CreateEntity(new Camera());
		camera.Set(new CameraTarget {
			TargetEntity = player,
			Offset = new FVector3(FP.Zero, 8.ToFP(), 8.ToFP())
		});
		camera.Set(new ViewAsset { PackedScenePath = "res://example/camera.tscn" });
		var pitch = (-45.ToFP() * FP.Deg2Rad).ToFloat();
		var yaw = 0f;
		var roll = 0f;

		camera.Set(
			new Transform {
				Rotation = System.Numerics.Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll)
			} 
		);
		
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