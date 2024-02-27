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
		public static List<string> ParseLinks(string body)
		{
			var msg = MimeMessage.Load(new MemoryStream(Encoding.ASCII.GetBytes(body)));
			var linkRegex = LinkRegex();
			var codeRegex = CodeRegex();
			var links = linkRegex.Matches(msg.HtmlBody).Select(m => m.Value);
			var codes = codeRegex.Matches(msg.HtmlBody).Select(m => m.Value);
			return links.Concat(codes).ToList();
		}

        [GeneratedRegex(@"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)")]
        private static partial Regex LinkRegex();

        [GeneratedRegex(@"[^a-zA-Z]+")]
        private static partial Regex CodeRegex();
	}

}