using Microsoft.Xna.Framework;
using OTAPI;
using SurvivalCore.Economy.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MySql.Data.MySqlClient;
using PowelderAPI;
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
			"Flame"
		};

		public static byte BoostBuffType = 0;
		public static DateTime BoostBuffEndTime;

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
			Commands.ChatCommands.Add(new Command("server.boost", SrvCommands.BoostCommand, "boost"));
			Commands.ChatCommands.Add(new Command("server.gracz", ExtendedChat.CommandPrefixItem, "prefixitem", "pitem"));
			Commands.ChatCommands.Add(new Command("server.gracz", ExtendedChat.CommandNickColor, "nickcolor", "ncolor"));
			Commands.ChatCommands.Add(new Command("server.postep", Goals.ProgressCommand, "postep"));
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
			ServerApi.Hooks.WorldSave.Register(this, OnWorldSave);
			ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
			ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
			ServerApi.Hooks.ServerConnect.Register(this, OnConnect);
			ServerApi.Hooks.GamePostInitialize.Register(this, PostInitialize);
			ServerApi.Hooks.NetSendData.Register(this, OnSendData);
			GetDataHandlers.KillMe += Npi.PlayerDeath;
			GetDataHandlers.NewProjectile += Npi.NewProjectile;
			AccountHooks.AccountCreate += OnAccountC;
			AccountHooks.AccountDelete += OnAccountD;
			PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;
			PlayerHooks.PlayerLogout += OnPlayerLogout;
			GeneralHooks.ReloadEvent += OnReload;
			Hooks.Npc.PostUpdate += NpcPostUpdate;
			

			//TShock.CharacterDB = new CharacterManager(new MySqlConnection($"Server=54.38.50.59; Port=3306; Database=www497_powelder_survival; Uid=www497_powelder_survival; Pwd=HorwAYBmXqYqeUsgqdBu;"));
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
				Goals.ProgressUpdate();
				
				ServerApi.Hooks.GameInitialize.Deregister(this, global::SurvivalCore.Economy.Economy.OnInitialize);
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
				return;
			}

			int bossId = (npc.type == 233 || npc.type == 13 || npc.type == 14 || npc.type == 15 ? 6969 : npc.type);
			
			if (i == 126 || i == 125 || i == 134 || i == 127 || i == 128 || i == 129 ||
			    i == 130 || i == 131)
			{
				bossId = 42069;
			}
			
			if (!Goals.IsDone(bossId))
			{
				Main.npc[i].active = false;
				TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", i);
			}
		}

		private void PostInitialize(EventArgs args)
		{
			IsReload = true;
			Economy.Economy.Products = QueryShop.GetProducts();
			TSPlayer.Server.SendInfoMessage("[Economy Shop] Pomyslnie zaladowano {0} produktow.", global::SurvivalCore.Economy.Economy.Products.Count);
			Goals.ProgressLoad();
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
			
			new Thread(GiveOutMoneyThread)
			{
				IsBackground = true
			}.Start(10);

			System.Timers.Timer task = new System.Timers.Timer();
			task.Elapsed += BuffBoostTask;
			task.Interval = 3000;
			task.Enabled = true;
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
				if (!((DateTime.UtcNow - GenOres).TotalMinutes >= 600.0))
				{
					continue;
				}
				TSPlayer.All.SendMessage("[i:1527] [c/595959:;] [c/9FBDCE:Mineraly] [c/595959:;] Wygenerowano nowe zloza mineralne.", new Color(223,230, 255));
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
			
			Goals.ProgressUpdate();
		}

		private static void BuffBoostTask(object source, ElapsedEventArgs args)
		{
			if (BoostBuffType != 0)
			{
				if ((DateTime.Now - BoostBuffEndTime).TotalSeconds > 0)
				{
					TSPlayer.All.SendWarningMessage("Boost zostal zakonczony.");
					File.Delete("boost.txt");
					BoostBuffType = 0;
					return;
				}
				
				for (int i = 0; i < 255; i++)
				{
					if (TShock.Players[i] != null)
					{
						TShock.Players[i].SetBuff(BoostBuffType, 330, true);
					}
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
				if (!args.Handled && args.MsgID == PacketTypes.NpcSpecial)
				{
					using (BinaryReader read11 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						args.Handled = Npi.SpecialNpcEffect(read11, TShock.Players[args.Msg.whoAmI]);
					}
				}
			}
		}

		private void GiveOutMoneyThread(object time)
		{
			Thread.Sleep((int)time*60000);

			Random rand = new Random(Guid.NewGuid().GetHashCode());

			int amount = rand.Next(5, 15);
			
			foreach (var player in TShock.Players.Where(x => x != null))
			{
				player.SendInfoMessage($"[i:1523] [c/595959:;] [c/ffff00:Bogaty Pan] [c/595959:;]  Kazdy z was otrzymal darmowe {amount} €");
				player.SendData(PacketTypes.CreateCombatTextExtended, "Pieniazki", (int) new Color(255, 255, 0).PackedValue, player.TPlayer.Center.X, player.TPlayer.Center.Y, 0.0f, 0);
				if (SrvPlayers[player.Index] != null)
					SrvPlayers[player.Index].Money += amount;
			}
			
			
			new Thread(GiveOutMoneyThread)
			{
				IsBackground = true
			}.Start(rand.Next(45, 85));
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

						TSPlayer[] players = TShock.Players;
						Goal goal = Goals.GetCurrentGoal();
						foreach (TSPlayer tSPlayer in players)
						{
							if (tSPlayer != null && tSPlayer.Account != null)
							{
								if (SavingFormat.IsTrue(SrvPlayers[tSPlayer.Index].StatusOptions, 0) && !IsStatusBusy[tSPlayer.Index])
								{
									string text2 = string.Format("{7} [c/595959:───── «] [c/52e092:Powelder] [c/595959:» ───── ]{0}{1}{2}{3}{4}{5}{6} \n\r [c/595959:───── «] [c/52e092:Survival] [c/595959:» ───── ]",
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
											: $"\n\r[c/66ff66:{goal.Name}][c/595959::] {goal.Progress}/{goal.ToComplete} ({Math.Round((float)goal.Progress / (float)goal.ToComplete, 3) * 100}%)",
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
							PlayTime[(byte)tSPlayer.Index].Stop();
							SrvPlayers[tSPlayer.Index].PlayTime += (long)PlayTime[(byte)tSPlayer.Index].Elapsed.TotalSeconds;
							PlayTime[(byte)tSPlayer.Index].Restart();
							DataBase.UpdatePlayTime(tSPlayer.Account, SrvPlayers[tSPlayer.Index].PlayTime);
							DataBase.SrvPlayerUpdate(SrvPlayers[tSPlayer.Index]);
						}
					}
					Goals.ProgressUpdate();
				}
				catch (Exception)
				{
					//Ignore
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
