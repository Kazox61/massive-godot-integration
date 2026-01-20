using System;
using System.Collections.Generic;
using Godot;
using massivegodotintegration.example.input;

namespace Massive.Netcode;

public class Server {
	private readonly ITransportHost _transportHost;
	private readonly Session _session;
	private readonly int _tickRate;
	private int _currentTick;
	private readonly int _maximumDelayedInputTicks;
	private double _accumulator;
	private int _lastApprovedTick;

	private int ConnectedChannels => _connectedSockets.Count;
	private readonly Dictionary<int, ISocket> _connectedSockets = new();
	private readonly List<(int tick, int inputChannel, IInput input)> _pendingInputs = [];
	private int _nextChannelId;
	private int NextChannelId => _nextChannelId++;
	
	private readonly MessageSerializer _messageSerializer = new();

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
			var connectionAdded = _connectedSockets.TryAdd(channelId, connection);
			if (!connectionAdded) {
				throw new Exception("Failed to add new connection to channel connections.");
			}
		}

		foreach (var (inputChannel, socket) in _connectedSockets) {
			while (socket.TryReceive(out var payload)) {
				var message = _messageSerializer.CreateMessage(payload.ToArray());
				switch (message) {
					case PingMessage pingMessage:
						var messageBytes = _messageSerializer.CreateBytes(
							new PongMessage {
								ClientStartTime = pingMessage.ClientStartTime
							}
						);
						socket.Send(messageBytes);
						break;
					case InputMessage inputMessage2:
						if (inputMessage2.Tick < _lastApprovedTick) {
							continue;
						}
				
						_session.Inputs.SetAt(inputMessage2.Tick, inputChannel, (PlayerInput)inputMessage2.Input);
						_pendingInputs.Add((inputMessage2.Tick, inputChannel, inputMessage2.Input));
						break;
				}
			}
		}
		
		if (ConnectedChannels < 2) { // lobby with 2 players
			return;
		}
		
		ProcessTick(deltaTime);

		_session.Inputs.DiscardUpTo(_lastApprovedTick);
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
			var allInputs = _session.Inputs.GetAllInputsAt<PlayerInput>(unapprovedTick);
			// Ensure there are 2 connected clients before approving ticks (lobby with 2 players)
			var hasAllInputs = allInputs.UsedChannels >= ConnectedChannels && ConnectedChannels >= 2;
			if (!hasAllInputs) {
				break;
			}
			
			_lastApprovedTick = unapprovedTick;
		}
		
		foreach (var (inputChannel, socket) in _connectedSockets) {
			var messageBytes = _messageSerializer.CreateBytes(
				new TickSyncMessage {
					InputChannel = inputChannel,
					ApprovedTick = _lastApprovedTick,
					ServerTime = (float)(_currentTick / (double)_tickRate),
					Inputs = _pendingInputs
				}
			);
			socket.Send(messageBytes);
		}
		
		_pendingInputs.Clear();
	}
}
