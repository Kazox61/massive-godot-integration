using Godot;
using Massive;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

public abstract partial class ViewComponent : Node {
	public abstract void Register(World world, Entity viewEntity);
}