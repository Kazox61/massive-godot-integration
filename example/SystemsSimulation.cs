using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;

namespace massivegodotintegration.example;

public class SystemsSimulation : ISimulation {
	public Massive.Systems Systems { get; }

	public SystemsSimulation(Massive.Systems systems) {
		Systems = systems;
	}

	public void Update(int tick) {
		Systems.Run<IUpdate>();
	}
}