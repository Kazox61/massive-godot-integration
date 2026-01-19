namespace Massive.Netcode;

public interface IBinarySerializable {
	public byte[] ToBytes();
	public void FromBytes(byte[] bytes);
}