using System.Threading.Tasks;

namespace Massive.Netcode;

public static class TaskExtensions {
	// https://www.meziantou.net/fire-and-forget-a-task-in-dotnet.htm
	public static void Forget(this Task task) {
		if (!task.IsCompleted || task.IsFaulted) {
			_ = ForgetAwaited(task);
		}

		return;

		static async Task ForgetAwaited(Task task) {
			await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
		}
	}
}