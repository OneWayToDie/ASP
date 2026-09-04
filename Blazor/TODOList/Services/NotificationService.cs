namespace TODOList.Services
{
	public class ToastMessage
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Title { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public bool IsError { get; set; }
	}

	public class NotificationService
	{
		private readonly List<ToastMessage> _toasts = new();
		public IReadOnlyList<ToastMessage> Toasts => _toasts;

		public event Action? OnToastsChanged;

		public void Push(string title, string message, bool isError = false)
		{
			var toast = new ToastMessage { Title = title, Message = message, IsError = isError };
			_toasts.Add(toast);
			OnToastsChanged?.Invoke();

			_ = Task.Run(async () =>
			{
				try
				{
					await Task.Delay(5000);
					Dismiss(toast.Id);
				}
				catch { }
			});
		}

		public void Dismiss(Guid id)
		{
			_toasts.RemoveAll(t => t.Id == id);
			OnToastsChanged?.Invoke();
		}

		private readonly Dictionary<string, Func<string, string, Task>> _handlers = new();
		public IDisposable RegisterBroadcastHandler(string circuitId, Func<string, string, Task> handler)
		{
			_handlers[circuitId] = handler;
			return new Unsubscriber(() => _handlers.Remove(circuitId));
		}

		public async Task BroadcastAsync(string title, string message)
		{
			Push(title, message);
			foreach (var handler in _handlers.Values)
			{
				try { await handler(title, message); }
				catch { /* circuit may be gone */ }
			}
		}

		private class Unsubscriber : IDisposable
		{
			private readonly Action _unsubscribe;
			public Unsubscriber(Action unsubscribe) => _unsubscribe = unsubscribe;
			public void Dispose() => _unsubscribe();
		}
	}
}
