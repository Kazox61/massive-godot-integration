using Fixed64;
using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.components;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;
using massivegodotintegration.addons.massive_godot_integration.systems;
using massivegodotintegration.example.components;
using massivegodotintegration.example.input;
using massivegodotintegration.example.systems;

namespace massivegodotintegration.example;

public partial class TestClient : Node {
	[Export] private MassiveStats _massiveStats;

	public Session Session => Client.Session;

	public GodotEntitySynchronization GodotEntitySynchronization;
	public readonly SimulationTickTracker TickTracker = new();
	
	
	public Client2 Client;

	private float _clientTime;

	public override void _Ready() {
		/*
		var clientConfig = new SocketConfig {
			ChallengeDifficulty = 20, // Difficulty 20 is fairly hard
			ChannelTypes = [
				ChannelType.Unreliable
			],
			DualListenPort = 0, // Port 0 means we get a port by the operating system
			SimulatorConfig = new SimulatorConfig {
				DropPercentage = 0.05f,
				MaxLatency = 10,
				MinLatency = 0
			},
			UseSimulator = true
		};
		var transport = new UdpClientTransport(clientConfig);
		*/
		var transport = new TcpTransportClient("127.0.0.1", 1987);
		Client = new Client2(transport, new SessionConfig());
		Client.Connect();
		CreateGame();
	}

	private void CreateGame() {
		_massiveStats.Initialize(Session.World);

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

		var player1 = Session.World.CreateEntity(new Player { InputChannel = 0 });
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
		
		var player2 = Session.World.CreateEntity(new Player { InputChannel = 1 });
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
			TargetEntity = player1,
			Offset = new FVector3(FP.Zero, 8.ToFP(), 8.ToFP())
		});
		camera.Set(new ViewAsset { PackedScenePath = "res://example/camera.tscn" });
		camera.Set(new Transform { Rotation = new FVector3(-45.ToFP() * FP.Deg2Rad, FP.Zero, FP.Zero) });
		
		Session.World.SaveFrame();

		GodotEntitySynchronization = new GodotEntitySynchronization(Session.World);
		GodotEntitySynchronization.SubscribeViews();
	}

	public override void _PhysicsProcess(double delta) {
		_clientTime += (float)delta;
		
		var inputDirection = Input.GetVector("left", "right", "up", "down").Normalized();
		var kill = Input.IsActionJustPressed("ui_cancel");
		var jump = Input.IsActionJustPressed("jump");
		var attack = Input.IsActionJustPressed("attack");
		Client.Session.Inputs.SetAt(
			Client.TargetTick, 
			Client.LocalInputChannel, 
			new PlayerInput {
				DirectionX = inputDirection.X, 
				DirectionY = inputDirection.Y, 
				Kill = kill,
				Jump = jump,
				Attack = attack
			}
		);
		
		Client.Update(_clientTime);
		GodotEntitySynchronization.SynchronizeViews();
	}
}