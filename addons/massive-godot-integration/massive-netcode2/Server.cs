using System;
using System.Collections.Generic;

namespace Massive.Netcode;

public class Server {
	private readonly ITransportHost _transportHost;
	private readonly Session _session;
	private readonly int _tickRate;
	private int _currentTick;
	private int _maximumDelayedInputTicks;
	private double _accumulator;
	private int _lastApprovedTick;

	private int ConnectedChannels => _channelConnections.Count;
	private readonly Dictionary<int, IConnection> _channelConnections = new();
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
			var connectionAdded = _channelConnections.TryAdd(NextChannelId, connection);
			if (!connectionAdded) {
				throw new Exception("Failed to add new connection to channel connections.");
			}
		}

		foreach (var (inputChannel, connection) in _channelConnections) {
			connection.Update();
			while (connection.TryDequeueMessage(out var message)) {
				HandleMessage(inputChannel, connection, message);
			}
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
		_session.Simulations.Update(_currentTick);
		
		if (_lastApprovedTick < _currentTick - _maximumDelayedInputTicks) {
			_lastApprovedTick = _currentTick - _maximumDelayedInputTicks;
		}

		for (var unapprovedTick = _lastApprovedTick; unapprovedTick < _currentTick; unapprovedTick++) {
			try {
				var allInputs = _session.Inputs.GetAllInputsAt<IInput>(unapprovedTick);
				var hasAllInputs = allInputs.UsedChannels >= ConnectedChannels;
				if (!hasAllInputs) {
					break;
				} 
			}
			catch (ArgumentOutOfRangeException) {
				// No inputs saved for this tick yet
				break;
			}
			
			_lastApprovedTick = unapprovedTick;
		}

		/*
		foreach (var connection in _channelConnections.Values) {
			connection.SendMessage(new TickSyncMessage {
				ApprovedTick = _lastApprovedTick,
				ServerTime = (float)(_currentTick / (double)_tickRate),
				ApprovedInputs = []
			});
		}
		*/
	}
	
	private void HandleMessage(int inputChannel, IConnection connection, NetMessage message) {
		switch (message) {
			case InputMessage inputMessage:
				foreach (var (tick, input) in inputMessage.PendingInputs) {
					if (tick < _currentTick - _maximumDelayedInputTicks || tick > _currentTick) {
						continue;
					}
					
					_session.Inputs.SetAt(tick, inputChannel, input);
				}
				
				// do we resend directly a TickSyncMessage here, or we do it in the tick loop or even in the update loop?
				break;
			case PingMessage pingMessage:
				connection.SendMessage(new PongMessage {
					ClientStartTime = pingMessage.ClientStartTime
				});
				break;
		}
	}
}