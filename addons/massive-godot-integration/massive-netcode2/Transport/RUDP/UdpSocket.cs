using System;
using System.Collections.Generic;
using Ruffles.Connections;

namespace Massive.Netcode;

public class UdpSocket : ISocket {
	private readonly Connection _connection;
	
	private readonly Queue<byte[]> _receivedData = new();
	
	public UdpSocket(Connection connection) {
		_connection = connection;
	}
	
	public void Send(ReadOnlySpan<byte> payload) {
		var arraySegment = new ArraySegment<byte>(payload.ToArray(), 0, payload.Length);
		_connection.Send(arraySegment, 0, false, 0); // not sure about the notification key? what is it for?
	}
	
	public bool TryReceive(out ReadOnlySpan<byte> payload) {
		if (_receivedData.Count > 0) {
			var packet = _receivedData.Dequeue();
			payload = new ReadOnlySpan<byte>(packet);
			return true;
		}
		
		payload = ReadOnlySpan<byte>.Empty;
		return false;
	}
	
	public void EnqueueReceivedData(ReadOnlySpan<byte> data) {
		var buffer = new byte[data.Length];
		data.CopyTo(buffer);
		_receivedData.Enqueue(buffer);
	}
}