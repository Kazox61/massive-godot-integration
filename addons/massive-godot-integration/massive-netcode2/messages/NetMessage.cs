namespace Massive.Netcode;

public abstract class NetMessage : IBinarySerializable {
	public abstract byte[] ToBytes();
	public abstract void FromBytes(byte[] bytes);
}