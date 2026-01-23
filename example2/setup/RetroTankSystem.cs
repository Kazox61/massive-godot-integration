using Massive;
using Massive.Netcode;

namespace massivegodotintegration.example2;

public class RetroTankSystem : NetSystem, IInject<RetroTankSetup> {
	public RetroTankSetup Setup { get; private set; }

	void IInject<RetroTankSetup>.Inject(RetroTankSetup setup) {
		Setup = setup;
	}
}