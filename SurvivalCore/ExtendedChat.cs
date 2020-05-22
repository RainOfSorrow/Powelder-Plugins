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
		public static string ChatFormat = "[i:{0}] [c/595959:⮘] [c/{1}:{2}] [c/595959:⮚] [c/{3}:{4}][c/595959::] {5}";

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

		public static void onChat(ServerChatEventArgs args)
		{
			TSPlayer plr = TShock.Players[args.Who];
			string text;

			if (SurvivalCore.isChatEvent && SurvivalCore.ChatEvent_Word == args.Text)
			{
				SurvivalCore.chatEventStoper.Stop();
				TSPlayer.All.SendMessage($"[i:889] [c/595959:⮘] [c/00cc66:Event] [c/595959:⮚] [c/00cc66:{TShock.Players[args.Who].Name}] napisal najszybciej [c/00cc66:{SurvivalCore.ChatEvent_Word}] i wygral 60 €. [c/595959:(]{SurvivalCore.chatEventStoper.Elapsed.TotalSeconds} sec[c/595959:)]", new Color(128, 255, 191));
				SurvivalCore.srvPlayers[args.Who].Money += 60;
				SurvivalCore.isChatEvent = false;
				SurvivalCore.chatEventTimer = DateTime.UtcNow;
				SurvivalCore.chatEventStoper.Reset();
			}
			try
			{
				try
				{
					text = string.Format(ChatFormat,
						(SurvivalCore.srvPlayers[args.Who].prefixItem == -1) ? PowelderAPI.Utils.getGroupItem(plr.Group.Name) : SurvivalCore.srvPlayers[args.Who].prefixItem, PowelderAPI.Utils.getGroupColor(plr.Group.Name), (plr.Group.Name == "Xedlefix" || plr.Group.Name == "Iwobos") ? "Administrator" : plr.Group.Name, (SurvivalCore.srvPlayers[args.Who].nickColor == null) ? PowelderAPI.Utils.getGroupColor(plr.Group.Name) : SurvivalCore.srvPlayers[args.Who].nickColor, plr.Name, args.Text);
				}
				catch (NullReferenceException)
				{
					text = string.Format(ChatFormat,
						PowelderAPI.Utils.getGroupItem(plr.Group.Name), PowelderAPI.Utils.getGroupColor(plr.Group.Name), plr.Group.Name, PowelderAPI.Utils.getGroupColor(plr.Group.Name), plr.Name, args.Text);
				}
				PlayerHooks.OnPlayerChat(plr, args.Text, ref text);
				TSPlayer.All.SendMessage(text, plr.Group.R, plr.Group.G, plr.Group.B);
				TSPlayer.Server.SendMessage(string.Format(((plr.Group.Name == "Xedlefix" || plr.Group.Name == "Iwobos") ? "Administrator" : plr.Group.Name) + " " + plr.Name + ": " + args.Text), byte.MaxValue, byte.MaxValue, byte.MaxValue);
				TShock.Log.Info(((plr.Group.Name == "Xedlefix" || plr.Group.Name == "Iwobos") ? "Administrator" : plr.Group.Name) + " " + plr.Name + ": " + args.Text);
				args.Handled = true;
			}
			catch (FormatException)
			{
				args.Handled = true;
			}

		}

		public static void CommandItem(TShockAPI.CommandArgs args)
		{
			int money = SurvivalCore.srvPlayers[args.Player.Index].Money;
			if (args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type == 0)
			{
				args.Player.SendErrorMessage("[c/595959:»]  Nie znaleziono itemu.");
				return;
			}
			int cost = 7500;
			cost = Utils.costCalc(args.Player, cost);
			if (money < cost)
			{
				args.Player.SendErrorMessage("[c/595959:»]  Nie stac cie na zmiane itemu w prefixie. [c/595959:(]Koszt {0} €[c/595959:)]", cost);
				return;
			}
			object[] data = new object[2]
			{
				cost,
				args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type
			};
			Chat.addAcceptation((byte)args.Player.Index, action_CommandItem, data);
			string msg = string.Format(ChatFormat, args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type, PowelderAPI.Utils.getGroupColor(args.Player.Group.Name), (args.Player.Group.Name == "Xedlefix" || args.Player.Group.Name == "Iwobos") ? "Administrator" : args.Player.Group.Name, (SurvivalCore.srvPlayers[args.Player.Index].nickColor == null) ? PowelderAPI.Utils.getGroupColor(args.Player.Group.Name) : SurvivalCore.srvPlayers[args.Player.Index].nickColor, args.Player.Name, "Przykladowa wiadomosc.");
			args.Player.SendMessage(msg, Color.White);
			args.Player.SendMessage($"[c/595959:»]  Czy zgadzasz sie na zmiane? Wpisz [c/66ff66:/tak] lub [c/ff6666:/nie] [c/595959:(][c/ffff66:Koszt {cost} €][c/595959:)]", Color.Gray);
		}

		public static void action_CommandItem(byte who, object[] data)
		{
			Array array = new object[2];
			array = data;
			int num = (int)array.GetValue(0);
			int num2 = (int)array.GetValue(1);
			if (SurvivalCore.srvPlayers[who].Money < num)
			{
				TShock.Players[who].SendErrorMessage("[c/595959:»]  Nie stac cie na zmiane itemu w prefixie. [c/595959:(]Koszt {0} €[c/595959:)]", num);
				return;
			}
			SurvivalCore.srvPlayers[who].Money -= num;
			SurvivalCore.srvPlayers[who].prefixItem = num2;
			TShock.Players[who].SendMessage($"[c/595959:»]  [c/66ff66:Oto twoj nowy item w prefixie][c/595959::] [i:{num2}]", Color.Gray);
		}

		public static void CommandNick(TShockAPI.CommandArgs args)
		{
			int money = SurvivalCore.srvPlayers[args.Player.Index].Money;
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
				args.Player.SendMessage($"[c/66ff66:Lista dostepnych kolorow][c/595959::]", new Color(0, 0, 0));
				PaginationTools.SendPage(args.Player, 0, PaginationTools.BuildLinesFromTerms(list, null, ", ", 140), new PaginationTools.Settings
				{
					IncludeHeader = false,
					LineTextColor = new Color(192, 192, 192),
					IncludeFooter = false,
					NothingToDisplayString = "Error 404."
				});
				return;
			}
			if (!Colors.ContainsKey(text))
			{
				args.Player.SendErrorMessage("[c/595959:»]  Nie znaleziono podanego koloru.");
				args.Player.SendMessage($"[c/595959:»]  Wpisz [c/66ff66:/ncolor list], aby otrzymac liste dostepnych kolorow.", Color.Gray);
				return;
			}
			int cost = 12500;
			cost = Utils.costCalc(args.Player, cost);
			if (money < cost)
			{
				args.Player.SendErrorMessage("[c/595959:»]  Nie stac cie na zmiane koloru nicku. [c/595959:(]Koszt {0} €[c/595959:)]", cost);
				return;
			}
			object[] data = new object[2]
			{
				cost,
				text
			};
			Chat.addAcceptation((byte)args.Player.Index, action_CommandNick, data);
			string msg = string.Format(ChatFormat, (SurvivalCore.srvPlayers[args.Player.Index].prefixItem == -1) ? PowelderAPI.Utils.getGroupItem(args.Player.Group.Name) : SurvivalCore.srvPlayers[args.Player.Index].prefixItem, PowelderAPI.Utils.getGroupColor(args.Player.Group.Name), (args.Player.Group.Name == "Xedlefix" || args.Player.Group.Name == "Iwobos") ? "Administrator" : args.Player.Group.Name, Colors[text], args.Player.Name, "Przykladowa wiadomosc.");
			args.Player.SendMessage(msg, Color.White);
			args.Player.SendMessage($"[c/595959:»]  Czy zgadzasz sie na zmiane? Wpisz [c/66ff66:/tak] lub [c/ff6666:/nie] [c/595959:(][c/ffff66:Koszt {cost} €][c/595959:)]", Color.Gray);
		}

		public static void action_CommandNick(byte who, object[] data)
		{
			Array array = new object[2];
			array = data;
			int num = (int)array.GetValue(0);
			string key = (string)array.GetValue(1);
			if (SurvivalCore.srvPlayers[who].Money < num)
			{
				TShock.Players[who].SendErrorMessage("[c/595959:»]  Nie stac cie na zmiane koloru nicku. [c/595959:(]Koszt {0} €[c/595959:)]", num);
				return;
			}
			SurvivalCore.srvPlayers[who].Money -= num;
			SurvivalCore.srvPlayers[who].nickColor = Colors[key];
			TShock.Players[who].SendMessage($"[c/595959:»]  [c/66ff66:Oto twoja nowa barwa nicku][c/595959::] [c/{Colors[key]}:{TShock.Players[who].Name}]", Color.Gray);
		}


		public static void chatEventThread()
		{
			SurvivalCore.ChatEvent_Word = RandomWord();
			SurvivalCore.chatEventStoper.Start();
			SurvivalCore.isChatEvent = true;
			TSPlayer.All.SendMessage("[i:889] [c/595959:⮘] [c/00cc66:Event] [c/595959:⮚] Kto napisze najszybciej [c/00cc66:" + SurvivalCore.ChatEvent_Word + "] wygra 60 €.", new Color(128, 255, 191));
			Thread.Sleep((SurvivalCore.ChatEvent_Word.Length + 8) * 1000);
			if (SurvivalCore.isChatEvent)
			{
				SurvivalCore.chatEventStoper.Reset();
				TSPlayer.All.SendMessage("[i:889] [c/595959:⮘] [c/00cc66:Event] [c/595959:⮚] Nikt nie napisal [c/00cc66:" + SurvivalCore.ChatEvent_Word + "] w czas.", new Color(128, 255, 191));
				SurvivalCore.isChatEvent = false;
				SurvivalCore.chatEventTimer = DateTime.UtcNow;
			}
		}

		private static string RandomWord()
		{
			Random random = new Random();
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
