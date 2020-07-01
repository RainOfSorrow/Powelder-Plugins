using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.UI.Chat;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using PowelderAPI;

namespace SurvivalCore
{
	internal class ExtendedChat
	{
		public static string ChatFormat = "[i:{0}] [c/595959:;] [c/{1}:{2}] [c/595959:;] [c/{3}:{4}][c/595959::] {5}";

		protected static readonly Dictionary<string, string> Colors = new Dictionary<string, string>
		{
			{
				"bialy",
				"ffffff"
			},
			{
				"szary",
				"696969"
			},
			{
				"czerwony",
				"ff3333"
			},
			{
				"zielony",
				"22ff22"
			},
			{
				"niebieski",
				"0066ff"
			},
			{
				"zolty",
				"ffff22"
			},
			{
				"cyjan",
				"22ffff"
			},
			{
				"magenta",
				"ff33ff"
			},
			{
				"pomaranczowy",
				"fd7c24"
			},
			{
				"fiolet",
				"a937ff"
			},
			{
				"brazowy",
				"86592d"
			},
			{
				"rozowy",
				"ff87ef"
			}
		};

		public static void OnChat(ServerChatEventArgs args)
		{
			TSPlayer plr = TShock.Players[args.Who];
			string text;

			if (SurvivalCore.IsChatEvent && SurvivalCore.ChatEventWord == args.Text)
			{
				SurvivalCore.ChatEventStoper.Stop();
				TSPlayer.All.SendMessage($"[i:889] [c/595959:;] [c/00cc66:Event] [c/595959:;] [c/00cc66:{TShock.Players[args.Who].Name}] napisal najszybciej [c/00cc66:{SurvivalCore.ChatEventWord}] i wygral 60 €. [c/595959:(]{Math.Round(SurvivalCore.ChatEventStoper.Elapsed.TotalSeconds, 3)} sec[c/595959:)]", new Color(128, 255, 191));
				SurvivalCore.SrvPlayers[args.Who].Money += 60;
				SurvivalCore.IsChatEvent = false;
				SurvivalCore.ChatEventTimer = DateTime.UtcNow;
				SurvivalCore.ChatEventStoper.Reset();
			}
			try
			{
				try
				{
					text = string.Format(ChatFormat,
						(SurvivalCore.SrvPlayers[args.Who].PrefixItem == -1) ? PowelderAPI.Utils.GetGroupItem(plr.Group.Name) : SurvivalCore.SrvPlayers[args.Who].PrefixItem, PowelderAPI.Utils.GetGroupColor(plr.Group.Name), (plr.Group.Name == "Xedlefix" || plr.Group.Name == "Iwobos") ? "Zalozyciel" : plr.Group.Name, SurvivalCore.SrvPlayers[args.Who].NickColor ?? PowelderAPI.Utils.GetGroupColor(plr.Group.Name), plr.Name, args.Text);
				}
				catch (NullReferenceException)
				{
					text = string.Format(ChatFormat,
						PowelderAPI.Utils.GetGroupItem(plr.Group.Name), PowelderAPI.Utils.GetGroupColor(plr.Group.Name), plr.Group.Name, PowelderAPI.Utils.GetGroupColor(plr.Group.Name), plr.Name, args.Text);
				}
				PlayerHooks.OnPlayerChat(plr, args.Text, ref text);
				TSPlayer.All.SendMessage(text, plr.Group.R, plr.Group.G, plr.Group.B);
				TSPlayer.Server.SendMessage(string.Format(((plr.Group.Name == "Xedlefix" || plr.Group.Name == "Iwobos") ? "Zalozyciel" : plr.Group.Name) + " " + plr.Name + ": " + args.Text), byte.MaxValue, byte.MaxValue, byte.MaxValue);
				TShock.Log.Info(((plr.Group.Name == "Xedlefix" || plr.Group.Name == "Iwobos") ? "Zalozyciel" : plr.Group.Name) + " " + plr.Name + ": " + args.Text);
				args.Handled = true;
			}
			catch (FormatException)
			{
				args.Handled = true;
			}

		}

		public static void CommandPrefixItem(TShockAPI.CommandArgs args)
		{
			int money = SurvivalCore.SrvPlayers[args.Player.Index].Money;
			if (args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type == 0)
			{
				args.Player.SendErrorMessage("Nie znaleziono itemu.");
				return;
			}
			int cost = 3500;
			cost = Utils.CostCalc(args.Player, cost);
			if (money < cost)
			{
				args.Player.SendErrorMessage("Nie stac cie na zmiane itemu w prefixie. Koszt {0} €", cost);
				return;
			}
			object[] data = new object[2]
			{
				cost,
				args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type
			};
			Chat.AddAcceptation((byte)args.Player.Index, Action_CommandItem, data);
			string msg = string.Format(
				ChatFormat,
				args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type, PowelderAPI.Utils.GetGroupColor(args.Player.Group.Name),
				(args.Player.Group.Name == "Xedlefix" || args.Player.Group.Name == "Iwobos") ? "Zalozyciel" : args.Player.Group.Name,
SurvivalCore.SrvPlayers[args.Player.Index].NickColor ?? PowelderAPI.Utils.GetGroupColor(args.Player.Group.Name),
				args.Player.Name,
				"Przykladowa wiadomosc.");
			args.Player.SendMessage(msg, Color.White);
			args.Player.SendInfoMessage($"Czy zgadzasz sie na zmiane? Wpisz [c/66ff66:/tak] lub [c/ff6666:/nie]. Koszt {cost} €");
		}

