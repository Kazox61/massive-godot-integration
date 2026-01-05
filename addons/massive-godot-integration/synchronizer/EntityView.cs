using Godot;
using Godot.Collections;
using Massive;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

public partial class EntityView : Node3D {
	[Export] private Array<ViewBehavior> _viewBehaviors = [];
	
	public World World { get; protected set; }
	public Entity Entity { get; protected set; }
	
	public void Register(World world, Entity  viewEntity) {
		viewEntity.Set(this);
	}
	
	public void AssignEntity(World world, Entity entity) {
		World = world;
		Entity = entity;

		foreach (var viewBehavior in _viewBehaviors) {
			viewBehavior.OnEntityAssigned(world, entity);
		}
	}
	
	public void RemoveEntity() {
		foreach (var viewBehavior in _viewBehaviors) {
			viewBehavior.OnEntityRemoved();
		}
	}
}