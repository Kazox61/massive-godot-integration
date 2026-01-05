using Mathematics.Fixed;

namespace massivegodotintegration.addons.massive_godot_integration.components;

public class BoxCollider : ICollider {
	public FVector3 HalfExtents;
	public FVector3 Centre { get; set; }
	public FVector3 SupportPoint(FVector3 direction) {
		return Centre + new FVector3(
			direction.X >= 0 ? HalfExtents.X : -HalfExtents.X,
			direction.Y >= 0 ? HalfExtents.Y : -HalfExtents.Y,
			direction.Z >= 0 ? HalfExtents.Z : -HalfExtents.Z
		);
	}

	public FP BoundingRadius => FVector3.Length(HalfExtents);
}