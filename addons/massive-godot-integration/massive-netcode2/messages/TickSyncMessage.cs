using System.Collections.Generic;
using System.IO;
using massivegodotintegration.example.input;

namespace Massive.Netcode;

public class TickSyncMessage : NetMessage {
	public int InputChannel { get; set; } // workaround to notify the client about its input channel
	public int ApprovedTick { get; set; }
	public float ServerTime { get; set; }
	public int[] AcknowledgedInputs { get; set; }
	public Dictionary<int, IInput> LastApprovedInputs { get; set; }
	
	
	public override byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(ApprovedTick);
		writer.Write(ServerTime);
		writer.Write(AcknowledgedInputs.Length);
		foreach (var inputTick in AcknowledgedInputs) {
			writer.Write(inputTick);
		}

		writer.Write(LastApprovedInputs.Count);
		foreach (var (inputChannel, input) in LastApprovedInputs) {
			writer.Write(inputChannel);
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
		ApprovedTick = reader.ReadInt32();
		ServerTime = reader.ReadSingle();
		var inputsCount = reader.ReadInt32();
		AcknowledgedInputs = new int[inputsCount];
		for (var i = 0; i < inputsCount; i++) {
			AcknowledgedInputs[i] = reader.ReadInt32();
		}
		
		var lastApprovedInputsCount = reader.ReadInt32();
		LastApprovedInputs = new Dictionary<int, IInput>(lastApprovedInputsCount);
		for (var i = 0; i < lastApprovedInputsCount; i++) {
			var inputChannel = reader.ReadInt32();
			var inputLength = reader.ReadInt32();
			var inputBytes = reader.ReadBytes(inputLength);
			
			//TODO: use InputTypeLookup to create correct input type
			var input = new PlayerInput();
			input.FromBytes(inputBytes);
			LastApprovedInputs[inputChannel] = input;
		}
	}
}