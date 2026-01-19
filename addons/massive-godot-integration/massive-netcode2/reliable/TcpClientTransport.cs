using System.Net.Sockets;

namespace Massive.Netcode;


public class TcpClientTransport : ITransport {
	public bool IsConnected { get; private set; }
	public IConnection Connection { get; private set; }
	public ISocket Socket { get; private set; }

	private TcpClient _client;

	private readonly string _host;
	private readonly int _port;

	public TcpClientTransport(string host, int port) {
		_host = host;
		_port = port;
	}

	public void Connect() {
		_client = new TcpClient();
		_client.NoDelay = true;

		_client.Connect(_host, _port);

		Socket = new TcpSocket(_client);
		Connection = new ReliableConnection(Socket);
		IsConnected = true;
	}

	public void Update() {
		// TCP does not need polling
		Connection?.Update();
	}
}