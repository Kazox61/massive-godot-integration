using System;
using System.Buffers.Binary;

namespace Massive.Netcode;

public class MessageSerializer {
	private readonly GenericTypeLookup<INetMessage> _messageTypeLookup = new();

	public MessageSerializer() {
		_messageTypeLookup.RegisterAll();
	}
	
	public byte[] CreateBytes(INetMessage message) {
		var messageId = _messageTypeLookup.GetTypeId(message.GetType());
		var messageIdBytes = new byte[4];
		BinaryPrimitives.WriteInt32BigEndian(messageIdBytes, messageId);

		var messagePayload = message.ToBytes();

		var result = new byte[messageIdBytes.Length + messagePayload.Length];
		Buffer.BlockCopy(messageIdBytes, 0, result, 0, messageIdBytes.Length);
		Buffer.BlockCopy(messagePayload, 0, result, messageIdBytes.Length, messagePayload.Length);
		
		return result;
	}

	public INetMessage CreateMessage(byte[] messageBytes) {
		var messageId = BinaryPrimitives.ReadInt32BigEndian(messageBytes.AsSpan(0,4));

		var messageType = _messageTypeLookup.GetType(messageId);
		if (messageType == null) {
			throw new InvalidOperationException("Unknown message type id: " + messageId);
		}
		
		var message = (INetMessage)Activator.CreateInstance(messageType)!;
		message.FromBytes(messageBytes[4..]);
		return message;
	}
}