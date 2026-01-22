using Godot;
using Godot.Collections;
using Massive;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

[GlobalClass]
public partial class EntityView3d : Node3D, IEntityView {
	[Export] private Array<ViewBehavior> _viewBehaviors = [];
	[Export] private Array<ViewComponent> _viewComponents = [];

	public World World { get; protected set; }
	public Entity Entity { get; protected set; }

	public void Register(World world, Entity  viewEntity) {
		viewEntity.Set(this);
		foreach (var viewComponent in _viewComponents) {
			viewComponent.Register(world, viewEntity);
		}
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