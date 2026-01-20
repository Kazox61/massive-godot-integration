using System.IO;

namespace Massive.Netcode;

public class PingMessage : INetMessage {
	public float ClientStartTime;
	
	public byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(ClientStartTime);
		return stream.ToArray();
	}
	
	public void FromBytes(byte[] bytes) {
		using var stream = new MemoryStream(bytes);
		using var reader = new BinaryReader(stream);
		ClientStartTime = reader.ReadSingle();
	}
}