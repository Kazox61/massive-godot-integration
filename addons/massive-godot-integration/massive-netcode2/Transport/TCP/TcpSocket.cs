using System;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Massive.Netcode;

public class TcpSocket : ISocket, IDisposable {
	private readonly TcpClient _client;
	private readonly NetworkStream _stream;

	private readonly byte[] _lengthBuffer = new byte[4];

	public TcpSocket(TcpClient client) {
		_client = client;
		_client.NoDelay = true; // IMPORTANT for latency
		_stream = client.GetStream();
	}

	public void Send(ReadOnlySpan<byte> payload)
	{
		SendPayloadSlow(payload.ToArray()).Forget();
	}

	private void SendPayloadFast(ReadOnlySpan<byte> payload)
	{
		Span<byte> lengthPrefix = stackalloc byte[4];
		BinaryPrimitives.WriteInt32BigEndian(lengthPrefix, payload.Length);

		_stream.Write(lengthPrefix);
		_stream.Write(payload);
	}

	private async Task SendPayloadSlow(byte[] payload)
	{
		await Task.Delay(200);

		var lengthPrefix = new byte[4];
		BinaryPrimitives.WriteInt32BigEndian(lengthPrefix, payload.Length);

		_stream.Write(lengthPrefix);
		_stream.Write(payload);
	}

	public bool TryReceive(out ReadOnlySpan<byte> payload) {
		payload = ReadOnlySpan<byte>.Empty;

		if (!_stream.DataAvailable) {
			return false;
		}

		if (_stream.Read(_lengthBuffer, 0, 4) != 4) {
			return false;
		}

		var length = BinaryPrimitives.ReadInt32BigEndian(_lengthBuffer);
		var buffer = new byte[length];

		var read = 0;
		while (read < length) {
			read += _stream.Read(buffer, read, length - read);
		}

		payload = buffer;
		return true;
	}

	public void Dispose() {
		_stream.Dispose();
		_client.Dispose();
	}
}