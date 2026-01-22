using Massive;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

public interface IEntityView {
	void Register(World world, Entity viewEntity);
	void AssignEntity(World world, Entity entity);
	void RemoveEntity();
}