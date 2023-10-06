using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using static System.Windows.Forms.NotifyIcon;
using Application=System.Windows.Application;
using MessageBox=System.Windows.MessageBox;

namespace TDMail
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			var ico = new Icon("AppIcon.ico");
			var icon = new NotifyIcon
			{
				Icon = ico,
				Visible = true,
				ContextMenuStrip = CreateContextMenu()
			};
			icon.DoubleClick += OpenItemOnClick;
			Current.Exit += (_, _) => { icon.Dispose(); };
		}

		private void OpenItemOnClick(object? sender, EventArgs e)
		{
			
		}

		private ContextMenuStrip CreateContextMenu()
		{
			var menuStrip = new ContextMenuStrip();
			var openItem = new ToolStripMenuItem("Open");
			var closeItem = new ToolStripMenuItem("Exit");
			
			openItem.Click += (_, _) => {};
			closeItem.Click += (_, _) => {Shutdown(1);};

			menuStrip.Items.AddRange(new ToolStripItem[]
			{
				openItem,
				closeItem
			});
			return menuStrip;
		}
	}
}