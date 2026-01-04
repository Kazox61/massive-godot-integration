using Massive;

namespace massivegodotintegration.addons.massive_godot_integration;

public interface IUpdate : ISystemMethod<IUpdate> {
	void Update();

	void ISystemMethod<IUpdate>.Run() => Update();
}