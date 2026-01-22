using Fixed64;
using Godot;
using Massive;
using massivegodotintegration.addons.massive_godot_integration.components;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.example2;

public partial class Transform2dViewBehavior : ViewBehavior {
	[Export] private Node2D _targetNode;

	private DataSet<Transform2d> _transforms;
	private Entity _entity;
	
	public override void OnEntityAssigned(World world, Entity entity) {
		_entity = entity;
		_transforms = world.DataSet<Transform2d>();
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
		_targetNode.Position = new Vector2(
			transform.Position.X.ToFloat(),
			transform.Position.Y.ToFloat()
		);
		_targetNode.Rotation = transform.Rotation.ToFloat();
	}
}