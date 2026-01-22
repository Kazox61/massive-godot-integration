using System.Numerics;
using Fixed64;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;
using Massive.Physics;
using massivegodotintegration.example.components;

namespace massivegodotintegration.example.systems;

public class CameraFollowSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity _, ref Camera _, ref CameraTarget cameraTarget, ref Transform transform) => {
			if (!cameraTarget.TargetEntity.IsAlive) {
				return;
			}
			
			var playerTransform = cameraTarget.TargetEntity.Get<Transform>();
			var desiredPosition = new FVector3(playerTransform.Position.X.ToFP(), playerTransform.Position.Y.ToFP(),playerTransform.Position.Z.ToFP()) + cameraTarget.Offset;
			transform.Position = new Vector3(desiredPosition.X.ToFloat(), desiredPosition.Y.ToFloat(), desiredPosition.Z.ToFloat());
		});
	}
}