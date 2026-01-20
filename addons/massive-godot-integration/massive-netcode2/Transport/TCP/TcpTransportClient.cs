using System.Net.Sockets;

namespace Massive.Netcode;


public class TcpTransportClient : ITransportClient {
	public bool IsConnected { get; private set; }
	public ISocket Socket { get; private set; }

	private TcpClient _client;

	private readonly string _host;
	private readonly int _port;

	public TcpTransportClient(string host, int port) {
		_host = host;
		_port = port;
	}

	public void Connect() {
		_client = new TcpClient();
		_client.NoDelay = true;

		_client.Connect(_host, _port);

		Socket = new TcpSocket(_client);
		IsConnected = true;
	}

	public void Update() { }
}