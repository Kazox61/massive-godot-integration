using System;
using System.Collections.Generic;
using Godot;

namespace Massive.Netcode;

public class Server {
	private readonly ITransportHost _transportHost;
	private readonly Session _session;
	private readonly int _tickRate;
	private int _currentTick;
	private readonly int _maximumDelayedInputTicks;
	private double _accumulator;
	private int _lastApprovedTick;

	private int ConnectedChannels => _channelConnections.Count;
	private readonly Dictionary<int, IConnection> _channelConnections = new();
	private readonly List<(int tick, int inputChannel, IInput input)> _pendingInputs = [];
	private int _nextChannelId;
	
	private int NextChannelId => _nextChannelId++;

	public Server(ITransportHost transportHost, SessionConfig config) {
		_transportHost = transportHost;
		_session = new Session(config);
		_tickRate = config.TickRate;
		_currentTick = config.StartTick;
		_maximumDelayedInputTicks = config.TickRate;
		_accumulator = 0;
	}
	
	public void Start() {
		_transportHost.Start();
	}
	
	public void Update(double deltaTime) {
		_transportHost.Update();

		while (_transportHost.TryAccept(out var connection)) {
			var channelId = NextChannelId;
			GD.Print($"Accepted new connection, assigning to channel {channelId}.");
			var connectionAdded = _channelConnections.TryAdd(channelId, connection);
			if (!connectionAdded) {
				throw new Exception("Failed to add new connection to channel connections.");
			}
		}

		foreach (var (inputChannel, connection) in _channelConnections) {
			connection.Update();

			while (connection.TryDequeueInput(out var tick, out var input)) {
				if (tick < _currentTick - _maximumDelayedInputTicks || tick > _currentTick) {
					continue;
				}
				
				_session.Inputs.SetAt(tick, inputChannel, input);
				_pendingInputs.Add((tick, inputChannel, input));
			}
			
			while (connection.TryDequeueMessage(out var message)) {
				switch (message) {
					case PingMessage pingMessage:
						connection.SendMessage(new PongMessage {
							ClientStartTime = pingMessage.ClientStartTime
						});
						break;
				}
			}
		}
		
		if (ConnectedChannels < 2) { // lobby with 2 players
			return;
		}
		
		ProcessTick(deltaTime);
	}
	
	private void ProcessTick(double deltaTime) {
		_accumulator += deltaTime;
		var tickTime = 1.0 / _tickRate;

		while (_accumulator >= tickTime) {
			Tick();
			_currentTick++;
			_accumulator -= tickTime;
		}
	}

	private void Tick() {
		_session.Inputs.PopulateUpTo(_currentTick);
		_session.Simulations.Update(_currentTick);
		
		if (_lastApprovedTick < _currentTick - _maximumDelayedInputTicks) {
			_lastApprovedTick = _currentTick - _maximumDelayedInputTicks;
		}

		for (var unapprovedTick = _lastApprovedTick; unapprovedTick < _currentTick; unapprovedTick++) {
			var allInputs = _session.Inputs.GetAllInputsAt<IInput>(unapprovedTick);
			// Ensure there are 2 connected clients before approving ticks (lobby with 2 players)
			var hasAllInputs = allInputs.UsedChannels >= ConnectedChannels && ConnectedChannels >= 2;
			if (!hasAllInputs) {
				break;
			}
			
			_lastApprovedTick = unapprovedTick;
		}
		_session.Inputs.DiscardUpTo(_lastApprovedTick);
		
		foreach (var (inputChannel, connection) in _channelConnections) {
			connection.SendMessage(new TickSyncMessage2 {
				InputChannel = inputChannel,
				ApprovedTick = _lastApprovedTick,
				ServerTime = (float)(_currentTick / (double)_tickRate),
				Inputs = _pendingInputs
			});
		}
		
		_pendingInputs.Clear();
	}
}
