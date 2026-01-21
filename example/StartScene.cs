using Godot;

namespace massivegodotintegration.example;

public partial class StartScene : Node {
	[Export] public PackedScene ServerScene;
	[Export] public PackedScene ClientScene;

	public override void _Ready() {
		foreach (var argument in OS.GetCmdlineArgs()) {
			switch (argument) {
				case "--server":
					var server = ServerScene.Instantiate();
					AddChild(server);
					GetWindow().Title = "Massive Server";
					break;
				case "--client":
					var client = ClientScene.Instantiate();
					AddChild(client);
					GetWindow().Title = "Massive Client";
					break;
			}
		}
	}
}