using AcademyAgain.Models;

namespace AcademyAgain.Helpers
{
	public static class ScheduleConflictChecker
	{
		/// <summary>
		/// Находит пересечения по времени (дата + интервал [start, start+duration])
		/// у одной группы или одного преподавателя.
		/// </summary>
		public static List<string> FindConflicts(
			AcademyAgainContext context,
			DateTime? date,
			TimeSpan? startTime,
			int durationMin,
			int? groupId,
			int? teacherId,
			long excludeLessonId)
		{
			var conflicts = new List<string>();
			if (date is null || startTime is null)
			{
				return conflicts;
			}

			var dateValue = date.Value.Date;
			var start = startTime.Value;
			var end = start + TimeSpan.FromMinutes(Math.Max(durationMin, 1));

			var candidates = context.Schedule
				.Where(s => s.date == dateValue && s.lesson_id != excludeLessonId)
				.AsEnumerable()
				.Where(s => s.time.HasValue &&
					((groupId is null || s.group == groupId.Value) ||
					 (teacherId is null || s.teacher == teacherId.Value)));

			foreach (var s in candidates)
			{
				var sStart = s.time!.Value;
				var sEnd = sStart + TimeSpan.FromMinutes(s.duration_min);
				if (start < sEnd && sStart < end)
				{
					var atSameGroup = groupId is not null && s.group == groupId.Value;
					var atSameTeacher = teacherId is not null && s.teacher == teacherId.Value;
					string message = atSameGroup && atSameTeacher
						? $"В {sStart:hh\\:mm} уже есть занятие для этой группы и преподавателя."
						: atSameGroup
							? $"В {sStart:hh\\:mm} у группы уже есть занятие."
							: $"В {sStart:hh\\:mm} преподаватель уже ведёт другое занятие.";
					conflicts.Add(message);
				}
			}

			return conflicts.Distinct().ToList();
		}
	}
}