using System;
using System.Collections.Generic;
using Fixed;
using Fixed32;
using Godot;

namespace massivegodotintegration.physics_test;

[Tool]
public partial class PhysicsTest : Node3D {
	[Export] public int BoxMinSize = 1;
	[Export] public int BoxMaxSize = 2;
	[Export] public int BoxAmount = 2000;
	[Export] public int ZoneDimensions = 50;
	[Export] public int RayLength = 100;
	[Export] public int Depth = 19;
	[Export] public Node3D From;

	private struct TestCollider {
		public int ProxyId;
		public FAABB AABB;
		public ulong LayerMask;
	}

	private readonly DynamicTree _tree = new();
	private readonly List<TestCollider> _colliders = [];
	private readonly List<ulong> _hits = [];

	private readonly Random _rng = new(123);

	public override void _Ready() {
		GenerateColliders(BoxAmount);
	}

	public override void _Process(double delta) {
		DrawColliders();
		RayCast();
	}

	private void GenerateColliders(int count) {
		for (var i = 0; i < count; i++) {
			var halfZone = ZoneDimensions / 2f;
			var center = new FVector3(
				(_rng.NextSingle() * ZoneDimensions - halfZone).ToFP(),
				(_rng.NextSingle() * ZoneDimensions - halfZone).ToFP(),
				(_rng.NextSingle() * ZoneDimensions - halfZone).ToFP()
			);

			var size = new Vector3(
				_rng.Next(BoxMinSize, BoxMaxSize + 1) * 0.5f,
				_rng.Next(BoxMinSize, BoxMaxSize + 1) * 0.5f,
				_rng.Next(BoxMinSize, BoxMaxSize + 1) * 0.5f
			);

			var aabb = new FAABB(
				new FVector3(center.X - size.X.ToFP(), center.Y - size.Y.ToFP(), center.Z - size.Z.ToFP()),
				new FVector3(center.X + size.X.ToFP(), center.Y + size.Y.ToFP(), center.Z + size.Z.ToFP())
			);

			var collider = new TestCollider {
				AABB = aabb,
				LayerMask = 1,
			};

			collider.ProxyId = _tree.CreateProxy(aabb, collider.LayerMask, (ulong)_colliders.Count);
			_colliders.Add(collider);
		}
	}

	private void DrawColliders() {
		foreach (var collider in _colliders) {
			var min = new Vector3(collider.AABB.LowerBound.X.ToFloat(), collider.AABB.LowerBound.Y.ToFloat(),
				collider.AABB.LowerBound.Z.ToFloat());
			var max = new Vector3(collider.AABB.UpperBound.X.ToFloat(), collider.AABB.UpperBound.Y.ToFloat(),
				collider.AABB.UpperBound.Z.ToFloat());
			var center = (min + max) * 0.5f;
			var size = max - min;

			DebugDraw3D.DrawBox(center, Quaternion.Identity, size, Colors.White);
		}
	}

	private void RayCast() {
		if (!GetViewport().GetMousePosition().IsFinite()) {
			return;
		}

		/*
		var camera = GetViewport().GetCamera3D();
		if (camera == null)
			return;

		var mouse = GetViewport().GetMousePosition();

		var rayOrigin = camera.ProjectRayOrigin(mouse);
		var rayDir = camera.ProjectRayNormal(mouse).Normalized();
		*/
		
		var rayOrigin = From.GlobalPosition;// Full rotation of the node
		var basis = From.GlobalTransform.Basis;

		var rayDir = -basis.Z.Normalized();


		_hits.Clear();

		var input = new DynamicTree.RayCastInput {
			Origin = new FVector3(rayOrigin.X.ToFP(), rayOrigin.Y.ToFP(), rayOrigin.Z.ToFP()),
			Direction = new FVector3(rayDir.X.ToFP(), rayDir.Y.ToFP(), rayDir.Z.ToFP()),
			MaxFraction = RayLength.ToFP()
		};

		_tree.RayCast(input, 1, RayCallback, null);    
		
		GD.Print($"HitCount {_hits.Count}");
		foreach (var userData in _hits) {
			var collider = _colliders[(int)userData];
			var center = (collider.AABB.LowerBound + collider.AABB.UpperBound) * FP.Half;
			DebugDraw3D.DrawBox(
				new Vector3(center.X.ToFloat(), center.Y.ToFloat(), center.Z.ToFloat()),
				Quaternion.Identity,
				Vector3.One * 1.5f, 
				Colors.Red
			);
		}
	}

	private FP RayCallback(ref DynamicTree.RayCastInput input, int nodeId, ulong userData, object _) {
		if (_hits.Count >= Depth) {
			return FP.MaxValue;
		}

		_hits.Add(userData);

		var collider = _colliders[(int)userData];
		var center = (collider.AABB.LowerBound + collider.AABB.UpperBound) * FP.Half;

		var toCenter = center - input.Origin;
		var t = FVector3.Dot(toCenter, input.Direction);

		return FP.MaxValue;
	}
}