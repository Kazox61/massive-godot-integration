using Fixed64;
using Godot;
using Massive;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.example2;

public partial class TankViewBehavior : ViewBehavior {
	[Export] private Node2D _body;
	[Export] private Node2D _gunAnchor;
	
	private DataSet<Tank> _tanks;
	private Entity _entity;
	
	public override void OnEntityAssigned(World world, Entity entity) {
		_entity = entity;
		_tanks = world.DataSet<Tank>();
	}
	public override void OnEntityRemoved() {
		_tanks = null;
		_entity = Entity.Dead;
	}

	public override void _PhysicsProcess(double delta) {
		if (!_tanks.Has(_entity.Id)) {
			return;
		}
		
		var tank = _tanks.Get(_entity.Id);
		
		_body.Rotation = FMath.Atan2(tank.MovementDirection.X, -tank.MovementDirection.Y).ToFloat();
		_gunAnchor.Rotation = FMath.Atan2(tank.GunDirection.X, -tank.GunDirection.Y).ToFloat();
	}
}