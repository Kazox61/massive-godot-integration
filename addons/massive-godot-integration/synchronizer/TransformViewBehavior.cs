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

	public override void _Process(double delta) {
		if (!_transforms.Has(_entity.Id)) {
			return;
		}
		
		var transform = _transforms.Get(_entity.Id);
		_targetNode.Position = new Vector3(
			transform.Position.X.ToFloat(),
			transform.Position.Y.ToFloat(),
			transform.Position.Z.ToFloat()
		);
		_targetNode.Rotation = new Vector3(
			transform.Rotation.X.ToFloat(),
			transform.Rotation.Y.ToFloat(),
			transform.Rotation.Z.ToFloat()
		);
	}
}