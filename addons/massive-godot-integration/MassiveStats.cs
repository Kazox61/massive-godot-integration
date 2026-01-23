using Godot;
using Massive.Netcode;

namespace massivegodotintegration.addons.massive_godot_integration;

public partial class MassiveStats : Node {
	[Export] private Label _entityCountLabel;

	private Session _session;
	
	public void Initialize(Session session) {
		_session = session;
	}

	public override void _PhysicsProcess(double delta) {
		if (_session == null) {
			return;
		}
		
		_entityCountLabel.Text = _session.World.Entities.Count.ToString();
	}
}