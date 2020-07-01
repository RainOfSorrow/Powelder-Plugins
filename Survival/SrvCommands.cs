using Microsoft.Xna.Framework;
using SurvivalCore.Economy.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Streams;
using System.Linq;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace SurvivalCore
{
	public class SrvCommands
	{
		public static void Top(TShockAPI.CommandArgs args)
		{
			string text = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}

			switch (text)
			{
				default:
					args.Player.SendMessage("[c/66FF66:Uzycie]: /top <kasa/pvp/czasgry>", Color.Gray);
					break;
				case "kasa":
				{
					Dictionary<string, int> topKasa = QueryPlr.GetTopKasa(args.Player.Name);
					int num3 = 1;
					Color color3 = Color.Gold;
					args.Player.SendMessage("               [c/595959:«]Top 5 - Kasa[c/595959:»]", Color.LightGray);
					foreach (string key in topKasa.Keys)
					{
						args.Player.SendMessage(
							string.Format("{0}# [c/595959:-] {1} [c/595959:(]{2} €[c/595959:)]", num3, key,
								topKasa[key].ToString("N0")), color3);
						num3++;
						switch (num3)
						{
							case 2:
								color3 = Color.Silver;
								break;
							case 3:
								color3 = new Color(205, 127, 50);
								break;
							default:
								color3 = Color.Gray;
								break;
						}
					}

					break;
				}
				case "pvp":
				{
					Dictionary<string, double[]> topPvp = DataBase.GetTopPvp(args.Player.Name);
					int num2 = 1;
					Color color2 = Color.Gold;
					args.Player.SendMessage("              [c/595959:«]Top 5 - PvP[c/595959:»]", Color.LightGray);
					foreach (string key2 in topPvp.Keys)
					{
						args.Player.SendMessage(
							$"{num2}# [c/595959:-] {key2} [c/595959:(]{topPvp[key2][0]}/{topPvp[key2][1]} | {topPvp[key2][2]}[c/595959:)]",
							color2);
						num2++;
						switch (num2)
						{
							case 2:
								color2 = Color.Silver;
								break;
							case 3:
								color2 = new Color(205, 127, 50);
								break;
							default:
								color2 = Color.Gray;
								break;
						}
					}

					break;
				}
				case "czasgry":
				{
					Dictionary<string, TimeSpan> topCzasGry = DataBase.GetTopCzasGry(args.Player.Name);
					int num = 1;
					Color color = Color.Gold;
					args.Player.SendMessage("             [c/595959:«]Top 5 - Czas Gry[c/595959:»]", Color.LightGray);
					foreach (string key3 in topCzasGry.Keys)
					{
						args.Player.SendMessage(
							$"{num}# [c/595959:-] {key3} [c/595959:(]{Math.Round(topCzasGry[key3].TotalHours, 1)} h[c/595959:)]",
							color);
						num++;
						switch (num)
						{
							case 2:
								color = Color.Silver;
								break;
							case 3:
								color = new Color(205, 127, 50);
								break;
							default:
								color = Color.Gray;
								break;
						}
					}

					break;
				}
				case "zgony":
				{
					Dictionary<string, int> topZgony = DataBase.GetTopZgony(args.Player.Name);
					int num = 1;
					Color color = Color.Gold;
					args.Player.SendMessage("               [c/595959:«]Top 5 - Zgony[c/595959:»]", Color.LightGray);
					foreach (string key in topZgony.Keys)
					{
						args.Player.SendMessage(
							$"{num}# [c/595959:-] {key} [c/595959:(]{topZgony[key]}[c/595959:)]",
							color);
						num++;
						switch (num)
						{
							case 2:
								color = Color.Silver;
								break;
							case 3:
								color = new Color(205, 127, 50);
								break;
							default:
								color = Color.Gray;
								break;
						}
					}

					break;
				}
			}
		}

		public static void GetPlayTime(TShockAPI.CommandArgs args)
		{
			string text = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			if (text != null)
			{
				List<TSPlayer> list = TSPlayer.FindByNameOrID(text);
				if (list.Count < 1)
				{
					args.Player.SendErrorMessage("Nie mozna bylo znalezc gracza {0}.", args.Parameters[0]);
					return;
				}
				TimeSpan timeSpan = TimeSpan.FromSeconds(SurvivalCore.SrvPlayers[list[0].Index].PlayTime + (long)SurvivalCore.PlayTime[(byte)list[0].Index].Elapsed.TotalSeconds);
				string text2 = (timeSpan.Seconds == 1) ? "sekunde" : ((timeSpan.Seconds <= 1 || timeSpan.Seconds >= 5) ? "sekund" : "sekundy");
				string text3 = (timeSpan.Minutes == 1) ? "minute" : ((timeSpan.Minutes <= 1 || timeSpan.Minutes >= 5) ? "minut" : "minuty");
				string text4 = ((int)timeSpan.TotalHours == 1) ? "godzine" : (((int)timeSpan.TotalHours <= 1 || (int)timeSpan.TotalHours >= 5) ? "godzin" : "godziny");
				args.Player.SendInfoMessage($"{list[0].Name} lacznie na serwerze spedzil {(int)timeSpan.TotalHours} {text4}, {timeSpan.Minutes} {text3}, {timeSpan.Seconds} {text2}.");
			}
			else
			{
				TimeSpan timeSpan2 = TimeSpan.FromSeconds(SurvivalCore.SrvPlayers[args.Player.Index].PlayTime + (long)SurvivalCore.PlayTime[(byte)args.Player.Index].Elapsed.TotalSeconds);
				string text5 = (timeSpan2.Seconds == 1) ? "sekunde" : ((timeSpan2.Seconds <= 1 || timeSpan2.Seconds >= 5) ? "sekund" : "sekundy");
				string text6 = (timeSpan2.Minutes == 1) ? "minute" : ((timeSpan2.Minutes <= 1 || timeSpan2.Minutes >= 5) ? "minut" : "minuty");
				string text7 = ((int)timeSpan2.TotalHours == 1) ? "godzine" : (((int)timeSpan2.TotalHours <= 1 || (int)timeSpan2.TotalHours >= 5) ? "godzin" : "godziny");
				args.Player.SendInfoMessage($"Lacznie na serwerze spedziles {(int)timeSpan2.TotalHours} {text7}, {timeSpan2.Minutes} {text6}, {timeSpan2.Seconds} {text5}.");
			}
		}

		public static void StaffChat(TShockAPI.CommandArgs args)
		{
			string text = string.Join(" ", args.Parameters);
			if (text == null || text == "")
			{
				args.Player.SendErrorMessage("Uzycie: /s <wiadomosc>");
				return;
			}
			foreach (TSPlayer item in TShock.Players.Where((TSPlayer p) => p?.HasPermission("server.jmod") ?? false))
			{
				item.SendMessage("[i:549] [c/595959:;] [c/2ECC71:Kadra] [c/595959:;] [c/" + PowelderAPI.Utils.GetGroupColor(args.Player.Group.Name) + ":" + args.Player.Name + "]: " + text, new Color(136, 255, 77));
			}
		}

		public static void HelpOp(TShockAPI.CommandArgs args)
		{
			string text = string.Join(" ", args.Parameters);
			if (text == null || text == "")
			{
				args.Player.SendErrorMessage("Uzycie: /adminhelp <wiadomosc>");
				return;
			}
			foreach (TSPlayer item in TShock.Players.Where((TSPlayer p) => p?.HasPermission("server.jmod") ?? false))
			{
				item.SendMessage("[i:548] [c/595959:;] [c/1072F6:AdminHelp] [c/595959:;] [c/" + PowelderAPI.Utils.GetGroupColor(args.Player.Group.Name) + ":" + args.Player.Name + "]: " + text, new Color(75, 171, 255));
			}
			args.Player.SendSuccessMessage("Pomyslne wyslano wiadomosc do wszystkich czlonkow kadry!");
		}

		public static void StatusOptions(TShockAPI.CommandArgs args)
		{
			string text = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}
			switch (text)
			{
			default:
				args.Player.SendErrorMessage("Uzycie: /status <opcja>");
				args.Player.SendInfoMessage("Opcje: cale, online, ping, konto, zgony, pvp, waznosc, postep");
				break;
			case "cale":
				if (SavingFormat.IsTrue(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 0))
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 0, false);
					args.Player.SendSuccessMessage("Status zostal calkowice wylaczony.");
					args.Player.SendData(PacketTypes.Status);
				}
				else
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 0, true);
					args.Player.SendSuccessMessage("Status zostal wlaczony.");
				}
				break;
			case "online":
				if (SavingFormat.IsTrue(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 1))
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 1, false);
					args.Player.SendSuccessMessage("Online w statusie zostal wylaczony.");
				}
				else
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 1, true);
					args.Player.SendSuccessMessage("Online w statusie zostal wlaczony.");
				}
				break;
			case "konto":
				if (SavingFormat.IsTrue(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 2))
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 2, false);
					args.Player.SendSuccessMessage("Konto w statusie zostalo wylaczone.");
				}
				else
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 2, true);
					args.Player.SendSuccessMessage("Konto w statusie zostalo wlaczone.");
				}
				break;
			case "zgony":
				if (SavingFormat.IsTrue(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 3))
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 3, false);
					args.Player.SendSuccessMessage("Zgony w statusie zostaly wylaczone.");
				}
				else
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 3, true);
					args.Player.SendSuccessMessage("Zgony w statusie zostaly wlaczone.");
				}
				break;
			case "waznosc":
				if (SavingFormat.IsTrue(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 4))
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 4, false);
					args.Player.SendSuccessMessage("Waznosc w statusie zostala wylaczona.");
				}
				else
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 4, true);
					args.Player.SendSuccessMessage("Waznosc w statusie zostala wlaczona.");
				}
				break;
			case "Postep":
				if (SavingFormat.IsTrue(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 5))
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 5, false);
					args.Player.SendSuccessMessage("Postep w statusie zostal wylaczony.");
				}
				else
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 5, true);
					args.Player.SendSuccessMessage("Postep w statusie zostal wlaczony.");
				}
				break;
			case "pvp":
				if (SavingFormat.IsTrue(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 6))
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 6, false);
					args.Player.SendSuccessMessage("PVP w statusie zostalo wylaczone.");
				}
				else
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 6, true);
					args.Player.SendSuccessMessage("PVP w statusie zostalo wlaczone.");
				}
				break;
			case "ping":
				if (SavingFormat.IsTrue(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 7))
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 7, false);
					args.Player.SendSuccessMessage("Ping w statusie zostal wylaczony.");
				}
				else
				{
					SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions = SavingFormat.Change(SurvivalCore.SrvPlayers[args.Player.Index].StatusOptions, 7, true);
					args.Player.SendSuccessMessage("Ping w statusie zostal wlaczony.");
				}
				break;
			}
		}

		public static void ClearChat(TShockAPI.CommandArgs args)
		{
			string str = string.Join(" ", args.Parameters);
			int num = 0;
			do
			{
				TSPlayer.All.SendInfoMessage(" ");
				num++;
			}
			while (num != 450);
			if (args.Parameters.Count > 0)
			{
				TSPlayer.All.SendInfoMessage("Czat zostal wyczyszczony - " + str);
			}
			else
			{
				TSPlayer.All.SendInfoMessage("Czat zostal wyczyszczony.");
			}
		}

		public static void DeathMessages(TShockAPI.CommandArgs args)
		{
			if (!SurvivalCore.IsDeathMessage[args.Player.Index])
			{
				SurvivalCore.IsDeathMessage[args.Player.Index] = true;
				args.Player.SendSuccessMessage("Powiadomienia o zgonach zostaly wlaczone.");
			}
			else
			{
				SurvivalCore.IsDeathMessage[args.Player.Index] = false;
				args.Player.SendSuccessMessage("Powiadomienia o zgonach zostaly wylaczone.");
			}
		}

		public static void Przywolywanie(TShockAPI.CommandArgs args)
		{
			string text = null;
			string text2 = null;
			if (args.Parameters.Count > 0)
			{
				text = args.Parameters[0].ToLower();
			}

			if (args.Parameters.Count > 1)
			{
				text2 = args.Parameters[1].ToLower();
			}
			
			if ((SurvivalCore.SrvPlayers[args.Player.Index].BossCooldown - DateTime.Now).TotalSeconds > 0)
			{
				args.Player.SendErrorMessage($"Musisz odczekac jakis czas, aby moc zrespic kolejnego bossa. Mozliwe to bedzie za {PowelderAPI.Utils.ExpireCountDown(SurvivalCore.SrvPlayers[args.Player.Index].BossCooldown)}");
				return;
			}

			switch (text)
			{
				default:
					args.Player.SendErrorMessage("Uzycie: /przywolaj <co>");
					break;
				case "wof":
				case "wall of flesh":
				{

					Item itemById = TShock.Utils.GetItemById(267);
					if (PowelderAPI.Utils.PlayerItemCount(args.Player, itemById) < 1)
					{
						args.Player.SendErrorMessage("Nie masz wymaganych materialow w ekwipunku.");
						args.Player.SendErrorMessage("Wymagane:  [i:267]");
						break;
					}

					if (Main.wofNPCIndex >= 0)
					{
						args.Player.SendErrorMessage("Wall of Flesh juz jest na swiecie.");
						break;
					}

					if (args.Player.Y / 16f < (float) (Main.maxTilesY - 205))
					{
						args.Player.SendErrorMessage("Musisz byc w piekle, aby przywolac Wall of Flesha.");
						break;
					}

					PowelderAPI.Utils.PlayerRemoveItems(args.Player, itemById, 1);
					NPC.SpawnWOF(new Vector2(args.Player.X, args.Player.Y));
					TSPlayer.All.SendInfoMessage("{0} przywolal Wall of Flesha.", args.Player.Name);
					break;
				}
				case "lunatic":
				case "Lun":
				case "lc":
				case "lunatic cultist":
				{
					NPC nPc2 = new NPC();
					nPc2.SetDefaults(439);
					if (!NPC.downedGolemBoss)
					{
						args.Player.SendErrorMessage("Przywolanie bedzie mozliwe po pokonaniu Golema.");
						break;
					}

					if (PowelderAPI.Utils.IsNpcOnWorld(nPc2.type))
					{
						args.Player.SendErrorMessage("Lunatic Cultist juz jest na swiecie.");
						break;
					}
					

					if (PowelderAPI.Utils.PlayerItemCount(args.Player, TShock.Utils.GetItemById(1274)) < 1 ||
					    PowelderAPI.Utils.PlayerItemCount(args.Player, TShock.Utils.GetItemById(148)) < 5)
					{
						args.Player.SendErrorMessage("Nie masz wymaganych materialow w ekwipunku.");
						args.Player.SendErrorMessage("Wymagane:  [i:1274] [i/s5:148]");
						break;
					}

					PowelderAPI.Utils.PlayerRemoveItems(args.Player, TShock.Utils.GetItemById(1274), 1);
					PowelderAPI.Utils.PlayerRemoveItems(args.Player, TShock.Utils.GetItemById(148), 5);
					TSPlayer.Server.SpawnNPC(nPc2.type, nPc2.FullName, 1, args.Player.TileX, args.Player.TileY);
					TSPlayer.All.SendInfoMessage("{0} przywolal Lunatic Cultista.", args.Player.Name);
					break;
				}
				case "martians":
				case "martian madness":
				case "mm":
				case "martian":
				{
					if (!NPC.downedGolemBoss)
					{
						args.Player.SendErrorMessage("Przywolanie bedzie mozliwe po pokonaniu Golema.");
						break;
					}

					if (Main.invasionType != 0)
					{
						args.Player.SendErrorMessage("Na swiecie juz jest jakas inwazja.");
						break;
					}
					

					if (PowelderAPI.Utils.PlayerItemCount(args.Player, TShock.Utils.GetItemById(3118)) < 1 ||
					         PowelderAPI.Utils.PlayerItemCount(args.Player, TShock.Utils.GetItemById(530)) < 95)
					{
						args.Player.SendErrorMessage("Nie masz wymaganych materialow w ekwipunku.");
						args.Player.SendErrorMessage("Wymagane:  [i:3118] [i/s5:148]");
					}
					else
					{
						PowelderAPI.Utils.PlayerRemoveItems(args.Player, TShock.Utils.GetItemById(3118), 1);
						PowelderAPI.Utils.PlayerRemoveItems(args.Player, TShock.Utils.GetItemById(530), 95);
					}

					break;
				}
				case "skeletron":
				case "skele":
				{
					NPC nPc = new NPC();
					nPc.SetDefaults(35);
					if (!NPC.downedBoss3)
					{
						args.Player.SendErrorMessage("Przywolanie bedzie mozliwe po pierwszym pokonaniu Skeletrona.");
						break;
					}

					if (PowelderAPI.Utils.IsNpcOnWorld(nPc.type))
					{
						args.Player.SendErrorMessage("Skeletron juz jest na swiecie.");
						break;
					}

					if (Main.dayTime)
					{
						args.Player.SendErrorMessage("Przywolanie jest mozliwe tylko w nocy.");
						break;
					}
					
					
					if (PowelderAPI.Utils.PlayerItemCount(args.Player, TShock.Utils.GetItemById(1307)) < 1)
					{
						args.Player.SendErrorMessage("Nie masz wymaganych materialow w ekwipunku.");
						args.Player.SendErrorMessage("Wymagane:  [i:1307]");
						break;
					}

					PowelderAPI.Utils.PlayerRemoveItems(args.Player, TShock.Utils.GetItemById(1307), 1);
					TSPlayer.Server.SpawnNPC(nPc.type, nPc.FullName, 1, args.Player.TileX, args.Player.TileY);
					TSPlayer.All.SendInfoMessage("{0} przywolal Skeletrona.", args.Player.Name);
					break;
				}
			}
		}

		public static readonly Dictionary<string, KeyValuePair<byte, int>> avalibleBuffs = new Dictionary<string, KeyValuePair<byte, int>>()
		{
			{
				"iron skin", //pelna nazwa
				new KeyValuePair<byte, int>(5, 2000) //ID i Koszt
			},
			{
				"regeneration",
				new KeyValuePair<byte, int>(2, 1500) 
			},
			{
				"builder",
				new KeyValuePair<byte, int>(107, 700)
			},
			{
				"night owl",
				new KeyValuePair<byte, int>(12, 700)
			},
			{
				"life force",
				new KeyValuePair<byte, int>(113, 2500)
			},
			{
				"shine",
				new KeyValuePair<byte, int>(11, 900)
			},
			{
				"endurance",
				new KeyValuePair<byte, int>(114, 3000)
			},
			{
				"hunter",
				new KeyValuePair<byte, int>(17, 900)
			},
			{
				"swiftness",
				new KeyValuePair<byte, int>(3, 800)
			},
			{
				"honey",
				new KeyValuePair<byte, int>(48, 2500)
			},
			{
				"finch",
				new KeyValuePair<byte, int>(216, 69666420)
			}
		};
			
		public static void BoostCommand(CommandArgs args)
		{

			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("Uzycie: /boost <nazwa buffa/list>");
			}
			else if (args.Parameters[0].ToLower() == "list")
			{
				List<string> list = new List<string>();
				foreach (string key in avalibleBuffs.Keys)
				{
					list.Add($"{key} - [c/ffff00:{avalibleBuffs[key].Value} €]");
				}

				args.Player.SendMessage($"Lista dostepnych buffow:", Color.Green);
				PaginationTools.SendPage(args.Player, 0, PaginationTools.BuildLinesFromTerms(list, null, " | ", 140), new PaginationTools.Settings
				{
					IncludeHeader = false,
					LineTextColor = new Color(192, 192, 192),
					IncludeFooter = false,
					NothingToDisplayString = "Error 404."
				});
			}
			else
			{
				string buff = string.Join(" ", args.Parameters).ToLower();
				
				if (avalibleBuffs.ContainsKey(buff))
				{
					if (SurvivalCore.SrvPlayers[args.Player.Index].Money < avalibleBuffs[buff].Value)
					{
						args.Player.SendErrorMessage($"Nie stac cie na wykupienie boosta na serwerze. Koszt {avalibleBuffs[buff].Value} €");
						return;;
					}

					if ((SurvivalCore.SrvPlayers[args.Player.Index].BoostCooldown - DateTime.Now).Seconds > 0)
					{
						args.Player.SendErrorMessage($"Nastepny boost bedzie mozliwy za {PowelderAPI.Utils.ExpireCountDown(SurvivalCore.SrvPlayers[args.Player.Index].BoostCooldown)}");
						return;;
					}

					if (SurvivalCore.BoostBuffType != 0)
					{
						args.Player.SendErrorMessage($"Aktualnie jest aktywny juz jeden boost. Minie on za {PowelderAPI.Utils.ExpireCountDown(SurvivalCore.BoostBuffEndTime)}");
						return;;
					}
					
					SurvivalCore.SrvPlayers[args.Player.Index].Money -= avalibleBuffs[buff].Value;
					
					SurvivalCore.SrvPlayers[args.Player.Index].BoostCooldown = DateTime.Now.AddDays(2);
					
					args.Player.SendSuccessMessage($"Pomyslnie zboostowales serwer buffem {buff} na 2 godziny. Twoj nastepny boost bedzie mozliwy za 48h.");
					args.Player.SendInfoMessage($"Twoj nowy stan konta: {SurvivalCore.SrvPlayers[args.Player.Index].Money} €");
					TSPlayer.All.SendWarningMessage($"Booster {args.Player.Name} obdarowal serwer buffem {buff} na 2 godziny.");
					SurvivalCore.BoostBuffType = avalibleBuffs[buff].Key;
					SurvivalCore.BoostBuffEndTime = DateTime.Now.AddHours(2);
					
					return;	
				}
				args.Player.SendErrorMessage("Nie znaleziono podanego buffa. Lista pod /boost list");
				
			}
		}
	}
}
