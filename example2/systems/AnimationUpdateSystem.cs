using Massive;
using massivegodotintegration.addons.massive_godot_integration;

namespace massivegodotintegration.example2;

public class AnimationUpdateSystem : RetroTankSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity entity, ref Animation animation) => {
			if (animation.CurrentTick >= animation.TickDuration) {
				if (animation.DestroyOnCompletion) {
					entity.Destroy();
				}
				else {
					entity.Remove<Animation>();
				}
				
				return;
			}
			
			animation.CurrentTick += 1;
		});
	}
}