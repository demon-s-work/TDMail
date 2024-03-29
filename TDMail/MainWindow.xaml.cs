﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.Notifications;
using mrousavy;
using SmorcIRL.TempMail.Models;
using TDMail.WinApiImpl;

namespace TDMail
{
	public partial class MainWindow
	{
		private HotKey? _hotKey;
		private bool _captureHotKey;
		private readonly MailService _mailService = new MailService();
		private readonly CancellationToken _cancellationToken = new CancellationToken();

		public MainWindow()
		{
			InitializeComponent();
			Closed += (_, _) => _hotKey?.Dispose();
		}
		
		protected override async void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			await _mailService.InitNewMail();
			_mailService.OnMessageReceive += MailServiceOnOnMessageReceive;
			_ = _mailService.StartListener(_cancellationToken);
		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
		}

		private void MailServiceOnOnMessageReceive(object? sender, MessageSource e)
		{
			Dispatcher.Invoke(() => {
				Notify("New email", MailParser.ParseLinks(e.Data));
			});
		}

		private void SetHotKey(Action<HotKey> action)
		{
			var mod = ModifierKeys.None;
			_modifier.ForEach(m => {
				mod |= m;
			});
			var key = Key.None;
			_keys.ForEach(k => {
				key |= k;
			});

			try
			{
				_hotKey = new HotKey(mod, key, this, action);
			}
			catch (Exception)
			{
				MessageBox.Show("Hotkey already in use, try another");
			}
		}

		private void Notify(string text, List<string> links)
		{
			new ToastContentBuilder()
				.AddText(text)
				.AddButton(new ToastButton()
				           .SetContent($"Copy {links}...")
				           .SetBackgroundActivation())
				.Show(toast => {
					toast.ExpirationTime = DateTimeOffset.Now.AddSeconds(20);
					toast.Activated += (_, _) => {
						Dispatcher.Invoke(() => {
							if (links != null)
								foreach (var link in links)
								{
									Clipboard.SetText(link);
								}
						});
					};
				});
		}

		private readonly List<ModifierKeys> _modifier = new List<ModifierKeys>();
		private readonly List<Key> _keys = new List<Key>();
		private readonly List<Key> _pressed = new List<Key>();

		private void HotKeyPickerButton_OnClick(object sender, RoutedEventArgs e)
		{
			_hotKey?.Dispose();
			_captureHotKey = true;
			_modifier.Clear();
			_keys.Clear();
			_pressed.Clear();
			HotKeyLabel.Content = "";
		}

		private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (!_captureHotKey || _pressed.Any(k => k == e.Key))
				return;
			
			_pressed.Add(e.Key);
			HotKeyLabel.Content = string.Join('+', _pressed);
			switch (e.Key)
			{
				case Key.LeftCtrl or Key.RightCtrl:
					break;
				case Key.LeftAlt:
				case Key.RightAlt:
					_modifier.Add(ModifierKeys.Alt);
					break;
				case Key.LeftShift:
				case Key.RightShift:
					_modifier.Add(ModifierKeys.Shift);
					break;
				default:
					_keys.Add(e.Key);
					break;
			}
		}

		private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
		{
			if (!_captureHotKey)
				return;
			
			_captureHotKey = false;
			SetHotKey(_ => {
				if (_mailService.Email == null)
					return;
				Clipboard.SetText(_mailService.Email);
				WinApi.SendCtrlV();
			});
		}
	}
}