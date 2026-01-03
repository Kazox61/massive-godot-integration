using Godot;
using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.Components;
using massivegodotintegration.Example.Components;
using massivegodotintegration.Example.Input;

namespace massivegodotintegration.Example.Systems;

public class MovementSystem : NetSystem, IUpdate {
	public void Update() {
		World.ForEach((Entity entity, ref Player player, ref Transform transform) => {
			var playerInput = Inputs.Get<PlayerInput>(player.InputChannel).LastActual();
			var direction = new Vector2(playerInput.DirectionX, playerInput.DirectionY).Normalized();
			transform.PositionX += direction.X * 0.1f;
			transform.PositionZ += direction.Y * 0.1f;

			if (playerInput.Kill) {
				entity.Destroy();
			}
		});
	}
}