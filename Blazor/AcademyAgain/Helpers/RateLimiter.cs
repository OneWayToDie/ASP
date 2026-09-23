using System.Collections.Concurrent;

namespace AcademyAgain.Helpers
{
	/// <summary>
	/// Простой in-memory лимитер запросов (ключ -> окно попыток).
	/// Подходит для защиты форм (например, регистрации): N попыток за интервал.
	/// </summary>
	public sealed class RateLimiter
	{
		private sealed class Counter
		{
			public DateTime Start = DateTime.UtcNow;
			public int Count;
		}

		private readonly ConcurrentDictionary<string, Counter> _counters = new();
		private readonly int _max;
		private readonly TimeSpan _window;
		private readonly object _pruneLock = new();
		private int _checkCounter;
		private const int PruneEvery = 1024;

		public RateLimiter(int max = 5, TimeSpan? window = null)
		{
			_max = max;
			_window = window ?? TimeSpan.FromMinutes(10);
		}

		/// <summary>
		/// Пытается занять слот для ключа. Возвращает false, если лимит исчерпан.
		/// При успехе счётчик увеличивается (попытка засчитывается независимо от результата).
		/// </summary>
		public bool TryConsume(string key)
		{
			PruneIfNeeded();
			var counter = _counters.GetOrAdd(key, _ => new Counter());
			lock (counter)
			{
				var now = DateTime.UtcNow;
				if (now - counter.Start >= _window)
				{
					counter.Start = now;
					counter.Count = 0;
				}
				if (counter.Count >= _max)
				{
					return false;
				}
				counter.Count++;
				return true;
			}
		}

		/// <summary>Сбрасывает счётчики ключа (например, после успешного действия).</summary>
		public void Reset(string key)
		{
			_counters.TryRemove(key, out _);
		}

		private void PruneIfNeeded()
		{
			var n = Interlocked.Increment(ref _checkCounter);
			if (n % PruneEvery != 0)
			{
				return;
			}
			lock (_pruneLock)
			{
				var cutoff = DateTime.UtcNow - _window;
				foreach (var kv in _counters)
				{
					var c = kv.Value;
					lock (c)
					{
						if (cutoff >= c.Start)
						{
							_counters.TryRemove(kv.Key, out _);
						}
					}
				}
			}
		}
	}
}