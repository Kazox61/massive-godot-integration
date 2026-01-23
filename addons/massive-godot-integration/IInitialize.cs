using Massive;

namespace massivegodotintegration.addons.massive_godot_integration;

public interface IInitialize : ISystemMethod<IInitialize> {
	void Initialize();

	void ISystemMethod<IInitialize>.Run() => Initialize();
}