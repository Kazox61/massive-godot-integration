using Godot;
using massivegodotintegration.example.input;

namespace Massive.Netcode;

public class Client2 {
	public Session Session { get; }

	public TickSync TickSync { get; }
	
	public ITransportClient TransportClient { get; }
	public GenericTypeLookup<IInput> InputTypeLookup { get; }

	public int ApprovedTick { get; private set; }
	public float ServerTime { get; private set; }
	
	public int TargetTick { get; private set; }
	
	public int LocalInputChannel { get; private set; }
	
	public readonly MessageSerializer MessageSerializer = new();
	
	private float _lastPingTime;

	public Client2(ITransportClient transportClient, SessionConfig sessionConfig) {
		TransportClient = transportClient;
		InputTypeLookup = new GenericTypeLookup<IInput>();
		InputTypeLookup.RegisterAll();
		Session = new Session(sessionConfig, new InputReceiver(this));
		TickSync = new TickSync(sessionConfig.TickRate, sessionConfig.RollbackTicksCapacity, 4);
	}

	public void Connect() {
		TransportClient?.Connect();
	}

	public void Disconnect() { }

	public void Update(float clientTime) {
		UpdateTransport(clientTime);

		TargetTick = TickSync.CalculateTargetTick(clientTime);
		Session.Loop.FastForwardToTick(TargetTick);

		// only for singleplayer
		if (TransportClient == null) {
			ApprovedTick = TargetTick;
			TickSync.ApproveSimulationTick(ApprovedTick);
		}
	}

	private void UpdateTransport(float clientTime) {
		if (TransportClient == null) {
			return;
		}
		
		if (clientTime - _lastPingTime >= 1f) {
			_lastPingTime = clientTime;
			var messageBytes = MessageSerializer.CreateBytes(
				new PingMessage {
					ClientStartTime = clientTime
				}
			);
			TransportClient.Socket.Send(messageBytes);
		}
		
		TransportClient.Update();

		if (TransportClient.IsConnected) {
			while (TransportClient.Socket.TryReceive(out var payload)) {
				var message = MessageSerializer.CreateMessage(payload.ToArray());
				switch (message) {
					case TickSyncMessage tickSyncMessage2:
						LocalInputChannel = tickSyncMessage2.InputChannel;
						var lastApprovedTick = tickSyncMessage2.ApprovedTick;
						ApprovedTick = tickSyncMessage2.ApprovedTick;
						
						TickSync.ApproveSimulationTick(ApprovedTick);

						// Discard all prediction.
						Session.Inputs.GetInputSet<PlayerInput>().MakeInactualInRange(lastApprovedTick, ApprovedTick);

						foreach (var (tick, inputChannel, input) in tickSyncMessage2.Inputs)
						{
							Session.Inputs.SetAt(tick, inputChannel, (PlayerInput)input);
						}
						
						break;
					case PongMessage pongMessage:
						var rtt = clientTime - pongMessage.ClientStartTime;
						ServerTime = pongMessage.ServerTime + rtt * 0.5f;
						TickSync.UpdateTimeSync(ServerTime, clientTime);
						TickSync.UpdateRTT(rtt);
						GD.Print($"RTT: {rtt}");
						break;
				}
			}
		}
	}
}