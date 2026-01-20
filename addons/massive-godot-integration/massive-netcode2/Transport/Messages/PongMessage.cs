using System.IO;

namespace Massive.Netcode;

public class PongMessage : INetMessage {
	public float ClientStartTime;
	public float ServerTime;
	
	public byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(ClientStartTime);
		writer.Write(ServerTime);
		return stream.ToArray();
	}
	public void FromBytes(byte[] bytes) {
		using var stream = new MemoryStream(bytes);
		using var reader = new BinaryReader(stream);
		ClientStartTime = reader.ReadSingle();
		ServerTime = reader.ReadSingle();
	}
}