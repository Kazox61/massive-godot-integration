using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.addons.massive_godot_integration.Components;
using massivegodotintegration.example.components;

namespace massivegodotintegration.example.systems;

public class CameraFollowSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity _, ref Camera _, ref CameraTarget cameraTarget, ref Transform transform) => {
			if (!cameraTarget.TargetEntity.IsAlive) {
				return;
			}
			
			var playerTransform = cameraTarget.TargetEntity.Get<Transform>();
			var desiredPosition = playerTransform.Position + cameraTarget.Offset;
			transform.Position = desiredPosition;
		});
	}
}