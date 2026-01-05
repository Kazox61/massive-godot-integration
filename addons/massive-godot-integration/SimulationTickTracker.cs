using Massive.Netcode;

namespace massivegodotintegration.addons.massive_godot_integration;

public class SimulationTickTracker : ISimulation {
	public int TicksAmount { get; private set; }

	public void Update(int tick) {
		TicksAmount += 1;
	}

	public void Restart() {
		TicksAmount = 0;
	}
}