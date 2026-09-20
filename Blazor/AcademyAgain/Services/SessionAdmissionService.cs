using AcademyAgain.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademyAgain.Services
{
	public sealed record StudentAdmission(
		int StudentId,
		string StudentName,
		int TotalLessons,
		int Present,
		int Missed,
		double AttendancePct,
		double? AverageGrade,
		int GradeCount,
		bool Granted,
		IReadOnlyList<string> Fails);

	/// <summary>
	/// Допуск к экзаменационной сессии рассчитывается по факту успеваемости студента:
	/// посещаемость не ниже порога, средний балл не ниже порога, пропусков не больше допустимых.
	/// </summary>
	public class SessionAdmissionService
	{
		private readonly IDbContextFactory<AcademyAgainContext> _dbFactory;

		public SessionAdmissionService(IDbContextFactory<AcademyAgainContext> dbFactory)
		{
			_dbFactory = dbFactory;
		}

		public async Task<SessionRule?> GetActiveRuleAsync()
		{
			await using var context = await _dbFactory.CreateDbContextAsync();
			return await context.SessionRules.AsNoTracking().FirstOrDefaultAsync(r => r.is_active);
		}

		/// <summary>Допуск по всем студентам группы (ведомость).</summary>
		public async Task<List<StudentAdmission>> ComputeGroupAsync(int groupId)
		{
			await using var context = await _dbFactory.CreateDbContextAsync();

			var rule = await context.SessionRules.AsNoTracking().FirstOrDefaultAsync(r => r.is_active);
			var studentIds = await context.Students.AsNoTracking()
				.Where(s => s.group == groupId)
				.Select(s => s.stud_id)
				.ToListAsync();
			if (studentIds.Count == 0)
			{
				return new List<StudentAdmission>();
			}

			var result = new List<StudentAdmission>();
			foreach (var studentId in studentIds)
			{
				result.Add(await ComputeStudentCoreAsync(context, studentId, rule));
			}
			return result;
		}

		/// <summary>Допуск одного студента (личная карточка).</summary>
		public Task<StudentAdmission?> ComputeStudentAsync(int studentId)
		{
			return ComputeStudentCoreAsync(studentId);
		}

		private async Task<StudentAdmission> ComputeStudentCoreAsync(AcademyAgainContext context, int studentId, SessionRule? rule)
		{
			var student = await context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.stud_id == studentId);
			return await BuildAdmissionAsync(context, student, rule);
		}

		private async Task<StudentAdmission?> ComputeStudentCoreAsync(int studentId)
		{
			await using var context = await _dbFactory.CreateDbContextAsync();
			var student = await context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.stud_id == studentId);
			if (student is null)
			{
				return null;
			}
			var rule = await context.SessionRules.AsNoTracking().FirstOrDefaultAsync(r => r.is_active);
			return await BuildAdmissionAsync(context, student, rule);
		}

		private async Task<StudentAdmission> BuildAdmissionAsync(AcademyAgainContext context, Student? student, SessionRule? rule)
		{
			if (student is null)
			{
				return new StudentAdmission(0, "—", 0, 0, 0, 0, null, 0, false, new[] { "Студент не найден" });
			}

			var groupId = student.group;
			var spent = await context.Schedule.AsNoTracking()
				.Where(s => s.group == groupId && s.spent)
				.Select(s => s.lesson_id)
				.ToListAsync();
			var spentIds = spent.ToHashSet();

			var attendance = await context.Attendance.AsNoTracking()
				.Where(a => a.student == student.stud_id)
				.ToListAsync();
			var inSpent = attendance.Where(a => spentIds.Contains(a.lesson)).ToList();
			int present = inSpent.Count(a => a.present);
			int total = spent.Count;

			var gradeValues = (await context.Grades.AsNoTracking()
					.Where(g => g.student == student.stud_id)
					.Select(g => new[] { (int?)g.grade_1, (int?)g.grade_2 })
					.ToListAsync())
				.SelectMany(v => v)
				.Where(v => v.HasValue && v.Value >= 2 && v.Value <= 5)
				.Select(v => v!.Value)
				.ToList();

			double attendancePct = total == 0 ? 100 : present * 100.0 / total;
			double? avg = gradeValues.Count == 0 ? null : gradeValues.Average();
			int missed = Math.Max(0, total - present);

			var fails = new List<string>();
			if (rule is null)
			{
				fails.Add("Правила сессии не заданы");
			}
			else
			{
				if (total > 0 && attendancePct < rule.min_attendance_pct)
				{
					fails.Add($"Посещаемость ниже {rule.min_attendance_pct}%");
				}
				if (avg is null)
				{
					fails.Add("Нет оценок для среднего балла");
				}
				else if (avg.Value < (double)rule.min_average_grade)
				{
					fails.Add($"Средний балл ниже {rule.min_average_grade:0.00}");
				}
				if (missed > rule.required_makeups)
				{
					fails.Add($"Пропусков больше допустимых ({rule.required_makeups})");
				}
			}

			bool granted = fails.Count == 0;
			return new StudentAdmission(
				student.stud_id,
				string.Join(" ", new[] { student.last_name, student.first_name, student.middle_name }.Where(p => !string.IsNullOrWhiteSpace(p))),
				total,
				present,
				missed,
				attendancePct,
				avg,
				gradeValues.Count,
				granted,
				fails);
		}
	}
}