		public static void Action_CommandItem(byte who, object[] data)
		{
			Array array = data;
			int num = (int)array.GetValue(0);
			int num2 = (int)array.GetValue(1);
			if (SurvivalCore.SrvPlayers[who].Money < num)
			{
				TShock.Players[who].SendErrorMessage("Nie stac cie na zmiane itemu w prefixie. Koszt {0} €", num);
				return;
			}
			SurvivalCore.SrvPlayers[who].Money -= num;
			SurvivalCore.SrvPlayers[who].PrefixItem = num2;
			TShock.Players[who].SendMessage($"Oto twoj nowy item w prefixie: [i:{num2}]", Color.Gray);
		}

		public static void CommandNickColor(TShockAPI.CommandArgs args)
		{
			int money = SurvivalCore.SrvPlayers[args.Player.Index].Money;
			string text = "0:0=1";
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			if (text == "list")
			{
				List<string> list = new List<string>();
				foreach (string key in Colors.Keys)
				{
					list.Add($"[c/{Colors[key]}:{key}]");
				}
				args.Player.SendMessage($"[c/00ff00:Lista dostepnych kolorow:]", new Color(0, 0, 0));
				PaginationTools.SendPage(args.Player, 0, PaginationTools.BuildLinesFromTerms(list, null, ", ", 140), new PaginationTools.Settings
				{
					IncludeHeader = false,
					LineTextColor = new Color(255, 255, 255),
					IncludeFooter = false,
					NothingToDisplayString = "Error 404."
				});
				return;
			}
			if (!Colors.ContainsKey(text))
			{
				args.Player.SendErrorMessage("Nie znaleziono podanego koloru.");
				args.Player.SendInfoMessage($"Wpisz /ncolor list, aby otrzymac liste dostepnych kolorow.");
				return;
			}
			int cost = 6500;
			cost = Utils.CostCalc(args.Player, cost);
			if (money < cost)
			{
				args.Player.SendErrorMessage("Nie stac cie na zmiane koloru nicku. Koszt {0} €", cost);
				return;
			}
			object[] data = new object[2]
			{
				cost,
				text
			};
			Chat.AddAcceptation((byte)args.Player.Index, Action_CommandNick, data);
			string msg = string.Format(ChatFormat, (
				SurvivalCore.SrvPlayers[args.Player.Index].PrefixItem == -1) ? PowelderAPI.Utils.GetGroupItem(args.Player.Group.Name) : SurvivalCore.SrvPlayers[args.Player.Index].PrefixItem,
				PowelderAPI.Utils.GetGroupColor(args.Player.Group.Name), (args.Player.Group.Name == "Xedlefix" || args.Player.Group.Name == "Iwobos") ? "Zalozyciel" : args.Player.Group.Name,
				Colors[text],
				args.Player.Name,
				"Przykladowa wiadomosc.");
			args.Player.SendMessage(msg, Color.White);
			args.Player.SendInfoMessage($"Czy zgadzasz sie na zmiane? Wpisz [c/66ff66:/tak] lub [c/ff6666:/nie] Koszt {cost} €");
		}

		public static void Action_CommandNick(byte who, object[] data)
		{
			Array array = data;
			int num = (int)array.GetValue(0);
			string key = (string)array.GetValue(1);
			if (SurvivalCore.SrvPlayers[who].Money < num)
			{
				TShock.Players[who].SendErrorMessage("Nie stac cie na zmiane koloru nicku. Koszt {0} €", num);
				return;
			}
			SurvivalCore.SrvPlayers[who].Money -= num;
			SurvivalCore.SrvPlayers[who].NickColor = Colors[key];
			TShock.Players[who].SendSuccessMessage($"[c/00ff00:Oto twoja nowa barwa nicku:] [c/{Colors[key]}:{TShock.Players[who].Name}]");
		}


		public static void ChatEventThread()
		{
			SurvivalCore.ChatEventWord = RandomWord();
			SurvivalCore.ChatEventStoper.Start();
			SurvivalCore.IsChatEvent = true;
			TSPlayer.All.SendMessage("[i:889] [c/595959:;] [c/00cc66:Event] [c/595959:;] Kto napisze najszybciej [c/00cc66:" + SurvivalCore.ChatEventWord + "] wygra 80 €.", new Color(128, 255, 191));
			Thread.Sleep((SurvivalCore.ChatEventWord.Length + 8) * 1000);
			if (SurvivalCore.IsChatEvent)
			{
				SurvivalCore.ChatEventStoper.Reset();
				TSPlayer.All.SendMessage("[i:889] [c/595959:;] [c/00cc66:Event] [c/595959:;] Nikt nie napisal [c/00cc66:" + SurvivalCore.ChatEventWord + "] wystarczajaco szybko.", new Color(128, 255, 191));
				SurvivalCore.IsChatEvent = false;
				SurvivalCore.ChatEventTimer = DateTime.UtcNow;
			}
		}

		private static string RandomWord()
		{
			Random random = new Random(Guid.NewGuid().GetHashCode());
			string text = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			byte[] array = new byte[3]
			{
				8,
				12,
				16
			};
			byte b = array[random.Next(0, 2)];
			string text2 = "";
			for (int i = 1; i <= b; i++)
			{
				text2 += text[random.Next(0, 61)].ToString();
			}
			return text2;
		}

		public static void Debug(TShockAPI.CommandArgs args)
		{
			Thread thread = new Thread(DebugThread)
			{
				IsBackground = true
			};
			thread.Start(args.Player);
		}

		public static void DebugThread(object x)
		{
			TSPlayer plr = (TSPlayer)x;
			plr.SendMessage("[DEBUG]", Color.PaleVioletRed);
			for (int i = -100; i <= 100; i++)
			{
				Thread.Sleep(50);
				plr.SendData(PacketTypes.PlayHarp, "", plr.Index, (float)i / 100f);
				plr.SendMessage($"[Debug] PlayHarp: {(float)i / 100f}", Color.DeepSkyBlue);
			}
		}
	}
}
