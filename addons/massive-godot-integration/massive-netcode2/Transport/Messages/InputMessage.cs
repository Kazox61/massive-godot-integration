using System.IO;
using massivegodotintegration.example.input;

namespace Massive.Netcode;

public class InputMessage : INetMessage {
	public int Tick { get; set; }
	public IInput Input { get; set; }
	
	public byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(Tick);
		//TODO: use InputTypeLookup to write type id
		var inputBytes = Input.ToBytes();
		writer.Write(inputBytes.Length);
		writer.Write(inputBytes);
		return stream.ToArray();
	}
	
	public void FromBytes(byte[] bytes) {
		using var stream = new MemoryStream(bytes);
		using var reader = new BinaryReader(stream);
		Tick = reader.ReadInt32();
		var inputLength = reader.ReadInt32();
		var inputBytes = reader.ReadBytes(inputLength);
		//TODO: use InputTypeLookup to create correct input type
		Input = new PlayerInput();
		Input.FromBytes(inputBytes);
	}
}