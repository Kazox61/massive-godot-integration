using System;
using System.Collections.Generic;
using Ruffles.Configuration;
using Ruffles.Connections;
using Ruffles.Core;

namespace Massive.Netcode;

public class UdpServerTransport : ITransportHost {
	private readonly RuffleSocket _server;
	private readonly Queue<IConnection> _pendingAccepts = new();
	
	private readonly Dictionary<Connection, UdpSocket> _sockets = new();

	public UdpServerTransport(SocketConfig config) {
		_server = new RuffleSocket(config);
	}

	public void Start() => _server.Start();
	public void Stop() => _server.Stop();

	public void Update() {
		while (true) {
			var serverEvent = _server.Poll();
			if (serverEvent.Type == NetworkEventType.Nothing) {
				break;
			}

			HandleEvent(serverEvent);
			serverEvent.Recycle();
		}
	}

	private void HandleEvent(NetworkEvent serverEvent) {
		switch (serverEvent.Type) {
			case NetworkEventType.Connect:
				var udpSocket = new UdpSocket(serverEvent.Connection);
				var connection = new UnreliableConnection(udpSocket);
				_sockets[serverEvent.Connection] = udpSocket;
				_pendingAccepts.Enqueue(connection);
				break;
			case NetworkEventType.Data:
				ReadOnlySpan<byte> bytes = serverEvent.Data.Array.AsSpan(serverEvent.Data.Offset, serverEvent.Data.Count);
				if (_sockets.TryGetValue(serverEvent.Connection, out var socket)) {
					socket.EnqueueReceivedData(bytes);
				}
				break;
		}
	}

	public bool TryAccept(out IConnection connection) => _pendingAccepts.TryDequeue(out connection);
}