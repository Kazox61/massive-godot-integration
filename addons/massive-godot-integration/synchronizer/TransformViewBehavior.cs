using System;
using BepuUtilities;
using Godot;
using Massive;
using Massive.Physics;
using Fixed64;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

[GlobalClass]
public partial class TransformViewBehavior : ViewBehavior {
	[Export] private Node3D _targetNode;

	private DataSet<Transform> _transforms;
	private Entity _entity;

	public override void OnEntityAssigned(World world, Entity entity) {
		_entity = entity;
		_transforms = world.DataSet<Transform>();
	}

	public override void OnEntityRemoved() {
		_transforms = null;
		_entity = Entity.Dead;
	}

	public override void _PhysicsProcess(double delta) {
		if (!_transforms.Has(_entity.Id)) {
			return;
		}

		var transform = _transforms.Get(_entity.Id);

		QuaternionEx.TransformUnitZ(transform.Rotation, out var forward);
		QuaternionEx.TransformUnitX(transform.Rotation, out var right);
		QuaternionEx.TransformUnitY(transform.Rotation, out var up);

		_targetNode.Position = new Vector3(
			transform.Position.X,
			transform.Position.Y,
			transform.Position.Z
		);
		_targetNode.Rotation = QuaternionToEuler(transform.Rotation);
	}

	private static Vector3 QuaternionToEuler(System.Numerics.Quaternion q) {
		Vector3 euler;

		// Roll (X-axis rotation)
		var sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
		var cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
		euler.X = MathF.Atan2(sinr_cosp, cosr_cosp);

		// Pitch (Y-axis rotation)
		var sinp = 2 * (q.W * q.Y - q.Z * q.X);
		if (MathF.Abs(sinp) >= 1)
			euler.Y = MathF.CopySign(MathF.PI / 2, sinp); // Use 90 degrees if out of range
		else
			euler.Y = MathF.Asin(sinp);

		// Yaw (Z-axis rotation)
		var siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
		var cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
		euler.Z = MathF.Atan2(siny_cosp, cosy_cosp);

		return euler; // Returns radians
	}
}