using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration;

namespace massivegodotintegration.example2;

public class RetroTankSystem : NetSystem, IInject<RetroTankSetup>, IInject<MassiveRandom> {
	public RetroTankSetup Setup { get; private set; }
	
	public MassiveRandom Random { get; private set; }

	void IInject<RetroTankSetup>.Inject(RetroTankSetup setup) {
		Setup = setup;
	}

	public void Inject(MassiveRandom random) {
		Random = random;
	}
}