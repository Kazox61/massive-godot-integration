using System.Collections.Generic;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.Components;
using Mathematics.Fixed;

namespace massivegodotintegration.addons.massive_godot_integration.systems;

public class PhysicsBroadPhaseSystem : NetSystem, IUpdate {
	public void Update() {
		var entities = new List<Entity>();
		World.ForEach((Entity entity, ref Transform _, ref BoxCollider _) => {
			entities.Add(entity);
		});

		for (var i = 0; i < entities.Count; i++) {
			for (var j = i + 1; j < entities.Count; j++) {
				var a = entities[i];
				var b = entities[j];

				ref var ta = ref a.Get<Transform>();
				ref var tb = ref b.Get<Transform>();

				ref var ca = ref a.Get<BoxCollider>();
				ref var cb = ref b.Get<BoxCollider>();

				var delta = tb.Position - ta.Position;
				var radius = ca.BoundingRadius + cb.BoundingRadius;

				if (FVector3.Dot(delta, delta) <= radius * radius) {
					World.Create(new BroadPhasePair { A = a, B = b });
				}
			}
		}
	}
}