using System.Net;
using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.addons.massive_godot_integration;

public partial class MassiveGodotRunner<TGame, TInputCollector> : Node where TGame : IGameSetup, new() where TInputCollector : IInputCollector, new() {
	[Export] public string IpAddress = "127.0.0.1";
	[Export] public int Port = 1987;
	[Export] public MassiveStats MassiveStats;
	
	public Client2 Client { get; private set; }
	public GodotEntitySynchronization GodotEntitySynchronization { get; private set; }
	public float ClientTime { get; private set; }
	public IGameSetup GameSetup { get; private set; }
	public IInputCollector InputCollector { get; private set; }
	
	public Session Session => Client.Session;

	public override void _Ready() {
		var endPoint = new IPEndPoint(IPAddress.Parse(IpAddress), Port);
		var transport = new TcpTransportClient(endPoint);
		Client = new Client2(transport, new SessionConfig());
		Client.Connect();
		
		GameSetup = new TGame();
		InputCollector = new TInputCollector();
		
		GameSetup.SetupGame(Session.Systems, Session.World);

		Session.Systems
			.Build(Session.World)
			.Inject(Session);
		
		Session.Simulations.Add(new SystemsSimulation(Session.Systems));
		
		Session.World.SaveFrame();

		GodotEntitySynchronization = new GodotEntitySynchronization(Session.World);
		GodotEntitySynchronization.SubscribeViews();

		MassiveStats?.Initialize(Session);
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
		GodotEntitySynchronization.SynchronizeViews();
	}
}