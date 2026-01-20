using System;
using System.Threading.Tasks;

namespace Massive.Netcode;

public class InputReceiver : IInputReceiver {
	private Client2 _client2;
	
	public InputReceiver(Client2 client2) {
		_client2 = client2;
	}
	
	public void SetInputAt<T>(int tick, int channel, T input) where T : IInput
	{
		if (channel != _client2.LocalInputChannel) {
			return;
		}

		// Don't send received input.
		if (tick <= _client2.ApprovedTick) {
			return;
		}

		if (_client2.TransportClient.IsConnected) {
			var messageBytes = _client2.MessageSerializer.CreateBytes(
				new InputMessage {
					Tick = tick,
					Input = (IInput)input
				}
			);
			_client2.TransportClient.Socket.Send(messageBytes);
		}
	}

	public void SetInputsAt<T>(int tick, AllInputs<T> allInputs) where T : IInput {
		throw new System.NotImplementedException();
	}
	public void ApplyEventAt<T>(int tick, int localOrder, T data) where T : IEvent
	{
		throw new System.NotImplementedException();
	}
	public void ApplyEventsAt<T>(int tick, AllEvents<T> allEvents) where T : IEvent {
		throw new System.NotImplementedException();
	}
}