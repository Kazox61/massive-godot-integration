using Godot;
using Massive;

namespace massivegodotintegration.example2;

public partial class PlayerScore : CanvasLayer {
	[Export] private Label _player1Score;
	[Export] private Label _player2Score;
	[Export] private RetroTankRunner _game;
	
	public override void _Process(double delta) {
		_game.Session.World.ForEach((Entity entity, ref Tank tank) => {
			switch (tank.InputChannel) {
				case 0:
					_player1Score.Text = tank.Score.ToString();
					break;
				case 1:
					_player2Score.Text = tank.Score.ToString();
					break;
			}
		});
	}
}