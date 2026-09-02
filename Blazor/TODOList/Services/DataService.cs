using TODOList.Models;

namespace TODOList.Services
{
	public class DataService
	{
		public List<TodoItem> Tasks { get; set; } = new();
		public List<Reminder> Reminders { get; set; } = new();

		private TimerSession _timer = new();
		public TimerSession Timer => _timer;

		private System.Threading.Timer? _timerTick;
		private DateTime _lastTick = DateTime.Now;

		public event Action? OnChange;
		public event Action? OnTimerTick;
		public event Action<Reminder>? OnReminderDue;

		public DataService()
		{
			Seed();
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
		}

		public void UpdateTask(TodoItem task)
		{
			var index = Tasks.FindIndex(t => t.Id == task.Id);
			if (index >= 0)
			{
				Tasks[index] = task;
				NotifyChange();
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
					var reminder = Reminders.FirstOrDefault(r => r.LinkedTaskId == task.Id);
					if (reminder != null && !reminder.IsFired)
					{
						reminder.IsFired = true;
					}
				}
				NotifyChange();
			}
		}

		public void RemoveTask(Guid id)
		{
			Tasks.RemoveAll(t => t.Id == id);
			NotifyChange();
		}

		public void AddReminder(Reminder reminder)
		{
			Reminders.Add(reminder);
			NotifyChange();
		}

		public void RemoveReminder(Guid id)
		{
			Reminders.RemoveAll(r => r.Id == id);
			NotifyChange();
		}

		public void MarkReminderFired(Guid id)
		{
			var reminder = Reminders.FirstOrDefault(r => r.Id == id);
			if (reminder != null && !reminder.IsFired)
			{
				reminder.IsFired = true;
				NotifyChange();
			}
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
		}

		public void SetPomodoroPhase(int seconds, bool isBreak)
		{
			_timer.DurationSeconds = seconds;
			_timer.RemainingSeconds = seconds;
			_timer.IsBreak = isBreak;
			NotifyChange();
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
							OnReminderDue?.Invoke(new Reminder
							{
								Title = nextBreak ? "Перерыв!" : "Работа!",
								Message = nextBreak
									? "Помодоро завершён. Отдохни 5 минут."
									: "Перерыв закончен. Продолжай работу!"
							});
						}
						else
						{
							OnReminderDue?.Invoke(new Reminder
							{
								Title = "Таймер завершён",
								Message = "Обратный отсчёт закончился."
							});
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
				OnReminderDue?.Invoke(reminder);
			}
			if (due.Count > 0)
			{
				NotifyChange();
			}
		}

		public void NotifyChange() => OnChange?.Invoke();
	}
}
