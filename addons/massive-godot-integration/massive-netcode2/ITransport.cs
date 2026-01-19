namespace Massive.Netcode;

public interface ITransport {
	bool IsConnected { get; }
	IConnection Connection { get; }
	ISocket Socket { get; }

	void Connect();
	void Update();
}