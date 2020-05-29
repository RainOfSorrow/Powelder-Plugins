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

		public static EConfig Config
		{
			get;
			private set;
		}

		public static void Initialize()
		{
			GeneralHooks.ReloadEvent += OnReload;
			Commands();
		}

		public static void PostInitialize(EventArgs e)
		{
		}

		public static void OnInitialize(EventArgs e)
		{

			Config = EConfig.Read("powelder/economy/config.json");
			if (!File.Exists("powelder/economy/config.json"))
			{
				Config.Write("powelder/economy/config.json");
			}

			Setup.SetupDb(PowelderAPI.PowelderApi.Db);
		}

		public static void OnReload(ReloadEventArgs e)
		{
			Config = EConfig.Read("powelder/economy/config.json");
			if (!File.Exists("powelder/economy/config.json"))
			{
				Config.Write("powelder/economy/config.json");
			}
		}

		public static void Commands()
		{
			TShockAPI.Commands.ChatCommands.Add(new Command("server.admin", ECommands.Admin, "economy", "eco"));
			TShockAPI.Commands.ChatCommands.Add(new Command("server.gracz", ECommands.Transfer, "przelej"));
			TShockAPI.Commands.ChatCommands.Add(new Command("server.gracz", ECommands.Currency, "konto"));
			TShockAPI.Commands.ChatCommands.Add(new Command("server.gracz", ECommands.CollectMoney, "odbierz"));
			TShockAPI.Commands.ChatCommands.Add(new Command("server.admin", SCommands.ShopAdmin, "asklep", "ashop"));
			TShockAPI.Commands.ChatCommands.Add(new Command("server.gracz", SCommands.ShopUser, "sklep"));
			TShockAPI.Commands.ChatCommands.Add(new Command("server.gracz", Reforge.ReforgeCost, "przekuj"));
			TShockAPI.Commands.ChatCommands.Add(new Command("server.gracz", Casino.CasinoCommand, "kasyno"));
		}
	}
}
