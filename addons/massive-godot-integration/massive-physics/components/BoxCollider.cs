using Fixed64;

namespace Massive.Physics;


public class BoxCollider : ISupportMappable {
	public FVector3 HalfExtents;
	public FVector3 Center { get; set; }

	public int ProxyKey = -1;

	public FVector3 SupportPoint(FVector3 direction) {
		return Center + new FVector3(
			direction.X >= 0 ? HalfExtents.X : -HalfExtents.X,
			direction.Y >= 0 ? HalfExtents.Y : -HalfExtents.Y,
			direction.Z >= 0 ? HalfExtents.Z : -HalfExtents.Z
		);
	}

	public FP BoundingRadius => FVector3.Length(HalfExtents);
}