using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace TDMail
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
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
			if (MainWindow is not null)
			{
				MainWindow.Show();
				MainWindow.WindowState = WindowState.Normal;
			}
		}

		private ContextMenuStrip CreateContextMenu()
		{
			var menuStrip = new ContextMenuStrip();
			var openItem = new ToolStripMenuItem("Open");
			var closeItem = new ToolStripMenuItem("Exit");

			openItem.Click += OpenItemOnClick;
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