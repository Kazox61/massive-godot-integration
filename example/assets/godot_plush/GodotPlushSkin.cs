using Godot;

namespace massivegodotintegration.example.assets.godot_plush;

public partial class GodotPlushSkin : Node3D {
	[Export] public MeshInstance3D GodotPlushMesh;
	[Export] public AnimationTree AnimationTree;
	
	public AnimationNodeStateMachinePlayback StateMachine;
	
	[Signal]
	public delegate void WavedEventHandler();

	private float _tilt;
	public float Tilt {
		get => _tilt;
		set {
			_tilt = Mathf.Clamp(value, -1f, 1f);
			AnimationTree.Set("parameters/AddTilt/add_amount", Mathf.Abs(_tilt));
			AnimationTree.Set("parameters/TiltAmount/blend_position", _tilt);
		}
	}

	private float _squashStretch;
	public float SquashStretch {
		get => _squashStretch;
		set {
			_squashStretch = value;
			var negative = 1f + (1f - _squashStretch);
			GodotPlushMesh.Scale = new Vector3(negative, _squashStretch, negative);
		}
	}

	public override void _Ready() {
		StateMachine = AnimationTree.Get("parameters/StateMachine/playback").As<AnimationNodeStateMachinePlayback>();
	}

	public void SetState(StringName stateName) {
		StateMachine.Travel(stateName);
	}

	public void Wave() {
		EmitSignalWaved();
		AnimationTree.Set("parameters/WaveOneShot/request", Variant.From(AnimationNodeOneShot.OneShotRequest.Fire));
	}
	
	public bool IsWaving() {
		return AnimationTree.Get("parameters/WaveOneShot/active").AsBool();
	}
}