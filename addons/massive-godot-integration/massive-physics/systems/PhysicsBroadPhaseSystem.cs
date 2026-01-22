using Massive.Netcode;
using Fixed64;
using massivegodotintegration.addons.massive_godot_integration;
using FConversions = Fixed32.FConversions;

namespace Massive.Physics;

public class PhysicsBroadPhaseSystem : NetSystem, IUpdate {
	private BroadPhase _broadPhase = new();
	
	public void Update() {
		World.ForEach((Entity entity, ref Transform transform, ref BoxCollider collider, ref RigidBody rigidBody) => {
			if (collider.ProxyKey == -1) {
				var bodyType = rigidBody.InverseMass == FP.Zero ? BroadPhase.BodyType.Static : BroadPhase.BodyType.Dynamic;
				var aabb = ComputeAABB(transform, collider);

				collider.ProxyKey = _broadPhase.CreateProxy(bodyType, aabb, ulong.MaxValue, (ulong)entity.Id);
			}
			else {
				// Move existing proxy
				var aabb = ComputeAABB(transform, collider);
				_broadPhase.MoveProxy(collider.ProxyKey, aabb);
			}
		});
		
		_broadPhase.UpdatePairs((entityIdA, entityIdB) => {
			World.Create(new BroadPhasePair {
				A = World.GetEntity((int)entityIdA),
				B = World.GetEntity((int)entityIdB)
			});
		});
	}

	private static Fixed32.FAABB ComputeAABB(Transform transform, BoxCollider collider) {
		var pos = new Fixed32.FVector3(
			FConversions.ToFP(transform.Position.X.ToFloat()),
			FConversions.ToFP(transform.Position.Y.ToFloat()),
			FConversions.ToFP(transform.Position.Z.ToFloat()));
		var halfSize = new Fixed32.FVector3(
			FConversions.ToFP(collider.HalfExtents.X.ToFloat()),
			FConversions.ToFP(collider.HalfExtents.Y.ToFloat()),
			FConversions.ToFP(collider.HalfExtents.Z.ToFloat()));

		return new Fixed32.FAABB {
			LowerBound = pos - halfSize,
			UpperBound = pos + halfSize
		};
	}
}