using System.Collections.Generic;
using System.IO;

namespace Massive.Netcode;

public class TickSyncMessage : INetMessage {
	public int ApprovedTick { get; set; }
	public List<(int tick, int inputChannel, IInput input)> Inputs { get; set; } = [];
	
	
	public byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(ApprovedTick);
		writer.Write(Inputs.Count);
		foreach (var (tick, inputChannel, input) in Inputs) {
			writer.Write(tick);
			writer.Write(inputChannel);
			//TODO: use InputTypeLookup to write type id
			var inputBytes = input.ToBytes();
			writer.Write(inputBytes.Length);
			writer.Write(inputBytes);
		}
		
		return stream.ToArray();
	}
	
	public void FromBytes(byte[] bytes) {
		using var stream = new MemoryStream(bytes);
		using var reader = new BinaryReader(stream);
		ApprovedTick = reader.ReadInt32();
		var inputsCount = reader.ReadInt32();
		Inputs = [];
		for (var i = 0; i < inputsCount; i++) {
			var tick = reader.ReadInt32();
			var inputChannel = reader.ReadInt32();
			var inputLength = reader.ReadInt32();
			var inputBytes = reader.ReadBytes(inputLength);
			//TODO: use InputTypeLookup to create correct input type
			var input = new PlayerInput();
			input.FromBytes(inputBytes);
			Inputs.Add((tick, inputChannel, input));
		}
	}
}