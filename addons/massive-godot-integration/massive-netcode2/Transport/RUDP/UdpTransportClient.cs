using System;
using System.Net;
using Ruffles.Configuration;
using Ruffles.Core;

namespace Massive.Netcode;

public class UdpTransportClient : ITransportClient {
	public bool IsConnected { get; private set; }
	
	public ISocket Socket { get; private set; }

	private readonly RuffleSocket _clientSocket;
	private readonly IPEndPoint _endPoint;
	
	public UdpTransportClient(IPEndPoint endPoint, SocketConfig config) {
		_endPoint = endPoint;
		_clientSocket = new RuffleSocket(config);
	}

	public void Connect() {
		_clientSocket.Start();
		_clientSocket.Connect(_endPoint);
	}
	
	public void Update() {
		while (true) {
			var clientEvent = _clientSocket.Poll();
			
			if (clientEvent.Type == NetworkEventType.Nothing) {
				break;
			}
			
			switch (clientEvent.Type) {
				case NetworkEventType.Connect:
					IsConnected = true;
					Socket = new UdpSocket(clientEvent.Connection);
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