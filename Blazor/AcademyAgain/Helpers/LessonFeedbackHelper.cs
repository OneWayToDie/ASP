using AcademyAgain.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademyAgain.Helpers
{
	public sealed record FeedbackRow(long LessonId, int TeacherId, string TeacherName, string DisciplineName, string DateText, string TimeText);

	public static class LessonFeedbackHelper
	{
		public static async Task<List<FeedbackRow>> GetPendingAsync(AcademyAgainContext context, int studentId, int groupId)
		{
			var attendedIds = await context.Attendance.AsNoTracking()
				.Where(a => a.student == studentId && a.present)
				.Select(a => a.lesson)
				.ToListAsync();
			var ratedIds = await context.LessonFeedbacks.AsNoTracking()
				.Where(f => f.student_id == studentId)
				.Select(f => f.lesson_id)
				.ToListAsync();
			var ratedSet = ratedIds.ToHashSet();
			var pendingIds = attendedIds.Where(id => !ratedSet.Contains(id)).ToList();
			if (pendingIds.Count == 0)
			{
				return new List<FeedbackRow>();
			}

			var lessons = await context.Schedule.AsNoTracking()
				.Where(s => s.group == groupId && s.spent && pendingIds.Contains(s.lesson_id))
				.OrderByDescending(s => s.date)
				.ThenByDescending(s => s.time)
				.ToListAsync();

			var teacherIds = lessons.Select(l => l.teacher).Distinct().ToList();
			var disciplineIds = lessons.Select(l => l.discipline).Distinct().ToList();

			var teachers = teacherIds.Count > 0
				? await context.Teachers.AsNoTracking().Where(t => teacherIds.Contains(t.teacher_id)).ToDictionaryAsync(t => t.teacher_id)
				: new Dictionary<int, Teacher>();
			var disciplines = disciplineIds.Count > 0
				? await context.Disciplines.AsNoTracking().Where(d => disciplineIds.Contains(d.discipline_id)).ToDictionaryAsync(d => d.discipline_id)
				: new Dictionary<int, Discipline>();

			return lessons.Select(l => new FeedbackRow(
				l.lesson_id,
				l.teacher,
				TeacherName(teachers.GetValueOrDefault(l.teacher)),
				DisciplineName(disciplines.GetValueOrDefault(l.discipline)),
				l.date?.ToString("dd.MM.yyyy") ?? "—",
				l.time?.ToString(@"hh\:mm") ?? "—"
			)).ToList();
		}

		public static string TeacherName(Teacher? t)
			=> t is null ? "Преподаватель" : string.Join(" ", new[] { t.last_name, t.first_name, t.middle_name }.Where(x => !string.IsNullOrWhiteSpace(x)));

		public static string DisciplineName(Discipline? d)
			=> d is null || string.IsNullOrWhiteSpace(d.discipline_name) ? "Дисциплина" : d.discipline_name!.Trim();
	}
}