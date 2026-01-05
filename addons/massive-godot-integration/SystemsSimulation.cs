using Massive.Netcode;

namespace massivegodotintegration.addons.massive_godot_integration;

public class SystemsSimulation : ISimulation {
	public Massive.Systems Systems { get; }

	public SystemsSimulation(Massive.Systems systems) {
		Systems = systems;
	}

	public void Update(int tick) {
		Systems.Run<IUpdate>();
	}
}