using System.IO;
using Massive.Netcode;

namespace massivegodotintegration.example.input;

public struct PlayerInput : IInput {
	public float DirectionX;
	public float DirectionY;
	public float AimX;
	public float AimY;
	public bool Kill;
	public bool Jump;
	public bool Attack;
	
	public byte[] ToBytes() {
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		writer.Write(DirectionX);
		writer.Write(DirectionY);
		writer.Write(AimX);
		writer.Write(AimY);
		writer.Write(Kill);
		writer.Write(Jump);
		writer.Write(Attack);
		return stream.ToArray();
	}
	
	public void FromBytes(byte[] bytes) {
		using var stream = new MemoryStream(bytes);
		using var reader = new BinaryReader(stream);
		DirectionX = reader.ReadSingle();
		DirectionY = reader.ReadSingle();
		AimX = reader.ReadSingle();
		AimY = reader.ReadSingle();
		Kill = reader.ReadBoolean();
		Jump = reader.ReadBoolean();
		Attack = reader.ReadBoolean();
	}
}