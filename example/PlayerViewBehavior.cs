using Godot;
using Massive;
using Massive.Physics;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;
using massivegodotintegration.example.assets.godot_plush;
using Fixed64;

namespace massivegodotintegration.example;

public partial class PlayerViewBehavior : ViewBehavior {
	[Export] private GodotPlushSkin _plushSkin;
	
	public PhysicsWorld PhysicsWorld { get; set; }
	
	private DataSet<PhysicsBody> _physicsBodies;
	private Entity _entity;
	
	public override void OnEntityAssigned(World world, Entity entity) {
		PhysicsWorld = PhysicsWorld.Instance;
		_entity = entity;
		_physicsBodies = world.DataSet<PhysicsBody>();
	}
	public override void OnEntityRemoved() {
		_physicsBodies = null;
		_entity = Entity.Dead;
	}

	public override void _PhysicsProcess(double delta) {
		if (!_physicsBodies.Has(_entity.Id)) {
			return;
		}
		
		var physicsBody = _physicsBodies.Get(_entity.Id);
		
		var bodyReference = PhysicsWorld.Simulation.Bodies[physicsBody.BodyHandle];
		
		if (bodyReference.Velocity.Linear.Y > 0.5f) {
			_plushSkin.SetState("jump");
		}
		else if (bodyReference.Velocity.Linear.Y < -0.5f) {
			_plushSkin.SetState("fall");
		}
		else if (!FMath.ApproximatelyEqual(bodyReference.Velocity.Linear.X.ToFP(), FP.Zero) || !FMath.ApproximatelyEqual(bodyReference.Velocity.Linear.Z.ToFP(), FP.Zero)) {
			_plushSkin.SetState("run");
		}
		else {
			_plushSkin.SetState("idle");
		}
	}
}