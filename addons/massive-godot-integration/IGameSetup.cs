using Massive;

namespace massivegodotintegration.addons.massive_godot_integration;

public interface IGameSetup {
	void SetupGame(MassiveSystems systems, MassiveWorld world, uint seed, int localInputChannel);
}