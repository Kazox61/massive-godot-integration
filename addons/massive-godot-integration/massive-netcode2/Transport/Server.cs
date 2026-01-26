using System;
using System.Collections.Generic;
using Godot;

namespace Massive.Netcode;

public class Server {
	private readonly ITransportHost _transportHost;
	private readonly Session _session;
	private int _currentTick;
	private double _serverTime;

	private int ConnectedChannels => _connectedSockets.Count;
	private readonly Dictionary<int, ISocket> _connectedSockets = new();
	private readonly List<(int tick, int inputChannel, IInput input)> _playedInputs = [];
	private int _nextChannelId;
	private int NextChannelId => _nextChannelId++;
	
	private readonly MessageSerializer _messageSerializer = new();

	public Server(ITransportHost transportHost, SessionConfig config) {
		_transportHost = transportHost;
		_session = new Session(config);
		_currentTick = config.StartTick;
	}
	
	public void Start() {
		_transportHost.Start();
	}
	
	public void Update(double deltaTime) {
		_transportHost.Update();

		while (_transportHost.TryAccept(out var socket)) {
			var channelId = NextChannelId;
			var socketAdded = _connectedSockets.TryAdd(channelId, socket);
			if (!socketAdded) {
				throw new Exception("Failed to add new socket to connected sockets.");
			}
			
			var setupMessageBytes = _messageSerializer.CreateBytes(
				new SetupClientMessage {
					Seed = 1,
					InputChannel = channelId
				}
			);
			
			socket.Send(setupMessageBytes);
			GD.Print($"Client connected on channel {channelId}");
		}

		foreach (var (inputChannel, socket) in _connectedSockets) {
			while (socket.TryReceive(out var payload)) {
				var message = _messageSerializer.CreateMessage(payload.ToArray());
				switch (message) {
					case PingMessage pingMessage:
						var messageBytes = _messageSerializer.CreateBytes(
							new PongMessage {
								ClientStartTime = pingMessage.ClientStartTime,
								ServerTime = (float)_serverTime
							}
						);
						socket.Send(messageBytes);
						break;
					case InputMessage inputMessage2:
						if (inputMessage2.Tick < _currentTick) {
							GD.Print($"Input dropped from channel {inputChannel} with tick {inputMessage2.Tick}");
							continue;
						}
				
						_session.Inputs.SetAt(inputMessage2.Tick, inputChannel, (PlayerInput)inputMessage2.Input);
						break;
				}
			}
		}
		
		if (ConnectedChannels < 2) { // lobby with 2 players
			return;
		}
		
		ProcessTick(deltaTime);

		if (_playedInputs.Count != 0)
		{
			foreach (var (inputChannel, socket) in _connectedSockets) {
				var messageBytes = _messageSerializer.CreateBytes(
					new TickSyncMessage {
						ApprovedTick = _currentTick,
						Inputs = _playedInputs
					}
				);
				socket.Send(messageBytes);
			}
			
			_playedInputs.Clear();
		}

		_session.Inputs.DiscardUpTo(_currentTick);
	}
	
	private void ProcessTick(double deltaTime)
	{
		_serverTime += deltaTime;

		var targetTick = Mathf.RoundToInt(_serverTime * _session.Time.TickRate);
		_session.Inputs.PopulateUpTo(targetTick);

		while (_currentTick < targetTick)
		{
			_session.Simulations.Update(_currentTick);

			foreach (var (channel, input) in _session.Inputs.GetAllActualAt<PlayerInput>(_currentTick))
			{
				_playedInputs.Add((_currentTick, channel, input.Actual()));
			}

			_currentTick++;
		}
	}
}
