using Massive;

namespace massivegodotintegration.addons.massive_godot_integration;

public interface IFirstTick : ISystemMethod<IFirstTick> {
	void FirstTick();
	
	void ISystemMethod<IFirstTick>.Run() => FirstTick();
}