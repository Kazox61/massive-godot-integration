using System.Collections.Generic;
using Godot;
using Massive;
using massivegodotintegration.addons.massive_godot_integration.Components;
using Mathematics.Fixed;

namespace massivegodotintegration.addons.massive_godot_integration;

public class NodeSynchronizer {
	public World World;
	public readonly Dictionary<Entity, Node> EntityNodeMap = new();

	public NodeSynchronizer(World world) {
		World = world;
	}

	public void Update() {
		var tree = Engine.GetMainLoop() as SceneTree;
		if (tree == null) {
			return;
		}

		var aliveEntities = new HashSet<Entity>();
		World.ForEach((Entity entity, ref ViewAsset viewAsset) => {
			aliveEntities.Add(entity);
			
			var hasNode = EntityNodeMap.TryGetValue(entity, out var node);
			if (!hasNode) {
				var packedScene = GD.Load<PackedScene>(viewAsset.PackedScenePath);
				if (packedScene == null) {
					GD.PrintErr($"Failed to load PackedScene at path: {viewAsset.PackedScenePath}");
					return;
				}

				var instance = packedScene.Instantiate();
				if (instance == null) {
					GD.PrintErr($"Failed to instantiate PackedScene at path: {viewAsset.PackedScenePath}");
					return;
				}

				tree.Root.AddChild(instance);
				EntityNodeMap[entity] = instance;
				node = instance;
			}

			if (hasNode && viewAsset.PackedScenePath != node.SceneFilePath) {
				node.QueueFree();

				var packedScene = GD.Load<PackedScene>(viewAsset.PackedScenePath);
				if (packedScene == null) {
					GD.PrintErr($"Failed to load PackedScene at path: {viewAsset.PackedScenePath}");
					return;
				}

				var instance = packedScene.Instantiate();
				if (instance == null) {
					GD.PrintErr($"Failed to instantiate PackedScene at path: {viewAsset.PackedScenePath}");
					return;
				}

				tree.Root.AddChild(instance);
				EntityNodeMap[entity] = instance;
				node = instance;
			}

			if (entity.Has<Transform>() && node is Node3D node3D) {
				var transform = entity.Get<Transform>();
				node3D.Position = new Vector3(transform.Position.X.ToFloat(), transform.Position.Y.ToFloat(), transform.Position.Z.ToFloat());
			}
		});

		foreach (var (entity, existingNode) in EntityNodeMap) {
			if (!aliveEntities.Contains(entity)) {
				existingNode.QueueFree();
				EntityNodeMap.Remove(entity);
			}
		}
	}
}