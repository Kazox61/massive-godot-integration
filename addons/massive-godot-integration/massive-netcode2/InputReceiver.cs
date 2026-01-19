namespace Massive.Netcode;

public class InputReceiver : IInputReceiver {
	private Client2 _client2;
	
	public InputReceiver(Client2 client2) {
		_client2 = client2;
	}
	
	public void SetInputAt<T>(int tick, int channel, T input) {
		if (channel != _client2.LocalInputChannel) {
			return;
		}
		
		// _client2.Connection.SendInput(tick, (IInput)input);
	}

	public void SetInputsAt<T>(int tick, AllInputs<T> allInputs) where T : IInput {
		throw new System.NotImplementedException();
	}
	public void ApplyEventAt<T>(int tick, int localOrder, T data) {
		throw new System.NotImplementedException();
	}
	public void ApplyEventsAt<T>(int tick, AllEvents<T> allEvents) where T : IEvent {
		throw new System.NotImplementedException();
	}
}