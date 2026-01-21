using System;

namespace Massive.Netcode;

public interface ISocket {
	void Send(ReadOnlySpan<byte> payload);
	bool TryReceive(out ReadOnlySpan<byte> payload);
}