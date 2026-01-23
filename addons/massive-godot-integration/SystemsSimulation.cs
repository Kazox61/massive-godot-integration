using Massive.Netcode;

namespace massivegodotintegration.addons.massive_godot_integration;

public class SystemsSimulation : ISimulation {
	public Massive.Systems Systems { get; }

	public SystemsSimulation(Massive.Systems systems) {
		Systems = systems;
	}
	
	public void Initialize() {
		Systems.Run<IInitialize>();
	}

	public void Update(int tick) {
		if (tick == 0) {
			Systems.Run<IFirstTick>();
		}
		
		Systems.Run<IUpdate>();
	}
}