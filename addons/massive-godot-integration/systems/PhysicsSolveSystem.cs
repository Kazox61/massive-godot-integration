using Massive;
using Massive.Netcode;
using massivegodotintegration.addons.massive_godot_integration.components;
using Mathematics.Fixed;

namespace massivegodotintegration.addons.massive_godot_integration.systems;

public class PhysicsSolveSystem : NetSystem, IUpdate {
	private static readonly FP Slop = 0.01f.ToFP();
	private static readonly FP Percent = 0.8f.ToFP();

	public void Update() {
		World.ForEach((Entity entity, ref Contact contact) => {
			ref var tA = ref World.Get<Transform>(contact.EntifierA);
			ref var tB = ref World.Get<Transform>(contact.EntifierB);

			ref var bA = ref World.Get<RigidBody>(contact.EntifierA);
			ref var bB = ref World.Get<RigidBody>(contact.EntifierB);

			var normal = contact.Collision.PenetrationNormal;
			var depth = contact.Collision.PenetrationDepth;

			var invMassA = bA.InverseMass;
			var invMassB = bB.InverseMass;

			if (invMassA + invMassB == FP.Zero) {
				entity.Destroy();
				return;
			}

			var correction = normal * (FMath.Max(depth - Slop, FP.Zero) / (invMassA + invMassB)) * Percent;

			tA.Position -= correction * invMassA;
			tB.Position += correction * invMassB;

			var groundDot = 0.7f.ToFP();

			if (normal.Y > groundDot && bA.InverseMass > FP.Zero) {
				if (bA.Velocity.Y < FP.Zero) {
					bA.Velocity = new FVector3(
						bA.Velocity.X,
						FP.Zero,
						bA.Velocity.Z
					);
				}
			}

			if (-normal.Y > groundDot && bB.InverseMass > FP.Zero) {
				if (bB.Velocity.Y < FP.Zero) {
					bB.Velocity = new FVector3(
						bB.Velocity.X,
						FP.Zero,
						bB.Velocity.Z
					);
				}
			}

			var rv = bB.Velocity - bA.Velocity;
				var velAlongNormal = FVector3.Dot(rv, normal);

				if (velAlongNormal > FP.Zero) {
					entity.Destroy();
					return;
				}

				var restitution = FMath.Min(bA.Restitution, bB.Restitution);

				var j = -(FP.One + restitution) * velAlongNormal;
				j /= invMassA + invMassB;

				var impulse = j * normal;

				bA.Velocity -= impulse * invMassA;
				bB.Velocity += impulse * invMassB;

				entity.Destroy();
			});
		}
	}