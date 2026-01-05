using Godot;
using Massive;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

public abstract partial class ViewBehavior : Node {
	public abstract void OnEntityAssigned(World world, Entity entity);
	public abstract void OnEntityRemoved();
}