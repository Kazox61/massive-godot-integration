using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Massive.Netcode;

public class TcpServerTransport : ITransportHost {
	private readonly TcpListener _listener;
	private readonly Queue<IConnection> _pendingAccepts = new();

	public TcpServerTransport(int port) {
		_listener = new TcpListener(IPAddress.Any, port);
	}

	public void Start() => _listener.Start();
	public void Stop() => _listener.Stop();

	public void Update() {
		while (_listener.Pending()) {
			var client = _listener.AcceptTcpClient();
			var socket = new TcpSocket(client);
			var connection = new ReliableConnection(socket);
			_pendingAccepts.Enqueue(connection);
		}
	}

	public bool TryAccept(out IConnection connection) => _pendingAccepts.TryDequeue(out connection);
}