using Massive;
using Fixed64;

namespace Massive.Physics;

public struct Contact {
	public Entifier EntifierA;
	public Entifier EntifierB;
	public Collision Collision;
}