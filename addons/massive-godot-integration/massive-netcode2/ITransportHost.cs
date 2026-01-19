namespace Massive.Netcode;

public interface ITransportHost {
	void Start();
	void Stop();
	void Update();

	bool TryAccept(out IConnection connection);
}