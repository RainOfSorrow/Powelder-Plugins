using Microsoft.Xna.Framework;
using OTAPI;
using SurvivalCore.Economy.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace SurvivalCore
{
	[ApiVersion(2, 1)]
	public class SurvivalCore : TerrariaPlugin
	{
		public static List<string> excluded = new List<string>
		{
			"Iwobos",
			"Flame",
			"Momo"
		};


		private static byte cleancount = 0;

		private static bool isClean = false;

		private static byte statusTick = 0;

		public static bool isReload = false;

		public static DateTime chatEventTimer = DateTime.UtcNow;

		public static Stopwatch chatEventStoper = new Stopwatch();

		public static string ChatEvent_Word = null;

		public static bool isChatEvent = false;

		public static byte tickPing = 0;
		public static Dictionary<byte, int> Ping = new Dictionary<byte, int>();
		public static Dictionary<byte, int[]> PingMeasure = new Dictionary<byte, int[]>();

		public static Dictionary<byte, Stopwatch> stoper = new Dictionary<byte, Stopwatch>();

		public static bool[] isStatusBusy = new bool[256];

		public static bool[] isDeathMessage = new bool[256];

		public static SrvPlayer[] srvPlayers = new SrvPlayer[256];

		public static Dictionary<byte, Stopwatch> playTime = new Dictionary<byte, Stopwatch>();

		public static DateTime genTrees = DateTime.UtcNow;

		public static DateTime genOres = DateTime.UtcNow;

		public static Dictionary<int, DateTime> guardians = new Dictionary<int, DateTime>();

		public static Config config
		{
			get;
			private set;
		}

		public override string Name => "Survival Core";

		public override Version Version => new Version(1, 0, 0);

		public override string Author => "Xedlefix";

		public override string Description => "SurvivalCore";

		public SurvivalCore(Main game)
			: base(game)
		{
			base.Order = 1001;
		}

		public override void Initialize()
		{
			for (int i = 0; i != isStatusBusy.Length; i++)
			{
				isStatusBusy[i] = false;
			}
			for (int j = 0; j != isDeathMessage.Length; j++)
			{
				isDeathMessage[j] = false;
			}
			Commands.ChatCommands.Add(new Command("server.owner", SrvCommands.broadcastXedlefix, "cxed"));
			Commands.ChatCommands.Add(new Command("server.owner", SrvCommands.broadcastIwobos, "ciwo"));

			Commands.ChatCommands.Add(new Command("server.jmod", SrvCommands.StaffChat, "staff", "s"));
			Commands.ChatCommands.Add(new Command("server.jmod", SrvCommands.clearChat, "clearchat", "cc"));
			Commands.ChatCommands.Add(new Command("server.jmod", SrvCommands.Vanish, "vanish", "v"));

			Commands.ChatCommands.Add(new Command("server.gracz", Change.Command, "wytworz"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.HelpOp, "adminhelp", "ah"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.statusOptions, "status"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.DeathMessages, "zgony"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.GetPlayTime, "czasgry"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.top, "top"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.Przywolywanie, "przywolaj", "przyw"));
			Commands.ChatCommands.Add(new Command("server.gracz", ExtendedChat.CommandItem, "prefixitem", "pitem"));
			Commands.ChatCommands.Add(new Command("server.gracz", ExtendedChat.CommandNick, "nickcolor", "ncolor"));
			Commands.ChatCommands.Add(new Command("server.debug", ExtendedChat.Debug, "srvdebug"));

			//Economy Hooks
			ServerApi.Hooks.GameInitialize.Register(this, Economy.Economy.OnInitialize);
			ServerApi.Hooks.GamePostInitialize.Register(this, Economy.Economy.PostInitialize);
			Economy.Economy.Initialize();

			//Hooks Other
			PowelderAPI.PowelderChat.OnPowelderChat += ExtendedChat.onChat;

			//Survival Hooks
			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
			ServerApi.Hooks.NetGetData.Register(this, onGetData);
			ServerApi.Hooks.GameUpdate.Register(this, onUpdate);
			ServerApi.Hooks.WorldSave.Register(this, onWorldSave);
			ServerApi.Hooks.ServerJoin.Register(this, onJoin);
			ServerApi.Hooks.ServerLeave.Register(this, onLeave);
			ServerApi.Hooks.ServerConnect.Register(this, onConnect);
			ServerApi.Hooks.GamePostInitialize.Register(this, PostInitialize);
			ServerApi.Hooks.NetSendData.Register(this, onSendData);
			AccountHooks.AccountCreate += onAccountC;
			AccountHooks.AccountDelete += onAccountD;
			PlayerHooks.PlayerPostLogin += onPlayerPostLogin;
			PlayerHooks.PlayerLogout += onPlayerLogout;
			GeneralHooks.ReloadEvent += onReload;
			Hooks.Npc.PostUpdate += npcPostUpdate;
		}

		private void onReload(ReloadEventArgs args)
		{
			Change.config = ChangeConfig.Read("powelder/recipes.json");
			if (!File.Exists("powelder/recipes.json"))
			{
				Change.config.Write("powelder/recipes.json");
			}
			Change.recipes = new Dictionary<int, Recipe>();
			for (int i = 0; i < Change.config.recipes.Count(); i++)
			{
				Change.recipes.Add(i, Change.config.recipes[i]);
			}
		}

		private void onPlayerLogout(PlayerLogoutEventArgs args)
		{
			srvPlayers[(byte)args.Player.Index] = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, global::SurvivalCore.Economy.Economy.OnInitialize);
				ServerApi.Hooks.GameUpdate.Deregister(this, onUpdate);
				ServerApi.Hooks.WorldSave.Deregister(this, onWorldSave);
				ServerApi.Hooks.NetGetData.Deregister(this, onGetData);
				ServerApi.Hooks.ServerJoin.Deregister(this, onJoin);
				ServerApi.Hooks.ServerLeave.Deregister(this, onLeave);
				ServerApi.Hooks.ServerConnect.Deregister(this, onConnect);
				ServerApi.Hooks.NetSendData.Deregister(this, onSendData);
				AccountHooks.AccountCreate -= onAccountC;
				AccountHooks.AccountDelete -= onAccountD;
				PlayerHooks.PlayerPostLogin -= onPlayerPostLogin;
				Hooks.Npc.PostUpdate = (Hooks.Npc.PostUpdateHandler)Delegate.Remove(Hooks.Npc.PostUpdate, new Hooks.Npc.PostUpdateHandler(npcPostUpdate));
			}
			base.Dispose(disposing);
		}

		private void onPlayerPostLogin(PlayerPostLoginEventArgs args)
		{
			srvPlayers[args.Player.Index] = DataBase.getSrvPlayer(args.Player.Account);
			playTime[(byte)args.Player.Index].Start();
			args.Player.SendData(PacketTypes.CreateCombatTextExtended, "Witamy na serwerze! ;)", (int)Colors.RarityGreen.PackedValue, args.Player.X, args.Player.Y);
		}

		private void onSendData(SendDataEventArgs args)
		{
			//if (!args.Handled || args.MsgId == PacketTypes.Status )
			//{
			//	if (!args.text.ToString().StartsWith("/>"))
			//	{
			//		args.Handled = true;
			//		return;
			//	}
			//}
		}

		private void npcPostUpdate(NPC npc, int i)
		{
			if (npc == null || i > Main.npc.Length - 1 || i < 0 || !npc.active)
			{
				return;
			}
			if (npc.type == 68)
			{
				if (!guardians.ContainsKey(i))
				{
					guardians.Add(i, DateTime.UtcNow);
				}
			}
			else if (Main.dayTime && (npc.type == 127 || npc.type == 35))
			{
				Main.npc[i].active = false;
				TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
			}
		}

		private void PostInitialize(EventArgs args)
		{
			isReload = true;
			global::SurvivalCore.Economy.Economy.Products = QueryShop.getProducts();
			TSPlayer.Server.SendInfoMessage("[Economy Shop] Pomyslnie zaladowano {0} produktow.", global::SurvivalCore.Economy.Economy.Products.Count);
			isReload = false;
			Thread thread = new Thread(OneSecondThread)
			{
				IsBackground = true
			};
			thread.Start();
			Thread thread2 = new Thread(EventThreadRun)
			{
				IsBackground = true
			};
			thread2.Start();
			Thread thread3 = new Thread(saveThread)
			{
				IsBackground = true
			};
			thread3.Start();
			Thread thread4 = new Thread(pingThread)
			{
				IsBackground = true
			};
			thread4.Start();
		}

		private void pingThread()
		{
			while (true)
			{
				Thread.Sleep(1500);

				tickPing++;

				if (tickPing > 4)
					tickPing = 0;

				foreach (TSPlayer item in TShock.Players.Where((TSPlayer p) => p?.Active ?? false))
				{
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					if (stoper.ContainsKey((byte)item.Index))
					{
						stoper[(byte)item.Index] = stopwatch;
					}
					else
					{
						stoper.Add((byte)item.Index, stopwatch);
					}
					item.SendData(PacketTypes.RemoveItemOwner, null, 400);
				}
			}
		}

		private void EventThreadRun()
		{
			chatEventTimer = DateTime.UtcNow;
			genTrees = DateTime.UtcNow;
			genOres = DateTime.UtcNow;
			while (true)
			{
				Thread.Sleep(5000);
				if ((DateTime.UtcNow - chatEventTimer).TotalMinutes >= 30.0)
				{
					chatEventTimer = DateTime.UtcNow;
					Thread thread = new Thread(ExtendedChat.chatEventThread)
					{
						IsBackground = true
					};
					thread.Start();
				}
				if ((DateTime.UtcNow - genTrees).TotalMinutes >= 65.0)
				{
					WorldRegeneration.DoTrees();
					genTrees = DateTime.UtcNow;
				}
				if (!((DateTime.UtcNow - genOres).TotalMinutes >= 700.0))
				{
					continue;
				}
				TSPlayer.All.SendInfoMessage("Generacja rud.");
				WorldRegeneration.DoOres("tin", 70f);
				WorldRegeneration.DoOres("copper", 70f);
				WorldRegeneration.DoOres("lead", 60f);
				WorldRegeneration.DoOres("iron", 60f);
				WorldRegeneration.DoOres("silver", 50f);
				WorldRegeneration.DoOres("tungsten", 50f);
				WorldRegeneration.DoOres("gold", 40f);
				WorldRegeneration.DoOres("platinum", 40f);
				WorldRegeneration.DoOres("demonite", 30f);
				WorldRegeneration.DoOres("crimtane", 30f);
				WorldRegeneration.DoOres("hellstone", 20f);
				if (Main.hardMode)
				{
					WorldRegeneration.DoOres("cobalt", 50f);
					WorldRegeneration.DoOres("palladium", 50f);
					WorldRegeneration.DoOres("mythril", 40f);
					WorldRegeneration.DoOres("orichalcum", 40f);
					WorldRegeneration.DoOres("adamantite", 30f);
					WorldRegeneration.DoOres("titanium", 30f);
					if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
					{
						WorldRegeneration.DoOres("chlorophyte", 15f);
					}
				}
				genOres = DateTime.UtcNow;
			}
		}

		private void onWorldSave(WorldSaveEventArgs args)
		{
			TSPlayer[] players = TShock.Players;
			foreach (TSPlayer tSPlayer in players)
			{
				if (tSPlayer != null && tSPlayer.Account != null)
				{
					DataBase.SrvPlayerUpdate(srvPlayers[tSPlayer.Index]);
				}
			}
		}

		public static void OnInitialize(EventArgs e)
		{

			config = Config.Read("powelder/srvconfig.json");
			if (!File.Exists("powelder/srvconfig.json"))
			{
				config.Write("powelder/srvconfig.json");
			}
			Change.config = ChangeConfig.Read("powelder/recipes.json");
			if (!File.Exists("powelder/recipes.json"))
			{
				Change.config.Write("powelder/recipes.json");
			}
			Change.recipes = new Dictionary<int, Recipe>();
			for (int i = 0; i < Change.config.recipes.Count(); i++)
			{
				Change.recipes.Add(i, Change.config.recipes[i]);
			}

			DataBase.SetupDb(PowelderAPI.PowelderAPI.Db);
		}

		private void onConnect(ConnectEventArgs args)
		{
			TSPlayer tSPlayer = new TSPlayer(args.Who);
		}

		private void onJoin(JoinEventArgs args)
		{
			TSPlayer tSPlayer = TShock.Players[args.Who];
			UserAccount user = tSPlayer.Account;
			try
			{
				playTime.Add((byte)args.Who, new Stopwatch());
				user = TShock.UserAccounts.GetUserAccountByName(tSPlayer.Name);
				tSPlayer.Group = TShock.Groups.GetGroupByName(user.Group);
				tSPlayer.Account = user;
				if (isReload && !tSPlayer.HasPermission("server.mod"))
				{
					tSPlayer.SendData(PacketTypes.Disconnect, "Serwer jest w trakcie ladowania zasobow. Sprobuj dolaczyc za chwile.");
					args.Handled = true;
					return;
				}
				if (tSPlayer.HasPermission("tshock.godmode"))
				{
					tSPlayer.GodMode = true;
				}
			}
			catch (NullReferenceException)
			{
			}
			if (user == null)
			{
				return;
			}
			srvPlayers[args.Who] = DataBase.getSrvPlayer(user);
			playTime[(byte)args.Who].Start();
		}

		private void onLeave(LeaveEventArgs args)
		{
			byte who = (byte)args.Who;
			if (playTime.ContainsKey((byte)who))
			{
				playTime[who].Stop();
				srvPlayers[who].playTime += (long)playTime[(byte)who].Elapsed.TotalSeconds;
				DataBase.updatePlayTime(TShock.Players[who].Account, srvPlayers[who].playTime);
				playTime.Remove((byte)who);
			}
			if (srvPlayers[who] != null)
			{
				DataBase.SrvPlayerUpdate(srvPlayers[who]);
				srvPlayers[who] = null;
			}
			isDeathMessage[who] = false;
		}

		private void onGetData(GetDataEventArgs args)
		{
			if (!args.Handled)
			{
				if (!args.Handled && args.MsgID == PacketTypes.PlayerDeathV2)
				{
					using (BinaryReader read = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.playerDeath(read, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.ItemOwner)
				{
					using (BinaryReader read2 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.itemOwner(read2, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.Tile)
				{
					using (BinaryReader read3 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.Tile(read3, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.ItemDrop)
				{
					using (BinaryReader read4 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.itemDrop(read4, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.PlayerTeam)
				{
					using (BinaryReader read5 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.playerTeam(read5, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.TogglePvp)
				{
					using (BinaryReader read6 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.togglePvP(read6, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.PlayerInfo)
				{
					using (BinaryReader read7 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.playerInfo(read7, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.SpawnBossorInvasion)
				{
					using (BinaryReader read8 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.bossorinvasionstart(read8);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.NpcStrike)
				{
					using (BinaryReader read9 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.strikeNPC(read9, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.PlaceItemFrame)
				{
					using (BinaryReader read10 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.itemFrame(read10, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.NpcSpecial)
				{
					using (BinaryReader read11 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = NPI.specialNPCEffect(read11, TShock.Players[args.Msg.whoAmI]);
					}
				}
			}
		}

		private void onUpdate(EventArgs args)
		{
			if (Main.time != 0.0 || isClean)
			{
				return;
			}
			int num = 0;
			Item[] item = Main.item;
			foreach (Item item2 in item)
			{
				if (item2.active)
				{
					num++;
				}
			}
			if (num >= 75)
			{
				isClean = true;
				cleancount = 60;
				TSPlayer.All.SendMessage($"[i:536] [c/595959:⮘] [c/0099ff:Clean Bot] [c/595959:⮚] Za [c/0099ff:{cleancount}] sekund zostana usuniete wszystkie przedmioty z ziemi.", new Color(102, 153, 255));
			}
		}

		private void onAccountC(AccountCreateEventArgs args)
		{
			DataBase.createSrvPlayer(TShock.UserAccounts.GetUserAccountByName(args.Account.Name));
			DataBase.createPlayTime(TShock.UserAccounts.GetUserAccountByName(args.Account.Name));
		}

		private void onAccountD(AccountDeleteEventArgs args)
		{
			DataBase.removeSrvPlayer(TShock.UserAccounts.GetUserAccountByName(args.Account.Name));
			DataBase.removePlayTime(TShock.UserAccounts.GetUserAccountByName(args.Account.Name));
		}

		protected string RepeatLineBreaks(int number)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < number; i++)
			{
				stringBuilder.Append("\r\n");
			}
			return stringBuilder.ToString();
		}

		private void OneSecondThread()
		{
			while (true)
			{
				try
				{
					while (true)
					{
						Thread.Sleep(1000);
						byte[] array = new byte[201];
						foreach (int key2 in guardians.Keys)
						{
							if ((DateTime.UtcNow - guardians[key2]).TotalMilliseconds >= 5888.0)
							{
								Main.npc[key2].active = false;
								TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", key2);
								array[key2] = (byte)key2;
							}
						}
						byte[] array2 = array;
						foreach (byte key in array2)
						{
							guardians.Remove(key);
						}
						if (isClean)
						{
							cleancount--;
							if (cleancount == 30)
							{
								TSPlayer.All.SendMessage($"[i:536] [c/595959:⮘] [c/0099ff:Clean Bot] [c/595959:⮚] Za [c/0099ff:{cleancount}] sekund zostana usuniete wszystkie przedmioty z ziemi.", new Color(102, 153, 255));
							}
							else if (cleancount == 5)
							{
								TSPlayer.All.SendMessage($"[i:536] [c/595959:⮘] [c/0099ff:Clean Bot] [c/595959:⮚] Za [c/0099ff:{cleancount}] sekund zostana usuniete wszystkie przedmioty z ziemi.", new Color(102, 153, 255));
							}
							else if (cleancount == 0)
							{
								int num = 0;
								Item[] item = Main.item;
								foreach (Item item2 in item)
								{
									if (item2.active)
									{
										item2.active = false;
										TSPlayer.All.SendData(PacketTypes.ItemDrop, "", Array.IndexOf(Main.item, item2));
										num++;
									}
								}
								TSPlayer.All.SendMessage($"[i:536] [c/595959:⮘] [c/0099ff:Clean Bot] [c/595959:⮚] Usunieto [c/0099ff:{num}] przedmiotow z ziemi.", new Color(102, 153, 255));
								isClean = false;
							}
						}
						string text = "     « Powelder »          ";
						if (statusTick > 5)
						{
							statusTick = 0;
						}
						if (statusTick == 1)
						{
							text = "    «  Powelder  »         ";
						}
						else if (statusTick == 2)
						{
							text = "   «   Powelder   »        ";
						}
						else if (statusTick == 3)
						{
							text = "   »   Powelder   «        ";
						}
						else if (statusTick == 4)
						{
							text = "    »  Powelder  «         ";
						}
						else if (statusTick == 5)
						{
							text = "     » Powelder «          ";
						}
						statusTick++;
						TSPlayer[] players = TShock.Players;
						foreach (TSPlayer tSPlayer in players)
						{
							if (tSPlayer != null && tSPlayer.Account != null)
							{
								if (SavingFormat.IsTrue(srvPlayers[tSPlayer.Index].statusOptions, 0) && !isStatusBusy[tSPlayer.Index])
								{
									string text2 = string.Format("{9}{0}{1}{2}{3}{4}{5}{6}{7}\n{0}{8}",
										text, SavingFormat.IsTrue(srvPlayers[tSPlayer.Index].statusOptions, 1) ? $"\n\rOnline: {TShock.Utils.GetActivePlayerCount()}" : null,
										(!SavingFormat.IsTrue(srvPlayers[tSPlayer.Index].statusOptions, 7)) ? null : (Ping.ContainsKey((byte)tSPlayer.Index) ? ("\n\rPing: " + Ping[(byte)tSPlayer.Index] + "ms") : "\n\rPing: -ms"),
										SavingFormat.IsTrue(srvPlayers[tSPlayer.Index].statusOptions, 2) ? $"\n\rKonto: {srvPlayers[tSPlayer.Index].Money:n0} {Economy.Economy.config.ValueName}" : null,
										SavingFormat.IsTrue(srvPlayers[tSPlayer.Index].statusOptions, 3) ? $"\n\rZgony: {srvPlayers[tSPlayer.Index].Deaths}" : null,
										SavingFormat.IsTrue(srvPlayers[tSPlayer.Index].statusOptions, 6) ? $"\n\rPVP: {srvPlayers[tSPlayer.Index].pvpKills}/{srvPlayers[tSPlayer.Index].pvpDeaths} | {((srvPlayers[tSPlayer.Index].pvpDeaths == 0) ? ((double)srvPlayers[tSPlayer.Index].pvpKills) : Math.Round((double)srvPlayers[tSPlayer.Index].pvpKills / (double)srvPlayers[tSPlayer.Index].pvpDeaths, 2))}" : null,
										null,
										(!SavingFormat.IsTrue(srvPlayers[tSPlayer.Index].statusOptions, 5)) ? null : (isClean ? ("\n\rClean: " + cleancount + " sec") : null),
										RepeatLineBreaks(75),
										RepeatLineBreaks(11));
									tSPlayer.SendData(PacketTypes.Status, text2);
								}
							}
						}
					}
				}
				catch (NullReferenceException)
				{
				}
			}
		}
		private static void saveThread()
		{
			while (true)
			{
				Thread.Sleep(8389);
				try
				{
					TSPlayer[] players = TShock.Players;
					foreach (TSPlayer tSPlayer in players)
					{
						if (tSPlayer != null && srvPlayers[tSPlayer.Index] != null)
						{
							Thread.Sleep(111);
							playTime[(byte)tSPlayer.Index].Stop();
							srvPlayers[tSPlayer.Index].playTime += (long)playTime[(byte)tSPlayer.Index].Elapsed.TotalSeconds;
							playTime[(byte)tSPlayer.Index].Restart();
							DataBase.updatePlayTime(tSPlayer.Account, srvPlayers[tSPlayer.Index].playTime);
							DataBase.SrvPlayerUpdate(srvPlayers[tSPlayer.Index]);
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
