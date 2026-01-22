using Godot;
using Massive;
using Massive.Physics;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;
using massivegodotintegration.example.assets.godot_plush;
using Fixed64;

namespace massivegodotintegration.example;

public partial class PlayerViewBehavior : ViewBehavior {
	[Export] private GodotPlushSkin _plushSkin;
	
	private DataSet<RigidBody> _rigidBodies;
	private Entity _entity;
	
	public override void OnEntityAssigned(World world, Entity entity) {
		_entity = entity;
		_rigidBodies = world.DataSet<RigidBody>();
	}
	public override void OnEntityRemoved() {
		_rigidBodies = null;
		_entity = Entity.Dead;
	}

	public override void _PhysicsProcess(double delta) {
		if (!_rigidBodies.Has(_entity.Id)) {
			return;
		}
		
		var rigidBody = _rigidBodies.Get(_entity.Id);
		
		if (rigidBody.Velocity.Y > FP.Half) {
			_plushSkin.SetState("jump");
		}
		else if (rigidBody.Velocity.Y < -FP.Half) {
			_plushSkin.SetState("fall");
		}
		else if (!FMath.ApproximatelyEqual(rigidBody.Velocity.X, FP.Zero) || !FMath.ApproximatelyEqual(rigidBody.Velocity.Z, FP.Zero)) {
			_plushSkin.SetState("run");
		}
		else {
			_plushSkin.SetState("idle");
		}
	}
}