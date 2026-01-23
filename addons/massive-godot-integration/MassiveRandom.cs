using System;
using System.Runtime.CompilerServices;
using Massive;

namespace massivegodotintegration.addons.massive_godot_integration;

public class MassiveRandom : IMassive {
	private readonly CyclicFrameCounter _cyclicFrameCounter;
	private readonly uint[] _stateFrames;

	private uint _state;

	public MassiveRandom(int framesCapacity, uint seed = 1) {
		_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);
		_stateFrames = new uint[framesCapacity];
		_state = seed;
	}

	public int CanRollbackFrames => _cyclicFrameCounter.CanRollbackFrames;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SaveFrame() {
		_cyclicFrameCounter.SaveFrame();
		var frame = _cyclicFrameCounter.CurrentFrame;
		_stateFrames[frame] = _state;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Rollback(int frames) {
		_cyclicFrameCounter.Rollback(frames);
		var frame = _cyclicFrameCounter.CurrentFrame;
		_state = _stateFrames[frame];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int NextInt() {
		_state ^= _state << 13;
		_state ^= _state >> 17;
		_state ^= _state << 5;
		return (int)_state;
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