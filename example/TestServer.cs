using Godot;
using Massive.Netcode;
using Ruffles.Channeling;
using Ruffles.Configuration;
using Ruffles.Simulation;

namespace massivegodotintegration.example;

public partial class TestServer : Node {
	public Server Server;

	public override void _Ready() {
		/*
		var transport = new UdpServerTransport(
			new SocketConfig {
				ChallengeDifficulty = 20, // Difficulty 20 is fairly hard
				ChannelTypes = [
					ChannelType.Reliable
				],
				DualListenPort = 1987,
				SimulatorConfig = new SimulatorConfig {
					DropPercentage = 0.05f,
					MaxLatency = 10,
					MinLatency = 0
				},
				UseSimulator = false 
			}
		);
		*/
		var transport = new TcpServerTransport(1987);
		Server = new Server(transport, new SessionConfig());
		Server.Start();
	}

	public override void _PhysicsProcess(double delta) {
		Server.Update(delta);
	}
}