using TODOList.Models;

namespace TODOList.Services
{
	public class DataService
	{
		public List<TodoItem> Tasks { get; set; } = new();
		public List<Reminder> Reminders { get; set; } = new();
		public List<Category> Categories { get; set; } = new();
		public List<FocusLog> Logs { get; set; } = new();

		private TimerSession _timer = new();
		public TimerSession Timer => _timer;

		private System.Threading.Timer? _timerTick;
		private DateTime _lastTick = DateTime.Now;

		public event Action? OnChange;
		public event Action? OnTimerTick;
		public event Action<Reminder>? OnReminderDue;
		public event Action? OnPersist;

		public DataService()
		{
			Seed();
			EnsureDefaultCategories();
			_timerTick = new System.Threading.Timer(_ => Tick(), null, 0, 1000);
		}

		private void Seed()
		{
			Tasks.Add(new TodoItem
			{
				Title = "Изучить Blazor",
				Description = "Разобраться с компонентами и рендер-режимами",
				Priority = Priority.High,
				Category = "Обучение",
				DueDate = DateTime.Now.AddDays(3)
			});
			Tasks.Add(new TodoItem
			{
				Title = "Настроить стили",
				Description = "Тёмная металкор-тема для органайзера",
				Priority = Priority.High,
				Category = "Дизайн",
				DueDate = DateTime.Now.AddDays(1)
			});
			Tasks.Add(new TodoItem
			{
				Title = "Помыть посуду",
				Priority = Priority.Low,
				Category = "Дом"
			});

			Reminders.Add(new Reminder
			{
				Title = "Обед",
				Message = "Не забудь поесть",
				TriggerAt = DateTime.Now.AddMinutes(2)
			});
		}

		public void AddTask(TodoItem task)
		{
			Tasks.Insert(0, task);
			NotifyChange();
			NotifyPersist();
		}

		public void UpdateTask(TodoItem task)
		{
			var index = Tasks.FindIndex(t => t.Id == task.Id);
			if (index >= 0)
			{
				Tasks[index] = task;
				NotifyChange();
				NotifyPersist();
			}
		}

		public void ToggleTask(Guid id)
		{
			var task = Tasks.FirstOrDefault(t => t.Id == id);
			if (task != null)
			{
				task.IsCompleted = !task.IsCompleted;
				if (task.IsCompleted)
				{
					Logs.Add(new FocusLog { Kind = LogKind.TaskDone, Timestamp = DateTime.Now, TaskId = task.Id.ToString() });
					var reminder = Reminders.FirstOrDefault(r => r.LinkedTaskId == task.Id);
					if (reminder != null && !reminder.IsFired)
					{
						reminder.IsFired = true;
					}
				}
				NotifyChange();
				NotifyPersist();
			}
		}

		public void RemoveTask(Guid id)
		{
			Tasks.RemoveAll(t => t.Id == id);
			NotifyChange();
			NotifyPersist();
		}

		public void AddReminder(Reminder reminder)
		{
			Reminders.Add(reminder);
			NotifyChange();
			NotifyPersist();
		}

		public void RemoveReminder(Guid id)
		{
			Reminders.RemoveAll(r => r.Id == id);
			NotifyChange();
			NotifyPersist();
		}

		public void MarkReminderFired(Guid id)
		{
			var reminder = Reminders.FirstOrDefault(r => r.Id == id);
			if (reminder != null && !reminder.IsFired)
			{
				reminder.IsFired = true;
				NotifyChange();
				NotifyPersist();
			}
		}

		public void EnsureDefaultCategories()
		{
			var palette = new[] { "#ff4444", "#ffaa00", "#4ace6a", "#4ab8ff", "#c084fc", "#ff7eb6", "#7ee7d3" };
			var icons = new[] { "bi-fire", "bi-lightning-charge", "bi-moon-stars", "bi-tag", "bi-briefcase", "bi-book", "bi-house" };
			int idx = 0;

			foreach (var task in Tasks)
			{
				var cat = task.Category;
				if (string.IsNullOrWhiteSpace(cat)) continue;
				if (Categories.Any(c => string.Equals(c.Name, cat, StringComparison.OrdinalIgnoreCase))) continue;
				Categories.Add(new Category
				{
					Name = cat.Trim(),
					Color = palette[idx % palette.Length],
					Icon = icons[idx % icons.Length]
				});
				idx++;
			}

			if (!Categories.Any())
			{
				Categories.Add(new Category { Name = "Общее", Color = "#888888", Icon = "bi-tag", IsDefault = true });
			}
		}

		public void AddCategory(Category category)
		{
			Categories.Add(category);
			NotifyChange();
			NotifyPersist();
		}

		public void UpdateCategory(Category category)
		{
			var index = Categories.FindIndex(c => c.Id == category.Id);
			if (index >= 0)
			{
				Categories[index] = category;
				NotifyChange();
				NotifyPersist();
			}
		}

