using Godot;
using massivegodotintegration.example.input;

namespace Massive.Netcode;

public class Client2 {
	public Session Session { get; }

	public TickSync TickSync { get; }
	
	public ITransport Transport { get; }
	public GenericTypeLookup<IInput> InputTypeLookup { get; }

	public int ApprovedTick { get; private set; }
	public float ServerTime { get; private set; }
	
	public int TargetTick { get; private set; }
	
	public int LocalInputChannel { get; private set; }
	
	private float _lastPingTime;

	public Client2(ITransport transport, SessionConfig sessionConfig) {
		Transport = transport;
		InputTypeLookup = new GenericTypeLookup<IInput>();
		InputTypeLookup.RegisterAll();
		Session = new Session(sessionConfig, new InputReceiver(this));
		TickSync = new TickSync(sessionConfig.TickRate, sessionConfig.RollbackTicksCapacity);
	}

	public void Connect() {
		Transport.Connect();
	}

	public void Disconnect() { }

	public void Update(float clientTime) {
		if (clientTime - _lastPingTime >= 1f) {
			_lastPingTime = clientTime;
			Transport.Connection.SendMessage(new PingMessage {
				ClientStartTime = clientTime
			});
		}
		
		Transport.Update();

		if (Transport.IsConnected) {
			while (Transport.Connection.TryDequeueMessage(out var message)) {
				switch (message) {
					case TickSyncMessage2 tickSyncMessage2:
						LocalInputChannel = tickSyncMessage2.InputChannel;
						ApprovedTick = tickSyncMessage2.ApprovedTick;
						ServerTime = tickSyncMessage2.ServerTime;
						
						TickSync.UpdateTimeSync(ServerTime, clientTime);
						
						foreach (var (tick, inputChannel, input) in tickSyncMessage2.Inputs) {
							// GD.Print($"Client {LocalInputChannel} received input for tick {tick} for channel {inputChannel}");
							// Skip local input; it was already applied when sent, it will be an infinite loop otherwise
							if (inputChannel == LocalInputChannel) {
								continue;
							}
							
							Session.Inputs.SetAt(tick, inputChannel, (PlayerInput)input); // not sure why but the cast is necessary
						}
						
						break;
					case PongMessage pongMessage:
						var rtt = clientTime - pongMessage.ClientStartTime;
						GD.Print($"RTT: {rtt}");
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