using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using SurvivalCore.Economy.Database;
using SurvivalCore.Economy.Gambling;
using SurvivalCore.Economy.Shop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using TShockAPI;
using TShockAPI.Hooks;

namespace SurvivalCore.Economy
{
	public class Economy
	{
		public static string DFormat = "dd.MM.yyyy HH:mm:ss";

		public static List<Product> Products = new List<Product>();

		public static EConfig config
		{
			get;
			private set;
		}

		public static void Initialize()
		{
			GeneralHooks.ReloadEvent += OnReload;
			commands();
		}

		public static void PostInitialize(EventArgs e)
		{
		}

		public static void OnInitialize(EventArgs e)
		{

			config = EConfig.Read("powelder/economy/config.json");
			if (!File.Exists("powelder/economy/config.json"))
			{
				config.Write("powelder/economy/config.json");
			}

			Setup.SetupDb(PowelderAPI.PowelderAPI.Db);
		}

		public static void OnReload(ReloadEventArgs e)
		{
			config = EConfig.Read("powelder/economy/config.json");
			if (!File.Exists("powelder/economy/config.json"))
			{
				config.Write("powelder/economy/config.json");
			}
		}

		public static void commands()
		{
			Commands.ChatCommands.Add(new Command("server.admin", ECommands.admin, "economy", "eco"));
			Commands.ChatCommands.Add(new Command("server.gracz", ECommands.transfer, "przelej"));
			Commands.ChatCommands.Add(new Command("server.gracz", ECommands.currency, "konto"));
			Commands.ChatCommands.Add(new Command("server.gracz", ECommands.collectMoney, "odbierz"));
			Commands.ChatCommands.Add(new Command("server.admin", SCommands.ShopAdmin, "asklep", "ashop"));
			Commands.ChatCommands.Add(new Command("server.gracz", SCommands.ShopUser, "sklep"));
			Commands.ChatCommands.Add(new Command("server.gracz", Reforge.ReforgeCost, "przekuj"));
			Commands.ChatCommands.Add(new Command("server.gracz", Casino.casino, "kasyno"));
		}
	}
}
