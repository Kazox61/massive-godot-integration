using BepuPhysics;

namespace Massive.Physics;

public struct PhysicsBody {
	public bool IsStatic;
	public BodyHandle BodyHandle;
	public StaticHandle StaticHandle;
}