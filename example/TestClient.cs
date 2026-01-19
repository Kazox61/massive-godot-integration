using System;
using System.Net;
using Godot;
using Massive.Netcode;
using massivegodotintegration.example.input;
using Ruffles.Channeling;
using Ruffles.Configuration;
using Ruffles.Connections;
using Ruffles.Core;
using Ruffles.Simulation;

namespace massivegodotintegration.example;

public partial class TestClient : Node {
	public Client2 Client;
	
	private RuffleSocket _client;
	private UdpSocket _udpSocket;

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
		
		_client = new RuffleSocket(clientConfig);
		_client.Start();
		_client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5674));
	}

	public override void _Process(double delta) {
		_clientTime += (float)delta;
		
		while (true) {
			var clientEvent = _client.Poll();
			
			if (clientEvent.Type == NetworkEventType.Nothing) {
				break;
			}
			
			switch (clientEvent.Type) {
				case NetworkEventType.Connect:
					OnConnectedToServer(clientEvent.Connection);
					break;
				case NetworkEventType.Data:
					ReadOnlySpan<byte> bytes = clientEvent.Data.Array.AsSpan(clientEvent.Data.Offset, clientEvent.Data.Count);
					_udpSocket.EnqueueReceivedData(bytes);
					break;
			}
			
			clientEvent.Recycle();
		}

		if (Client == null) {
			return;
		}
		
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
	
	private void OnConnectedToServer(Connection connection) {
		GD.Print("Connected to server!");
		_udpSocket = new UdpSocket(connection);
		var unreliableConnection = new UnreliableConnection(_udpSocket);
		Client = new Client2(unreliableConnection, new SessionConfig());
	}
}