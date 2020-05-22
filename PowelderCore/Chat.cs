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

		public static Dictionary<byte, Action<byte, object[]>> acceptation = new Dictionary<byte, Action<byte, object[]>>();

		public static Dictionary<byte, object[]> acceptData = new Dictionary<byte, object[]>();

		private static Dictionary<int, DateTime> antiSpam = new Dictionary<int, DateTime>();

		public static void onChat(ServerChatEventArgs args)
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
				if (acceptation.ContainsKey((byte)tSPlayer.Index))
				{
					bool flag = false;
					if (text.ToLower().StartsWith("/tak"))
					{
						flag = true;
					}
					if (flag)
					{
						acceptation[(byte)tSPlayer.Index]((byte)tSPlayer.Index, acceptData[(byte)tSPlayer.Index]);
						acceptData.Remove((byte)tSPlayer.Index);
						acceptation.Remove((byte)tSPlayer.Index);
					}
					else
					{
						tSPlayer.SendMessage("[c/595959:»]  Anulowales zadanie.", Color.Gray);
						acceptData.Remove((byte)tSPlayer.Index);
						acceptation.Remove((byte)tSPlayer.Index);
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
					if (antiSpam.ContainsKey(tSPlayer.Index))
					{
						if (!((DateTime.Now - antiSpam[tSPlayer.Index]).TotalMilliseconds >= 800.0))
						{
							antiSpam[tSPlayer.Index] = DateTime.Now;
							tSPlayer.SendErrorMessage("[c/595959:»]  Zwolnij troche. Za szybko piszesz.");
							args.Handled = true;
							return;
						}
						antiSpam[tSPlayer.Index] = DateTime.Now;
					}
					else
					{
						antiSpam.Add(tSPlayer.Index, DateTime.Now);
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

			if (acceptation.ContainsKey((byte)who))
			{
				acceptation.Remove((byte)who);
			}
			if (acceptData.ContainsKey((byte)who))
			{
				acceptData.Remove((byte)who);
			}
			if (antiSpam.ContainsKey(who))
			{
				antiSpam.Remove(who);
			}
		}

		public static void addAcceptation(byte who, Action<byte, object[]> action, object[] data)
		{
			if (acceptation.ContainsKey(who))
			{
				acceptation[who] = action;
			}
			else
			{
				acceptation.Add(who, action);
			}
			if (acceptData.ContainsKey(who))
			{
				acceptData[who] = data;
			}
			else
			{
				acceptData.Add(who, data);
			}
		}
	}
}
