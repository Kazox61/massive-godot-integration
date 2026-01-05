using System;
using Massive;
using massivegodotintegration.addons.massive_godot_integration.components;

namespace massivegodotintegration.addons.massive_godot_integration.synchronizer;

public class GodotEntitySynchronization : IDisposable {
	private readonly World _world;
	private readonly EntityViewSynchronizer _entityViewSynchronizer;

	public GodotEntitySynchronization(World world) {
		_world = world;
		_entityViewSynchronizer = new EntityViewSynchronizer(world);
	}

	public void SubscribeViews() {
		_world.DataSet<ViewAsset>().AfterAdded += OnAfterViewAdded;
		_world.DataSet<ViewAsset>().BeforeRemoved += OnBeforeViewRemoved;
	}

	public void UnsubscribeViews() {
		_world.DataSet<ViewAsset>().AfterAdded -= OnAfterViewAdded;
		_world.DataSet<ViewAsset>().BeforeRemoved -= OnBeforeViewRemoved;
	}

	public void SynchronizeViews() {
		_entityViewSynchronizer.SynchronizeAll();
	}

	private void OnAfterViewAdded(int entityId) {
		_entityViewSynchronizer.SynchronizeView(entityId);
	}

	private void OnBeforeViewRemoved(int entityId) {
		_entityViewSynchronizer.DestroyView(entityId);
	}

	public void Dispose() {
		UnsubscribeViews();
	}
}