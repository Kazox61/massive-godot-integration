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
		TickSync = new TickSync(sessionConfig.TickRate, sessionConfig.RollbackTicksCapacity);
	}

	public void Connect() {
		TransportClient.Connect();
	}

	public void Disconnect() { }

	public void Update(float clientTime) {
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
						ApprovedTick = tickSyncMessage2.ApprovedTick;
						ServerTime = tickSyncMessage2.ServerTime;
						
						TickSync.UpdateTimeSync(ServerTime, clientTime);
						
						foreach (var (tick, inputChannel, input) in tickSyncMessage2.Inputs) {
							// Skip local input; it was already applied when sent, it will be an infinite loop otherwise
							if (inputChannel == LocalInputChannel) {
								continue;
							}
							
							Session.Inputs.SetAt(tick, inputChannel, (PlayerInput)input); // not sure why but the cast is necessary
						}
						
						break;
					case PongMessage pongMessage:
						var rtt = clientTime - pongMessage.ClientStartTime;
						TickSync.UpdateRTT(rtt);
						break;
				}
			}
		}

		
		TickSync.ApproveSimulationTick(ApprovedTick);

		TargetTick = TickSync.CalculateTargetTick(clientTime);
		Session.Loop.FastForwardToTick(TargetTick);
	}
}