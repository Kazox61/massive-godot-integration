using System;
using System.Net;
using Ruffles.Configuration;
using Ruffles.Core;

namespace Massive.Netcode;

public class UdpClientTransport : ITransport {
	public bool IsConnected { get; private set; }
	public IConnection Connection { get; private set; }
	
	public ISocket Socket { get; private set; }

	private readonly RuffleSocket _clientSocket;
	
	public UdpClientTransport(SocketConfig config) {
		_clientSocket = new RuffleSocket(config);
	}

	public void Connect() {
		_clientSocket.Start();
		_clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5674));
	}
	
	public void Update() {
		Connection?.Update();

		while (true) {
			var clientEvent = _clientSocket.Poll();
			
			if (clientEvent.Type == NetworkEventType.Nothing) {
				break;
			}
			
			switch (clientEvent.Type) {
				case NetworkEventType.Connect:
					IsConnected = true;
					Socket = new UdpSocket(clientEvent.Connection);
					Connection = new UnreliableConnection(Socket);
					break;
				case NetworkEventType.Data:
					ReadOnlySpan<byte> bytes = clientEvent.Data.Array.AsSpan(clientEvent.Data.Offset, clientEvent.Data.Count);
					((UdpSocket)Socket).EnqueueReceivedData(bytes);
					break;
			}
			
			clientEvent.Recycle();
		}
	}
}