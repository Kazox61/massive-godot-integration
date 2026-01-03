using Massive.Netcode;

namespace massivegodotintegration.Example;

public class SystemsSimulation : ISimulation {
	public Massive.Systems Systems { get; }

	public SystemsSimulation(Massive.Systems systems) {
		Systems = systems;
	}

	public void Update(int tick) {
		Systems.Run<IUpdate>();
	}
}