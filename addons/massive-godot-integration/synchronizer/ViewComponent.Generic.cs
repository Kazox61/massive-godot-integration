using Massive;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

public abstract partial class ViewComponent<T> : ViewComponent where T : ViewComponent<T> {
	public override void Register(World world, Entity viewEntity) {
		viewEntity.Set((T)this);
	}
}