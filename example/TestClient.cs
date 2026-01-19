using Godot;
using Massive.Netcode;
using massivegodotintegration.example.input;
using Ruffles.Channeling;
using Ruffles.Configuration;
using Ruffles.Simulation;

namespace massivegodotintegration.example;

public partial class TestClient : Node {
	public Client2 Client;

	private float _clientTime;

	public override void _Ready() {
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
		Client = new Client2(transport, new SessionConfig());
		Client.Connect();
	}

	public override void _Process(double delta) {
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
	}
}