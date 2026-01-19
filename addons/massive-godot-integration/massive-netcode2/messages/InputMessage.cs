using System.Collections.Generic;
using System.IO;
using massivegodotintegration.example.input;

namespace Massive.Netcode;

public class InputMessage : NetMessage {
	public Dictionary<int, IInput> PendingInputs { get; set; } = [];
	
	public override byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		
		writer.Write(PendingInputs.Count);
		foreach (var (tick, input) in PendingInputs) {
			writer.Write(tick);
			//TODO: use InputTypeLookup to write type id
			var inputBytes = input.ToBytes();
			writer.Write(inputBytes.Length);
			writer.Write(inputBytes);
		}
		return stream.ToArray();
	}
	
	public override void FromBytes(byte[] bytes) {
		using var stream = new MemoryStream(bytes);
		using var reader = new BinaryReader(stream);
		
		var count = reader.ReadInt32();
		PendingInputs = new Dictionary<int, IInput>(count);
		for (var i = 0; i < count; i++) {
			var tick = reader.ReadInt32();
			var inputLength = reader.ReadInt32();
			var inputBytes = reader.ReadBytes(inputLength);
			
			//TODO: use InputTypeLookup to create correct input type
			var input = new PlayerInput();
			input.FromBytes(inputBytes);
			
			PendingInputs[tick] = input;
		}
	}
}