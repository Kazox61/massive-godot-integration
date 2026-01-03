using System;
using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.Components;
using massivegodotintegration.addons.massive_godot_integration;
using massivegodotintegration.Example.Components;
using massivegodotintegration.Example.Systems;

namespace massivegodotintegration.Example;

public partial class TestWorld : Node3D {
	public Session Session;

	public int TargetTick;
	public NodeSynchronizer NodeSynchronizer;
	public readonly SimulationTickTracker TickTracker = new();
	
	public override void _Ready() {
		Session = new Session();

		Session.Systems
			.New<MovementSystem>()
			.Build(Session.World)
			.Inject(Session);

		Session.Simulations.Add(new SystemsSimulation(Session.Systems));
		Session.Simulations.Add(TickTracker);
		
		var player = Session.World.CreateEntity(new Player { InputChannel = 0 });
		player.Set(new Transform { PositionX = 6 });
		player.Set(new ViewAsset { PackedScenePath = "res://Example/player.tscn" });
		
		Session.World.SaveFrame();

		NodeSynchronizer = new NodeSynchronizer(Session.World);
	}

	public override void _PhysicsProcess(double delta) {
		// Randomize Rollback Ticks
		Session.ChangeTracker.NotifyChange(MathUtils.Max(0, TargetTick - Random.Shared.Next(0, 10)));
		TickTracker.Restart();
		Session.Loop.FastForwardToTick(TargetTick);
		// GD.Print($"Ticks Processed This Frame: {TickTracker.TicksAmount}");
		NodeSynchronizer.Update();
		TargetTick++;
	}
}