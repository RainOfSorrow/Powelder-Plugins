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
		public static List<string> Excluded = new List<string>
		{
			"Iwobos",
			"Flame",
			"Momo"
		};


		private static byte _cleancount = 0;

		private static bool _isClean = false;

		private static byte _statusTick = 0;

		public static bool IsReload = false;

		public static DateTime ChatEventTimer = DateTime.UtcNow;

		public static Stopwatch ChatEventStoper = new Stopwatch();

		public static string ChatEventWord = null;

		public static bool IsChatEvent = false;

		public static byte TickPing = 0;
		public static Dictionary<byte, string> Ping = new Dictionary<byte, string>();
		public static Dictionary<byte, int[]> PingMeasure = new Dictionary<byte, int[]>();

		public static Dictionary<byte, Stopwatch> Stoper = new Dictionary<byte, Stopwatch>();

		public static bool[] IsStatusBusy = new bool[256];

		public static bool[] IsDeathMessage = new bool[256];

		public static SrvPlayer[] SrvPlayers = new SrvPlayer[256];

		public static Dictionary<byte, Stopwatch> PlayTime = new Dictionary<byte, Stopwatch>();

		public static DateTime GenTrees = DateTime.UtcNow;

		public static DateTime GenOres = DateTime.UtcNow;

		public static Dictionary<int, DateTime> Guardians = new Dictionary<int, DateTime>();

		public static Config Config
		{
			get;
			private set;
		}

		public override string Name => "Survival Core";

		public override Version Version => new Version(1, 0, 0);

		public override string Author => "Xedlefix";

		public override string Description => "Survival";

		public SurvivalCore(Main game)
			: base(game)
		{
			base.Order = 1001;
		}

		public override void Initialize()
		{
			for (int i = 0; i != IsStatusBusy.Length; i++)
			{
				IsStatusBusy[i] = false;
			}
			for (int j = 0; j != IsDeathMessage.Length; j++)
			{
				IsDeathMessage[j] = false;
			}
			Commands.ChatCommands.Add(new Command("server.jmod", SrvCommands.StaffChat, "staff", "s"));
			Commands.ChatCommands.Add(new Command("server.jmod", SrvCommands.ClearChat, "clearchat", "cc"));

			Commands.ChatCommands.Add(new Command("server.gracz", Change.Command, "wytworz"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.HelpOp, "adminhelp", "ah"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.StatusOptions, "status"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.DeathMessages, "zgony"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.GetPlayTime, "czasgry"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.Top, "top"));
			Commands.ChatCommands.Add(new Command("server.gracz", SrvCommands.Przywolywanie, "przywolaj", "przyw"));
			Commands.ChatCommands.Add(new Command("server.gracz", ExtendedChat.CommandItem, "prefixitem", "pitem"));
			Commands.ChatCommands.Add(new Command("server.gracz", ExtendedChat.CommandNick, "nickcolor", "ncolor"));
			Commands.ChatCommands.Add(new Command("server.debug", ExtendedChat.Debug, "srvdebug"));

			//Economy Hooks
			ServerApi.Hooks.GameInitialize.Register(this, Economy.Economy.OnInitialize);
			ServerApi.Hooks.GamePostInitialize.Register(this, Economy.Economy.PostInitialize);
			Economy.Economy.Initialize();

			//Hooks Other
			PowelderAPI.Chat.OnPowelderChat += ExtendedChat.OnChat;

			//Survival Hooks
			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
			ServerApi.Hooks.WorldSave.Register(this, OnWorldSave);
			ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
			ServerApi.Hooks.ServerConnect.Register(this, OnConnect);
			ServerApi.Hooks.GamePostInitialize.Register(this, PostInitialize);
			ServerApi.Hooks.NetSendData.Register(this, OnSendData);
			GetDataHandlers.KillMe += Npi.PlayerDeath;
			AccountHooks.AccountCreate += OnAccountC;
			AccountHooks.AccountDelete += OnAccountD;
			PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;
			PlayerHooks.PlayerLogout += OnPlayerLogout;
			GeneralHooks.ReloadEvent += OnReload;
			Hooks.Npc.PostUpdate += NpcPostUpdate;
		}

		private void OnReload(ReloadEventArgs args)
		{
			Change.Config = ChangeConfig.Read("powelder/recipes.json");
			if (!File.Exists("powelder/recipes.json"))
			{
				Change.Config.Write("powelder/recipes.json");
			}
			Change.Recipes = new Dictionary<int, Recipe>();
			for (int i = 0; i < Change.Config.Recipes.Count(); i++)
			{
				Change.Recipes.Add(i, Change.Config.Recipes[i]);
			}
		}

		private void OnPlayerLogout(PlayerLogoutEventArgs args)
		{
			SrvPlayers[(byte)args.Player.Index] = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, global::SurvivalCore.Economy.Economy.OnInitialize);
				ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
				ServerApi.Hooks.WorldSave.Deregister(this, OnWorldSave);
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
				ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
				ServerApi.Hooks.ServerConnect.Deregister(this, OnConnect);
				ServerApi.Hooks.NetSendData.Deregister(this, OnSendData);
				AccountHooks.AccountCreate -= OnAccountC;
				AccountHooks.AccountDelete -= OnAccountD;
				PlayerHooks.PlayerPostLogin -= OnPlayerPostLogin;
				Hooks.Npc.PostUpdate = (Hooks.Npc.PostUpdateHandler)Delegate.Remove(Hooks.Npc.PostUpdate, new Hooks.Npc.PostUpdateHandler(NpcPostUpdate));
			}
			base.Dispose(disposing);
		}

		private void OnPlayerPostLogin(PlayerPostLoginEventArgs args)
		{
			SrvPlayers[args.Player.Index] = DataBase.GetSrvPlayer(args.Player.Account);
			PlayTime[(byte)args.Player.Index].Start();
			args.Player.SendData(PacketTypes.CreateCombatTextExtended, "Witamy na serwerze! ;)", (int)Colors.RarityGreen.PackedValue, args.Player.X, args.Player.Y);
		}

		private void OnSendData(SendDataEventArgs args)
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

		private void NpcPostUpdate(NPC npc, int i)
		{
			if (npc == null || i > Main.npc.Length - 1 || i < 0 || !npc.active)
			{
				return;
			}
			if (npc.type == 68)
			{
				if (!Guardians.ContainsKey(i))
				{
					Guardians.Add(i, DateTime.UtcNow);
				}
			}
			else if (Main.dayTime && (npc.type == 127 || npc.type == 35 || npc.type == 636))
			{
				Main.npc[i].active = false;
				TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
			}
		}

		private void PostInitialize(EventArgs args)
		{
			IsReload = true;
			global::SurvivalCore.Economy.Economy.Products = QueryShop.GetProducts();
			TSPlayer.Server.SendInfoMessage("[Economy Shop] Pomyslnie zaladowano {0} produktow.", global::SurvivalCore.Economy.Economy.Products.Count);
			IsReload = false;
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
			Thread thread3 = new Thread(SaveThread)
			{
				IsBackground = true
			};
			thread3.Start();
			Thread thread4 = new Thread(PingThread)
			{
				IsBackground = true
			};
			thread4.Start();
		}

		private void PingThread()
		{
			while (true)
			{
				Thread.Sleep(1500);

				TickPing++;

				if (TickPing > 4)
					TickPing = 0;

				foreach (TSPlayer item in TShock.Players.Where((TSPlayer p) => p?.Active ?? false))
				{
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					if (Stoper.ContainsKey((byte)item.Index))
					{
						Stoper[(byte)item.Index] = stopwatch;
					}
					else
					{
						Stoper.Add((byte)item.Index, stopwatch);
					}
					item.SendData(PacketTypes.RemoveItemOwner, null, 400);
				}
			}
		}

		private void EventThreadRun()
		{
			ChatEventTimer = DateTime.UtcNow;
			GenTrees = DateTime.UtcNow;
			GenOres = DateTime.UtcNow;
			while (true)
			{
				Thread.Sleep(5000);
				if ((DateTime.UtcNow - ChatEventTimer).TotalMinutes >= 30.0)
				{
					ChatEventTimer = DateTime.UtcNow;
					Thread thread = new Thread(ExtendedChat.ChatEventThread)
					{
						IsBackground = true
					};
					thread.Start();
				}
				if ((DateTime.UtcNow - GenTrees).TotalMinutes >= 65.0)
				{
					WorldRegeneration.DoTrees();
					GenTrees = DateTime.UtcNow;
				}
				if (!((DateTime.UtcNow - GenOres).TotalMinutes >= 700.0))
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
				GenOres = DateTime.UtcNow;
			}
		}

		private void OnWorldSave(WorldSaveEventArgs args)
		{
			TSPlayer[] players = TShock.Players;
			foreach (TSPlayer tSPlayer in players)
			{
				if (tSPlayer != null && tSPlayer.Account != null)
				{
					DataBase.SrvPlayerUpdate(SrvPlayers[tSPlayer.Index]);
				}
			}
		}

		public static void OnInitialize(EventArgs e)
		{

			Config = Config.Read("powelder/srvconfig.json");
			if (!File.Exists("powelder/srvconfig.json"))
			{
				Config.Write("powelder/srvconfig.json");
			}
			Change.Config = ChangeConfig.Read("powelder/recipes.json");
			if (!File.Exists("powelder/recipes.json"))
			{
				Change.Config.Write("powelder/recipes.json");
			}
			Change.Recipes = new Dictionary<int, Recipe>();
			for (int i = 0; i < Change.Config.Recipes.Count(); i++)
			{
				Change.Recipes.Add(i, Change.Config.Recipes[i]);
			}

			DataBase.SetupDb(PowelderAPI.PowelderApi.Db);
		}

		private void OnConnect(ConnectEventArgs args)
		{
			
		}

		private void OnJoin(JoinEventArgs args)
		{
			TSPlayer tSPlayer = TShock.Players[args.Who];
			UserAccount user = tSPlayer.Account;
			try
			{
				if (!PlayTime.ContainsKey((byte)args.Who))
					PlayTime.Add((byte)args.Who, new Stopwatch());
				user = TShock.UserAccounts.GetUserAccountByName(tSPlayer.Name);
				tSPlayer.Group = TShock.Groups.GetGroupByName(user.Group);
				tSPlayer.Account = user;
				if (IsReload && !tSPlayer.HasPermission("server.mod"))
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
			SrvPlayers[args.Who] = DataBase.GetSrvPlayer(user);
			PlayTime[(byte)args.Who].Start();
		}

		private void OnLeave(LeaveEventArgs args)
		{
			byte who = (byte)args.Who;
			if (PlayTime.ContainsKey((byte)who))
			{
				PlayTime[who].Stop();
				SrvPlayers[who].PlayTime += (long)PlayTime[(byte)who].Elapsed.TotalSeconds;
				DataBase.UpdatePlayTime(TShock.Players[who].Account, SrvPlayers[who].PlayTime);
				PlayTime.Remove((byte)who);
			}
			if (SrvPlayers[who] != null)
			{
				DataBase.SrvPlayerUpdate(SrvPlayers[who]);
				SrvPlayers[who] = null;
			}
			IsDeathMessage[who] = false;
		}

		private void OnGetData(GetDataEventArgs args)
		{
			if (!args.Handled)
			{
				if (!args.Handled && args.MsgID == PacketTypes.ItemOwner)
				{
					using (BinaryReader read2 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.ItemOwner(read2, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.Tile)
				{
					using (BinaryReader read3 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.Tile(read3, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.ItemDrop)
				{
					using (BinaryReader read4 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.ItemDrop(read4, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.PlayerTeam)
				{
					using (BinaryReader read5 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.PlayerTeam(read5, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.PlayerInfo)
				{
					using (BinaryReader read7 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.PlayerInfo(read7, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.SpawnBossorInvasion)
				{
					using (BinaryReader read8 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.BossOrInvasionStart(read8);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.NpcStrike)
				{
					using (BinaryReader read9 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.StrikeNpc(read9, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.PlaceItemFrame)
				{
					using (BinaryReader read10 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.ItemFrame(read10, TShock.Players[args.Msg.whoAmI]);
					}
				}
				if (!args.Handled && args.MsgID == PacketTypes.NpcSpecial)
				{
					using (BinaryReader read11 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.SpecialNpcEffect(read11, TShock.Players[args.Msg.whoAmI]);
					}
				}
			}
		}

		private void OnUpdate(EventArgs args)
		{
			if (Main.time != 0.0 || _isClean)
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
				_isClean = true;
				_cleancount = 60;
				TSPlayer.All.SendMessage($"[i:536] [c/595959:⮘] [c/0099ff:Clean Bot] [c/595959:⮚] Za [c/0099ff:{_cleancount}] sekund zostana usuniete wszystkie przedmioty z ziemi.", new Color(102, 153, 255));
			}
		}

		private void OnAccountC(AccountCreateEventArgs args)
		{
			DataBase.CreateSrvPlayer(TShock.UserAccounts.GetUserAccountByName(args.Account.Name));
			DataBase.CreatePlayTime(TShock.UserAccounts.GetUserAccountByName(args.Account.Name));
		}

		private void OnAccountD(AccountDeleteEventArgs args)
		{
			DataBase.RemoveSrvPlayer(TShock.UserAccounts.GetUserAccountByName(args.Account.Name));
			DataBase.RemovePlayTime(TShock.UserAccounts.GetUserAccountByName(args.Account.Name));
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
						foreach (int key2 in Guardians.Keys)
						{
							if ((DateTime.UtcNow - Guardians[key2]).TotalMilliseconds >= 5888.0)
							{
								Main.npc[key2].active = false;
								TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", key2);
								array[key2] = (byte)key2;
							}
						}
						byte[] array2 = array;
						foreach (byte key in array2)
						{
							Guardians.Remove(key);
						}
						if (_isClean)
						{
							_cleancount--;
							if (_cleancount == 30)
							{
								TSPlayer.All.SendMessage($"[i:536] [c/595959:⮘] [c/0099ff:Clean Bot] [c/595959:⮚] Za [c/0099ff:{_cleancount}] sekund zostana usuniete wszystkie przedmioty z ziemi.", new Color(102, 153, 255));
							}
							else if (_cleancount == 5)
							{
								TSPlayer.All.SendMessage($"[i:536] [c/595959:⮘] [c/0099ff:Clean Bot] [c/595959:⮚] Za [c/0099ff:{_cleancount}] sekund zostana usuniete wszystkie przedmioty z ziemi.", new Color(102, 153, 255));
							}
							else if (_cleancount == 0)
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
								TSPlayer.All.SendMessage($"[i:536] [c/595959:;] [c/0099ff:Clean Bot] [c/595959:;] Usunieto [c/0099ff:{num}] przedmiotow z ziemi.", new Color(102, 153, 255));
								_isClean = false;
							}
						}
						TSPlayer[] players = TShock.Players;
						foreach (TSPlayer tSPlayer in players)
						{
							if (tSPlayer != null && tSPlayer.Account != null)
							{
								if (SavingFormat.IsTrue(SrvPlayers[tSPlayer.Index].StatusOptions, 0) && !IsStatusBusy[tSPlayer.Index])
								{
									string text2 = string.Format(">|{7} [c/595959:───── «] [c/52e092:Powelder] [c/595959:» ───── ]{0}{1}{2}{3}{4}{5}{6} \n\r [c/595959:───── «] [c/52e092:Survival] [c/595959:» ───── ]",
										SavingFormat.IsTrue(SrvPlayers[tSPlayer.Index].StatusOptions, 1) //0
											? $"\n\r[c/66ff66:Online][c/595959::] {TShock.Utils.GetActivePlayerCount()}"
											: null,
										(!SavingFormat.IsTrue(SrvPlayers[tSPlayer.Index].StatusOptions, 7)) //1
											? null
											: (Ping.ContainsKey((byte) tSPlayer.Index)
												? ("\n\r[c/66ff66:Ping][c/595959::] " + Ping[(byte)tSPlayer.Index])
												: "\n\r[c/66ff66:Ping][c/595959::] -ms"),
										SavingFormat.IsTrue(SrvPlayers[tSPlayer.Index].StatusOptions, 2) //2
											? $"\n\r[c/66ff66:Konto][c/595959::] {SrvPlayers[tSPlayer.Index].Money:n0} {Economy.Economy.Config.ValueName}"
											: null,
										SavingFormat.IsTrue(SrvPlayers[tSPlayer.Index].StatusOptions, 3) //3
											? $"\n\r[c/66ff66:Zgony][c/595959::] {SrvPlayers[tSPlayer.Index].Deaths}"
											: null,
										SavingFormat.IsTrue(SrvPlayers[tSPlayer.Index].StatusOptions, 6) //4
											? $"\n\r[c/66ff66:PvP][c/595959::] {SrvPlayers[tSPlayer.Index].PvpKills}/{SrvPlayers[tSPlayer.Index].PvpDeaths} | {((SrvPlayers[tSPlayer.Index].PvpDeaths == 0) ? ((double) SrvPlayers[tSPlayer.Index].PvpKills) : Math.Round((double) SrvPlayers[tSPlayer.Index].PvpKills / (double) SrvPlayers[tSPlayer.Index].PvpDeaths, 2))}"
											: null,
										null,
										(!SavingFormat.IsTrue(SrvPlayers[tSPlayer.Index].StatusOptions, 5)) //5
											? null
											: (_isClean ? ("\n\rClean: " + _cleancount + " sec") : null),
										RepeatLineBreaks(10)
									);
										tSPlayer.SendData(PacketTypes.Status, text2, 0 , 3);
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
		private static void SaveThread()
		{
			while (true)
			{
				Thread.Sleep(8389);
				try
				{
					TSPlayer[] players = TShock.Players;
					foreach (TSPlayer tSPlayer in players)
					{
						if (tSPlayer != null && SrvPlayers[tSPlayer.Index] != null)
						{
							Thread.Sleep(111);
							PlayTime[(byte)tSPlayer.Index].Stop();
							SrvPlayers[tSPlayer.Index].PlayTime += (long)PlayTime[(byte)tSPlayer.Index].Elapsed.TotalSeconds;
							PlayTime[(byte)tSPlayer.Index].Restart();
							DataBase.UpdatePlayTime(tSPlayer.Account, SrvPlayers[tSPlayer.Index].PlayTime);
							DataBase.SrvPlayerUpdate(SrvPlayers[tSPlayer.Index]);
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}
		
		private static string RepeatLineBreaks(int number)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < number; i++)
			{
				stringBuilder.Append("\r\n");
			}
			return stringBuilder.ToString();
		}
	}
}
