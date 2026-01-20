using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Massive.Netcode;

public class TcpServerTransport : ITransportHost {
	private readonly TcpListener _listener;
	private readonly Queue<ISocket> _pendingAccepts = new();

	public TcpServerTransport(int port) {
		_listener = new TcpListener(IPAddress.Any, port);
	}

	public void Start() => _listener.Start();
	public void Stop() => _listener.Stop();

	public void Update() {
		while (_listener.Pending()) {
			var client = _listener.AcceptTcpClient();
			var socket = new TcpSocket(client);
			_pendingAccepts.Enqueue(socket);
		}
	}

	public bool TryAccept(out ISocket socket) => _pendingAccepts.TryDequeue(out socket);
}