using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmorcIRL.TempMail;
using SmorcIRL.TempMail.Models;

namespace TDMail
{
	public class MailService
	{
		private readonly MailClient _client = new MailClient(new Uri("https://api.mail.gw/"));
		public event EventHandler<MessageSource>? OnMessageReceive;
		public string? Email { get; private set; }

		public async Task InitNewMail()
		{
			var domain = await _client.GetFirstAvailableDomainName();
			var login = GetRandomString(10);
			const string password = "password";
			Email = string.Concat(login, "@", domain);
			await _client.Register(Email, password);
		}

		public async Task StartListener(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					var allMessages = await _client.GetAllMessages();
					var message = allMessages.FirstOrDefault(m => m.Seen == false);
					if (message is not null)
					{
						var raw = await _client.GetMessageSource(message.Id);
						OnMessageReceive?.Invoke(this, raw);
						await _client.MarkMessageAsSeen(message.Id, true);
					}
					await Task.Delay(1000, cancellationToken);
				}
				catch (Exception e)
				{
					await Task.Delay(1000, cancellationToken);
				}
			}
		}

		private static string GetRandomString(int length)
		{
			var res = string.Empty;
			var rand = new Random();
			for (var i = 0; i < length; i++)
			{
				res += (char)rand.Next('a', 'z' + 1);
			}

			return res;
		}
	}
}