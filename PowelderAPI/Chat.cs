using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.UI.Chat;
using TerrariaApi.Server;
using TShockAPI;

namespace PowelderAPI
{
    public class Chat
    {

		public delegate void PowelderChatHookD(ServerChatEventArgs chat);

		public static event PowelderChatHookD OnPowelderChat;

		public static Dictionary<byte, Action<byte, object[]>> Acceptation = new Dictionary<byte, Action<byte, object[]>>();

		public static Dictionary<byte, object[]> AcceptData = new Dictionary<byte, object[]>();

		private static Dictionary<int, DateTime> _antiSpam = new Dictionary<int, DateTime>();

		public static void OnChat(ServerChatEventArgs args)
		{
			if (args.Handled)
				return;

			TSPlayer tSPlayer = TShock.Players[args.Who];
			if (tSPlayer == null)
			{
				args.Handled = true;
				return;
			}
			if (args.Text.Length > 500)
			{
				tSPlayer.SendErrorMessage("[c/595959:»]  Twoja wiadomosc jest zbyt dluga.");
				args.Handled = true;
				return;
			}
			string text = args.Text;
			foreach (var item in ChatManager.Commands._localizedCommands)
			{
				if (item.Value._name == args.CommandId._name)
				{
					if (!String.IsNullOrEmpty(text))
					{
						text = item.Key.Value + ' ' + text;
					}
					else
					{
						text = item.Key.Value;
					}
					break;
				}
			}
			if (text.ToLower().StartsWith("/tak") || text.ToLower().StartsWith("/nie"))
			{
				if (Acceptation.ContainsKey((byte)tSPlayer.Index))
				{
					bool flag = false;
					if (text.ToLower().StartsWith("/tak"))
					{
						flag = true;
					}
					if (flag)
					{
						Acceptation[(byte)tSPlayer.Index]((byte)tSPlayer.Index, AcceptData[(byte)tSPlayer.Index]);
						AcceptData.Remove((byte)tSPlayer.Index);
						Acceptation.Remove((byte)tSPlayer.Index);
					}
					else
					{
						tSPlayer.SendMessage("[c/595959:»]  Anulowales zadanie.", Color.Gray);
						AcceptData.Remove((byte)tSPlayer.Index);
						Acceptation.Remove((byte)tSPlayer.Index);
					}
				}
				args.Handled = true;
				return;
			}
			if ((text.StartsWith(TShock.Config.CommandSpecifier) || text.StartsWith(TShock.Config.CommandSilentSpecifier)) && !string.IsNullOrWhiteSpace(text.Substring(1)))
			{
				try
				{
					args.Handled = true;
					if (!Commands.HandleCommand(tSPlayer, text))
					{
						tSPlayer.SendErrorMessage("Unable to parse command. Please contact an administrator for assistance.");
						TShock.Log.ConsoleError("Unable to parse command '{0}' from player {1}.", text, tSPlayer.Name);
					}
				}
				catch (Exception ex)
				{
					TShock.Log.ConsoleError("An exception occurred executing a command.");
					TShock.Log.Error(ex.ToString());
				}
			}
			else
			{
				if (!tSPlayer.HasPermission("server.jmod"))
				{
					if (_antiSpam.ContainsKey(tSPlayer.Index))
					{
						if (!((DateTime.Now - _antiSpam[tSPlayer.Index]).TotalMilliseconds >= 800.0))
						{
							_antiSpam[tSPlayer.Index] = DateTime.Now;
							tSPlayer.SendErrorMessage("[c/595959:»]  Zwolnij troche. Za szybko piszesz.");
							args.Handled = true;
							return;
						}
						_antiSpam[tSPlayer.Index] = DateTime.Now;
					}
					else
					{
						_antiSpam.Add(tSPlayer.Index, DateTime.Now);
					}
				}
				if (!tSPlayer.HasPermission(Permissions.canchat))
				{
					args.Handled = true;
				}
				else if (tSPlayer.mute)
				{
					tSPlayer.SendErrorMessage("[c/595959:»]  Jestes wyciszony.");
					args.Handled = true;
				}
				else
				{
					OnPowelderChat.Invoke(args);
				}
			}
			args.Handled = true;
		}

		public static void OnLeave(LeaveEventArgs args)
		{
			int who = args.Who;

			if (Acceptation.ContainsKey((byte)who))
			{
				Acceptation.Remove((byte)who);
			}
			if (AcceptData.ContainsKey((byte)who))
			{
				AcceptData.Remove((byte)who);
			}
			if (_antiSpam.ContainsKey(who))
			{
				_antiSpam.Remove(who);
			}
		}

		public static void AddAcceptation(byte who, Action<byte, object[]> action, object[] data)
		{
			if (Acceptation.ContainsKey(who))
			{
				Acceptation[who] = action;
			}
			else
			{
				Acceptation.Add(who, action);
			}
			if (AcceptData.ContainsKey(who))
			{
				AcceptData[who] = data;
			}
			else
			{
				AcceptData.Add(who, data);
			}
		}
	}
}
