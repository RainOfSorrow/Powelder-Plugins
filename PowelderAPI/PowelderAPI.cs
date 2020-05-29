using MySql.Data.MySqlClient;
using PowelderAPI.CommandRegions;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace PowelderAPI
{
	[ApiVersion(2, 1)]
	public class PowelderApi : TerrariaPlugin
	{
		public static IDbConnection Db;

		public static string GlobalDateFormat  = "dd.MM.yyyy HH:mm:ss";

		public Config Config;

		public delegate void PlayerThreadHookD(TSPlayer player);

		public static event PlayerThreadHookD PlayerThreadHook;

		public override string Name => "PowelderAPI";

		public override Version Version => new Version(1, 0, 0);

		public override string Author => "Xedlefix";

		public override string Description => "Rdzen serwerow Powelder";

		public static string PowelderDateFormat 
		{ 
			get { return GlobalDateFormat; }
			private set { }
		}

		public PowelderApi(Main game)
			: base(game)
		{
			base.Order = 1000;
		}

		public override void Initialize()
		{
			//CommandRegions
			ServerApi.Hooks.ServerJoin.Register(this, CommandRegions.CommandRegions.OnJoin);
			ServerApi.Hooks.ServerLeave.Register(this, CommandRegions.CommandRegions.OnLeave);
			ServerApi.Hooks.NetGetData.Register(this, CommandRegions.CommandRegions.OnGetData);
			CommandRegions.CommandRegions.CommandInitialize();

			//Global
			ServerApi.Hooks.ServerChat.Register(this, Chat.OnChat);
			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
			ServerApi.Hooks.GamePostInitialize.Register(this, PostInitialize);
			Commands.ChatCommands.Add(new Command("server.admin", AutoBroadcast.Reload, "broadcastreload"));

			//TempGroup
			Commands.ChatCommands.Remove(Commands.ChatCommands.Find(x => x.Name == "tempgroup"));
			Commands.ChatCommands.Add(new Command("server.admin", TempGroup.TempGroup.TempGroupCommand, "tempgroup"));
			ServerApi.Hooks.ServerJoin.Register(this, TempGroup.TempGroup.OnJoin);
			ServerApi.Hooks.ServerLeave.Register(this, TempGroup.TempGroup.OnLeave);
			ServerApi.Hooks.GameUpdate.Register(this, TempGroup.TempGroup.OnUpdate);
			TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += TempGroup.TempGroup.PostLogin;



		}

		protected void OnInitialize(EventArgs args)
		{
			Config = Config.Read("powelder/srvconfig.json");
			if (!File.Exists("powelder/srvconfig.json"))
			{
				Config.Write("powelder/srvconfig.json");
			}
			if (TShock.Config.StorageType.Equals("mysql", StringComparison.OrdinalIgnoreCase))
			{
				if (string.IsNullOrWhiteSpace(Config.MySqlHost) || string.IsNullOrWhiteSpace(Config.MySqlDbName))
				{
					Console.WriteLine("[Down na pokladzie] Baza danych ma downa xdd.");
					Console.ResetColor();
					return;
				}
				string[] array = Config.MySqlHost.Split(':');
				MySqlConnection val = (MySqlConnection)(object)new MySqlConnection();
				((DbConnection)(object)val).ConnectionString = $"Server={array[0]}; Port={((array.Length == 1) ? "3306" : array[1])}; Database={Config.MySqlDbName}; Uid={Config.MySqlUsername}; Pwd={Config.MySqlPassword};";
				Db = val;
			}
			CrDatabase.SetupDb(Db);
			TempGroup.TempGroupDbManage.SetupDb(Db);
		}

		protected void PostInitialize(EventArgs args)
		{
			CommandRegions.CommandRegions.CRegions = CrDatabase.GetRegions();
			TSPlayer.Server.SendInfoMessage("[CommandRegions] Pomyslnie zaladowano {0} regionow.", CommandRegions.CommandRegions.CRegions.Count);

			new Thread(CommandRegions.CommandRegions.RegionThread)
			{
				IsBackground = true
			}.Start();
			new Thread(AutoBroadcast.StartBroadcast)
			{
				IsBackground = true
			}.Start(); ;

			new Thread(PlayerThread)
			{
				IsBackground = true
			}.Start();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.NetGetData.Deregister(this, CommandRegions.CommandRegions.OnGetData);
				ServerApi.Hooks.ServerLeave.Deregister(this, CommandRegions.CommandRegions.OnLeave);
				ServerApi.Hooks.ServerJoin.Deregister(this, CommandRegions.CommandRegions.OnJoin);
			}
			base.Dispose(disposing);
		}


		private void PlayerThread()
		{
			PlayerThreadHook += delegate(TSPlayer tSPlayer)
			{
				string shortPrefix = Utils.GetShortPrefix(tSPlayer.Group.Name);
				if (shortPrefix != null)
				{
					foreach (TSPlayer item4 in TShock.Players.Where((TSPlayer p) => p?.Active ?? false))
					{
						if (tSPlayer != null && tSPlayer.Account != null && (item4.Team != tSPlayer.Team || tSPlayer.Team == 0))
						{
							string name = tSPlayer.TPlayer.name;
							tSPlayer.TPlayer.name = $"{shortPrefix}[c/{Utils.GetGroupColor(tSPlayer.Group.Name)}:{tSPlayer.Name}]";
							item4.SendData(PacketTypes.PlayerInfo, tSPlayer.TPlayer.name, tSPlayer.Index);
							tSPlayer.TPlayer.name = name;
						}
						else
						{
							item4.SendData(PacketTypes.PlayerInfo, null, tSPlayer.Index);
						}
					}
				}
				else
				{
					NetMessage.SendData(4, -1, -1, NetworkText.FromLiteral(tSPlayer.TPlayer.name), tSPlayer.Index);
				}
			};

			while (true)
			{
				Thread.Sleep(1000);
				foreach (TSPlayer tSPlayer in TShock.Players.Where((TSPlayer p) => p?.Active ?? false))
				{
					PlayerThreadHook.Invoke(tSPlayer);
				}
			}
		}
	}
}
