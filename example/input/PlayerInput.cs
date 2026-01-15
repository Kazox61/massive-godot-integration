using Massive.Netcode;

namespace massivegodotintegration.example.input;

public struct PlayerInput : IInput {
	public float DirectionX;
	public float DirectionY;
	public bool Kill;
	public bool Jump;
	public bool Attack;
}