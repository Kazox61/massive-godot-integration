using Massive.Netcode;

namespace Massive.Physics;

public class PhysicsSystem : NetSystem, IInject<PhysicsWorld> {
	public PhysicsWorld PhysicsWorld { get; private set; }
	
	public void Inject(PhysicsWorld physicsWorld) {
		PhysicsWorld = physicsWorld;
	}
}