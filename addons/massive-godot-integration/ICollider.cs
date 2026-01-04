using Mathematics.Fixed;

namespace massivegodotintegration.addons.massive_godot_integration;

public interface ICollider : ISupportMappable {
	public FP BoundingRadius { get; }
}