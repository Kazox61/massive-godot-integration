namespace Massive.Netcode;

public interface ITransportClient {
	bool IsConnected { get; }
	ISocket Socket { get; }

	void Connect();
	void Update();
}