namespace Massive.Netcode;

public interface IConnection {
	bool TryDequeueMessage(out NetMessage message);
	void SendInput(int tick, IInput input);
	void SendMessage(NetMessage message);
	void Update();
}