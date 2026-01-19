using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Massive.Netcode;

public class ReliableConnection : IConnection {
	private readonly ISocket _socket;

	private readonly GenericTypeLookup<NetMessage> _messageTypeLookup = new();

	private readonly Queue<NetMessage> _incoming = new();
	private readonly Queue<(int tick, IInput input)> _incomingInputs = new();

	public ReliableConnection(ISocket socket) {
		_socket = socket;
		_messageTypeLookup.RegisterAll();
	}

	public void Update() {
		while (_socket.TryReceive(out var payload)) {
			var message = CreateMessage(payload.ToArray());
			_incoming.Enqueue(message);
			switch (message) {
				case InputMessage2 inputMessage2:
					_incomingInputs.Enqueue((inputMessage2.Tick, inputMessage2.Input));
					break;
			}
		}
	}
	
	public bool TryDequeueInput(out int tick, out IInput input) {
		if (_incomingInputs.Count > 0) {
			(tick, input) = _incomingInputs.Dequeue();
			return true;
		}

		tick = 0;
		input = null!;
		return false;
	}

	public bool TryDequeueMessage(out NetMessage message) {
		if (_incoming.Count > 0) {
			message = _incoming.Dequeue();
			return true;
		}

		message = null!;
		return false;
	}

	public void SendInput(int tick, IInput input) {
		SendMessage(new InputMessage2 {
			Tick = tick,
			Input = input
		});
	}

	public void SendMessage(NetMessage message) {
		var messageId = _messageTypeLookup.GetTypeId(message.GetType());
		var messageIdBytes = new byte[4];
		BinaryPrimitives.WriteInt32BigEndian(messageIdBytes, messageId);

		var messagePayload = message.ToBytes();

		var result = new byte[messageIdBytes.Length + messagePayload.Length];
		Buffer.BlockCopy(messageIdBytes, 0, result, 0, messageIdBytes.Length);
		Buffer.BlockCopy(messagePayload, 0, result, messageIdBytes.Length, messagePayload.Length);

		_socket.Send(result);
	}

	private NetMessage CreateMessage(byte[] messageBytes) {
		var messageId = BinaryPrimitives.ReadInt32BigEndian(messageBytes.AsSpan(0,4));

		var messageType = _messageTypeLookup.GetType(messageId);
		if (messageType == null) {
			throw new InvalidOperationException("Unknown message type id: " + messageId);
		}
		
		var message = (NetMessage)Activator.CreateInstance(messageType)!;
		message.FromBytes(messageBytes[4..]);
		return message;
	}
}