using System.IO;

namespace Massive.Netcode;

public class SetupClientMessage : INetMessage {
	public uint Seed { get; set; }
	public int InputChannel { get; set; }
	
	public byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(Seed);
		writer.Write(InputChannel);
		
		return stream.ToArray();
	}
	
	public void FromBytes(byte[] bytes) {
		using var stream = new MemoryStream(bytes);
		using var reader = new BinaryReader(stream);
		Seed = reader.ReadUInt32();
		InputChannel = reader.ReadInt32();
	}
}