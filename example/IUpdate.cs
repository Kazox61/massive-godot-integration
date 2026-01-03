using Massive;

namespace massivegodotintegration.Example;

public interface IUpdate : ISystemMethod<IUpdate> {
	void Update();

	void ISystemMethod<IUpdate>.Run() => Update();
}