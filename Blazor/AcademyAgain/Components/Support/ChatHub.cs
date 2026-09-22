using AcademyAgain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace AcademyAgain.Components.Support
{
	public record SupportMessageDto(
		[property: JsonPropertyName("msgId")] int msg_id,
		[property: JsonPropertyName("chatId")] int chat_id,
		[property: JsonPropertyName("authorUserId")] int author_user_id,
		[property: JsonPropertyName("body")] string body,
		[property: JsonPropertyName("createdAt")] string? created_at);

	[Authorize]
	public class ChatHub : Hub
	{
		private static readonly string SupportGroup = "support";
		private static string ChatGroup(int chatId) => $"chat-{chatId}";
		private static string UserGroup(int userId) => $"user-{userId}";

		private readonly IDbContextFactory<AcademyAgainContext> _dbFactory;

		public ChatHub(IDbContextFactory<AcademyAgainContext> dbFactory)
		{
			_dbFactory = dbFactory;
		}

		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			var userId = await CurrentUserIdAsync();
			if (userId is not null)
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId.Value));
			}
			if (await IsSupportRoleAsync())
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, SupportGroup);
				await Clients.Caller.SendAsync("ChatListUpdated", 0);
			}
		}

		public async Task JoinChat(int chatId)
		{
			if (await CanAccessChatAsync(chatId))
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroup(chatId));
			}
		}

		public async Task LeaveChat(int chatId)
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroup(chatId));
		}

		public async Task<SupportMessageDto?> SendMessage(int chatId, string body)
		{
			if (string.IsNullOrWhiteSpace(body))
			{
				return null;
			}

			await using var db = await _dbFactory.CreateDbContextAsync();
			var chat = await db.SupportChats.FirstOrDefaultAsync(c => c.chat_id == chatId);
			if (chat is null)
			{
				return null;
			}

			var user = await CurrentUserAsync(db);
			if (user is null)
			{
				return null;
			}

			if (chat.user_id == user.user_id)
			{
				if (await db.SupportBans.AnyAsync(b => b.chat_id == chatId))
				{
					return null;
				}
			}
			else if (!await IsSupportUserAsync(db, user))
			{
				return null;
			}

			var message = new SupportMessage
			{
				chat_id = chatId,
				author_user_id = user.user_id,
				body = body.Trim(),
				created_at = DateTime.Now
			};
			db.SupportMessages.Add(message);
			chat.updated_at = DateTime.Now;
			if (chat.status == 0)
			{
				chat.status = 1;
			}
			await db.SaveChangesAsync();

			var payload = new SupportMessageDto(
				message.msg_id,
				message.chat_id,
				message.author_user_id,
				message.body,
				message.created_at?.ToString("O"));

			await Clients.Group(ChatGroup(chatId)).SendAsync("ReceiveMessage", payload);
			await Clients.Group(UserGroup(chat.user_id)).SendAsync("ConversationUpdated", chatId);
			await Clients.Group(UserGroup(chat.user_id)).SendAsync("ChatListUpdated", chatId);
			await Clients.Group(SupportGroup).SendAsync("ChatListUpdated", chatId);

			return payload;
		}

		private async Task<bool> CanAccessChatAsync(int chatId)
		{
			await using var db = await _dbFactory.CreateDbContextAsync();
			var user = await CurrentUserAsync(db);
			if (user is null)
			{
				return false;
			}
			if (await db.SupportChats.AnyAsync(c => c.chat_id == chatId && c.user_id == user.user_id))
			{
				return true;
			}
			return await IsSupportUserAsync(db, user);
		}

		private async Task<int?> CurrentUserIdAsync()
		{
			var name = Context?.User?.Identity?.Name;
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			await using var db = await _dbFactory.CreateDbContextAsync();
			return await db.Users.AsNoTracking().Where(u => u.username == name).Select(u => (int?)u.user_id).FirstOrDefaultAsync();
		}

		private async Task<User?> CurrentUserAsync(AcademyAgainContext db)
		{
			var name = Context?.User?.Identity?.Name;
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			return await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.username == name);
		}

		private async Task<bool> IsSupportRoleAsync()
		{
			return await Task.FromResult(Context?.User?.FindFirstValue(ClaimTypes.Role) is "admin" or "moderator" or "support");
		}

		private static async Task<bool> IsSupportUserAsync(AcademyAgainContext db, User user)
		{
			var role = await db.Roles.AsNoTracking().Where(r => r.role_id == user.role_id).Select(r => r.role_name).FirstOrDefaultAsync();
			return role is "admin" or "moderator" or "support";
		}
	}
}