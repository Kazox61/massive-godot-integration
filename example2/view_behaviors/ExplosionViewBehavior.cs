using Godot;
using Massive;
using massivegodotintegration.addons.massive_godot_integration.synchronizer;

namespace massivegodotintegration.example2;

public partial class ExplosionViewBehavior : ViewBehavior {
	[Export] private AnimatedSprite2D _animatedSprite;
	
	public override void OnEntityAssigned(World world, Entity entity) {
		
	}
	
	public override void OnEntityRemoved() {
		
	}
}