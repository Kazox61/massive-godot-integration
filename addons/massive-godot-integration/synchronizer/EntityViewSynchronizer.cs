using Godot;
using Massive;
using Massive.Physics;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

public class EntityViewSynchronizer {
	private readonly DataSet<ViewInstance> _viewInstances = new();
	private readonly World _world;

	public EntityViewSynchronizer(World world) {
		_world = world;
	}

	public void SynchronizeAll() {
		var viewAssets = _world.DataSet<ViewAsset>();

		foreach (var id in _viewInstances) {
			var viewInstance = _viewInstances.Get(id);

			if (!viewAssets.Has(id) || viewAssets.Get(id).PackedScenePath != viewInstance.Asset.PackedScenePath) {
				RemoveViewInstance(id, viewInstance);
			}
		}
		
		foreach (var id in viewAssets) {
			if (_viewInstances.Has(id)) {
				continue;
			}

			var viewAsset = viewAssets.Get(id);
			AssignViewInstance(viewAsset, id);
		}
	}

	public void SynchronizeView(int entityId) {
		var viewAsset = _world.Get<ViewAsset>(entityId);

		if (_viewInstances.Has(entityId)) {
			var viewInstance = _viewInstances.Get(entityId);

			if (viewInstance.Asset.PackedScenePath == viewAsset.PackedScenePath) {
				return;
			}

			RemoveViewInstance(entityId, viewInstance);
		}
		
		AssignViewInstance(viewAsset, entityId);
	}
	
	public void DestroyView(int entityId) {
		if (_viewInstances.Has(entityId)) {
			RemoveViewInstance(entityId, _viewInstances.Get(entityId));
		}
	}
	
	private void AssignViewInstance(in ViewAsset viewAsset, int entityId) {
		var packedScene = GD.Load<PackedScene>(viewAsset.PackedScenePath);
		if (packedScene == null) {
			GD.PrintErr($"Failed to load PackedScene at path: {viewAsset.PackedScenePath}");
			return;
		}

		var entityView = packedScene.Instantiate();
		if (entityView == null) {
			GD.PrintErr($"Failed to instantiate PackedScene at path: {viewAsset.PackedScenePath}");
			return;
		}

		var entity = _world.GetEntity(entityId);
		if (entityView is IEntityView view) {
			view.AssignEntity(_world, entity);
		}
		else {
			GD.PrintErr($"Instantiated scene does not implement IEntityView: {viewAsset.PackedScenePath}");
		}

		var tree = Engine.GetMainLoop() as SceneTree;
		if (tree == null) {
			GD.PrintErr("Failed to get SceneTree.");
			return;
		}
		
		tree.Root.AddChild(entityView);
		_viewInstances.Set(entityId, new ViewInstance { Instance = entityView, Asset = viewAsset });
	}
	
	private void RemoveViewInstance(int entityId, ViewInstance viewInstance) {
		if (viewInstance.Instance is IEntityView view) {
			view.RemoveEntity();
		}
		viewInstance.Instance.Free();
		_viewInstances.Remove(entityId);
	}
}