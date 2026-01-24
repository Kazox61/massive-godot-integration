using System.Net;
using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.addons.massive_godot_integration;

public partial class MassiveGodotRunner<TGame, TInputCollector> : Node where TGame : IGameSetup, new() where TInputCollector : IInputCollector, new() {
	public enum PlayMode {
		Singleplayer,
		Multiplayer
	}
	
	[Export] public MassiveStats MassiveStats;
	[Export] public PlayMode Mode = PlayMode.Multiplayer;
	
	[Export] public string IpAddress = "127.0.0.1";
	[Export] public int Port = 1987;
	
	public Client2 Client { get; private set; }
	public GodotEntitySynchronization GodotEntitySynchronization { get; private set; }
	public float ClientTime { get; private set; }
	public IGameSetup GameSetup { get; private set; }
	public IInputCollector InputCollector { get; private set; }
	
	public Session Session => Client.Session;

	public override void _Ready() {
		ITransportClient transport = null;
		if (Mode == PlayMode.Multiplayer) {
			var endPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
			transport = new TcpTransportClient(endPoint);
		}
		Client = new Client2(transport, new SessionConfig());
		
		GameSetup = new TGame();
		InputCollector = new TInputCollector();
		
		Client.Initialized += (seed, localInputChannel) => {
			GameSetup.SetupGame(Session.Systems, Session.World, seed, localInputChannel);

			Session.Systems
				.Build(Session.World)
				.Inject(Session);

			var systemsSimulation = new SystemsSimulation(Session.Systems);
		
			Session.Simulations.Add(systemsSimulation);
			
			systemsSimulation.Initialize();
		
			Session.World.SaveFrame();

			GodotEntitySynchronization = new GodotEntitySynchronization(Session.World);
			GodotEntitySynchronization.SubscribeViews();

			MassiveStats?.Initialize(Session);
		};
		
		Client.Connect();
	}

	public override void _PhysicsProcess(double delta) {
		ClientTime += (float)delta;

		var playerInput = InputCollector.CollectInput();

		Client.Session.Inputs.SetAt(
			Client.TargetTick,
			Client.LocalInputChannel,
			playerInput
		);

		Client.Update(ClientTime);
		GodotEntitySynchronization?.SynchronizeViews();
	}
}