using System.Globalization;
using Godot;
using Massive;

namespace massivegodotintegration.addons.massive_godot_integration;

public partial class MassiveStats : PanelContainer {
	[Export] private Label _fpsLabel;
	[Export] private Label _entityCountLabel;

	private World _world;
	
	public void Initialize(World world) {
		_world = world;
	}

	public override void _PhysicsProcess(double delta) {
		_entityCountLabel.Text = _world.Entities.Count.ToString();
		_fpsLabel.Text = Engine.GetFramesPerSecond().ToString(CultureInfo.InvariantCulture);
	}
}