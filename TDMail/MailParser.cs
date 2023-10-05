using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MimeKit;

namespace TDMail
{
	public abstract partial class MailParser
	{
		public static IEnumerable<string?> ParseLinks(string body)
		{
			var msg = MimeMessage.Load(new MemoryStream(Encoding.ASCII.GetBytes(body)));
			var regx = LinkRegex();
			var collection = regx.Matches(msg.HtmlBody);
			return collection.Select(m => m.Value);
		}

        [GeneratedRegex(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)")]
        private static partial Regex LinkRegex();
    }
}