using System;
using System.Runtime.CompilerServices;
using Massive;

namespace massivegodotintegration.addons.massive_godot_integration;

public class MassiveRandom : WorldSystem<uint>, IInjectSelf<MassiveRandom> {

	public MassiveRandom(uint seed = 1) : base(seed) {
		
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int NextInt() {
		var state = State;
		
		state ^= state << 13;
		state ^= state >> 17;
		State ^= state << 5;
		return (int)state;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int NextInt(int min, int max) {
		if (min >= max)
			throw new ArgumentException("min must be less than max");

		var range = (uint)(max - min);
		var value = (uint)NextInt();
		return (int)(value % range) + min;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float NextInt(int max) {
		return NextInt(0, max);
	}
}