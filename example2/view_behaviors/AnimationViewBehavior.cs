using Godot;
using Massive;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.example2;

public partial class AnimationViewBehavior : ViewBehavior {
	[Export] private AnimatedSprite2D _animatedSprite;
	
	private DataSet<Animation> _animations;
	private Entity _entity;
	
	public override void OnEntityAssigned(World world, Entity entity) {
		_entity = entity;
		_animations = world.DataSet<Animation>();
	}
	public override void OnEntityRemoved() {
		_animations = null;
		_entity = Entity.Dead;
	}

	public override void _Process(double delta) {
		if (!_animations.Has(_entity.Id)) {
			return;
		}
		
		var animation = _animations.Get(_entity.Id);
		var frameCount = _animatedSprite.SpriteFrames.GetFrameCount(_animatedSprite.Animation);
		var frame = (int)((float)(animation.CurrentTick * frameCount) / animation.TickDuration);
		_animatedSprite.Frame = frame;
	}
}