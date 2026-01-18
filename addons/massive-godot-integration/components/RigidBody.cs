using Fixed64;

namespace massivegodotintegration.addons.massive_godot_integration.components;

public struct RigidBody {
	public FVector3 Velocity;
	public FVector3 AngularVelocity;
	
	public FP InverseMass; // 0 for static bodies
	public FVector3 InverseInertia;
	
	public FP Restitution;
	public FP Friction;

	public bool UseGravity;
}