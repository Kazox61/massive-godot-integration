using Godot;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;

namespace Massive.Physics;

public class SyncPhysicsWorldSystem : PhysicsSystem, IUpdate {

	public void Update() {
		World
			.Include<Transform, PhysicsBody>()
			.ForEach((Entity entity, ref Transform transform, ref PhysicsBody body) => {
				if (body.IsStatic) {
					return;
				}
				
				var rigidBody = PhysicsWorld.Simulation.Bodies[body.BodyHandle];
				transform.Position = rigidBody.Pose.Position;
				transform.Rotation = rigidBody.Pose.Orientation;
			});
	}
}