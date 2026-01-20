using System.Net;
using System.Net.Sockets;

namespace Massive.Netcode;


public class TcpTransportClient : ITransportClient {
	public bool IsConnected { get; private set; }
	public ISocket Socket { get; private set; }

	private TcpClient _client;

	private readonly IPEndPoint _endPoint;

	public TcpTransportClient(IPEndPoint endPoint) {
		_endPoint = endPoint;
	}

	public void Connect() {
		_client = new TcpClient();
		_client.NoDelay = true;

		_client.Connect(_endPoint);

		Socket = new TcpSocket(_client);
		IsConnected = true;
	}

	public void Update() { }
}