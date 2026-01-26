using Massive;
using Massive.Netcode;
using Massive.Physics;
using Fixed64;
using Godot;
using massivegodotintegration.addons.massive_godot_integration;

namespace Massive.Physics;

public class PhysicsNarrowPhaseSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity pairEntity, ref BroadPhasePair pair) => {
			ref var colA = ref pair.A.Get<BoxCollider>();
			ref var colB = ref pair.B.Get<BoxCollider>();

			// update collider centres
			colA.Center = pair.A.Get<Transform>().Position;
			colB.Center = pair.B.Get<Transform>().Position;

			var gjk = GJK.Calculate(colA, colB);

			if (!gjk.CollisionHappened) {
				pairEntity.Destroy();
				return;
			}

			var collision = EPA.Calculate(gjk.Simplex, colA, colB);

			World.Create(new Contact {
				EntifierA = pair.A.Entifier,
				EntifierB = pair.B.Entifier,
				Collision = collision
			});

			pairEntity.Destroy();
		});
	}
}