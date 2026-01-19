using System.IO;

namespace Massive.Netcode;

public class TickSyncMessage : NetMessage {
	public int ApprovedTick { get; set; }
	public float ServerTime { get; set; }
	public int[] ApprovedInputs { get; set; }
	
	public override byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(ApprovedTick);
		writer.Write(ServerTime);
		writer.Write(ApprovedInputs.Length);
		foreach (var inputTick in ApprovedInputs) {
			writer.Write(inputTick);
		}
		return stream.ToArray();
	}
	
	public override void FromBytes(byte[] bytes) {
		using var stream = new MemoryStream(bytes);
		using var reader = new BinaryReader(stream);
		ApprovedTick = reader.ReadInt32();
		ServerTime = reader.ReadSingle();
		var inputsCount = reader.ReadInt32();
		ApprovedInputs = new int[inputsCount];
		for (var i = 0; i < inputsCount; i++) {
			ApprovedInputs[i] = reader.ReadInt32();
		}
	}
}