		public void RemoveCategory(Guid id)
		{
			var cat = Categories.FirstOrDefault(c => c.Id == id);
			if (cat == null) return;
			Categories.Remove(cat);
			foreach (var task in Tasks.Where(t => t.Category == cat.Name))
			{
				task.Category = null;
			}
			NotifyChange();
			NotifyPersist();
		}

		public void StartTimer(int durationSeconds, TimerType type)
		{
			_timer.Type = type;
			_timer.DurationSeconds = durationSeconds;
			_timer.RemainingSeconds = durationSeconds;
			_timer.IsRunning = true;
			_timer.IsBreak = false;
			_lastTick = DateTime.Now;
			NotifyChange();
			NotifyPersist();
		}

		public void StopTimer()
		{
			_timer.IsRunning = false;
			NotifyChange();
		}

		public void ResetTimer()
		{
			_timer.IsRunning = false;
			_timer.RemainingSeconds = _timer.DurationSeconds;
			_timer.IsBreak = false;
			NotifyChange();
			NotifyPersist();
		}

		public void SetPomodoroPhase(int seconds, bool isBreak)
		{
			_timer.DurationSeconds = seconds;
			_timer.RemainingSeconds = seconds;
			_timer.IsBreak = isBreak;
			NotifyChange();
			NotifyPersist();
		}

		private void Tick()
		{
			CheckReminders();

			if (_timer.IsRunning)
			{
				var now = DateTime.Now;
				var elapsed = (int)(now - _lastTick).TotalSeconds;
				_lastTick = now;

				if (elapsed > 0)
				{
					_timer.RemainingSeconds -= elapsed;
					if (_timer.RemainingSeconds <= 0)
					{
						_timer.RemainingSeconds = 0;
						_timer.IsRunning = false;

						if (_timer.Type == TimerType.Pomodoro)
						{
							var nextBreak = !_timer.IsBreak;
							_timer.IsBreak = nextBreak;
							_timer.DurationSeconds = nextBreak ? 5 * 60 : 25 * 60;
							_timer.RemainingSeconds = _timer.DurationSeconds;
							try
							{
								OnReminderDue?.Invoke(new Reminder
								{
									Title = nextBreak ? "Перерыв!" : "Работа!",
									Message = nextBreak
										? "Помодоро завершён. Отдохни 5 минут."
										: "Перерыв закончен. Продолжай работу!"
								});
							}
							catch { }
							if (nextBreak)
							{
								Logs.Add(new FocusLog { Kind = LogKind.Pomodoro, Timestamp = DateTime.Now });
								NotifyPersist();
							}
						}
						else
						{
							try
							{
								OnReminderDue?.Invoke(new Reminder
								{
									Title = "Таймер завершён",
									Message = "Обратный отсчёт закончился."
								});
							}
							catch { }
						}
					}
					OnTimerTick?.Invoke();
					NotifyChange();
				}
			}
		}

		private void CheckReminders()
		{
			var now = DateTime.Now;
			var due = Reminders.Where(r => !r.IsFired && r.TriggerAt <= now).ToList();
			foreach (var reminder in due)
			{
				reminder.IsFired = true;
				try { OnReminderDue?.Invoke(reminder); }
				catch { }
			}
			if (due.Count > 0)
			{
				NotifyChange();
				NotifyPersist();
			}
		}

		public void NotifyChange() => OnChange?.Invoke();

		public void NotifyPersist() => OnPersist?.Invoke();

		public string SerializeState()
		{
			try
			{
				var state = new AppState
				{
					Tasks = Tasks,
					Reminders = Reminders,
					Categories = Categories,
					Logs = Logs,
					TimerPomodoro = _timer.Type == TimerType.Pomodoro,
					TimerDurationSeconds = _timer.Type == TimerType.Pomodoro
						? (_timer.IsBreak ? 5 * 60 : 25 * 60)
						: _timer.DurationSeconds
				};
				return System.Text.Json.JsonSerializer.Serialize(state);
			}
			catch
			{
				return "{}";
			}
		}

		public void RestoreState(string json)
		{
			try
			{
				var state = System.Text.Json.JsonSerializer.Deserialize<AppState>(json);
				if (state == null) return;

				Tasks = state.Tasks ?? new List<TodoItem>();
				Reminders = state.Reminders ?? new List<Reminder>();
				Categories = state.Categories ?? new List<Category>();
				Logs = state.Logs ?? new List<FocusLog>();
				EnsureDefaultCategories();

				if (state.TimerPomodoro)
				{
					_timer.Type = TimerType.Pomodoro;
					_timer.DurationSeconds = 25 * 60;
					_timer.RemainingSeconds = 25 * 60;
				}
				else
				{
					_timer.Type = TimerType.Countdown;
					var secs = state.TimerDurationSeconds > 0 ? state.TimerDurationSeconds : 25 * 60;
					_timer.DurationSeconds = secs;
					_timer.RemainingSeconds = secs;
				}
				_timer.IsRunning = false;
				_timer.IsBreak = false;

				NotifyChange();
			}
			catch
			{
				// corrupted data — keep defaults
			}
		}
	}
}
