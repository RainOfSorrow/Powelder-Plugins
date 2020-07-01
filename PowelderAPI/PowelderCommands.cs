using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;

namespace PowelderAPI
{
    public static class PowelderCommands
    {
	    public static void BroadcastXedlefix(CommandArgs args)
		{
			string str = string.Join(" ", args.Parameters);
			TSPlayer.All.SendMessage("[i:3570] [c/595959:;] [c/18B690:Konsola] [c/595959:;] [c/18B690:Xedlefix][c/595959::] " + str, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		public static void BroadcastIwobos(CommandArgs args)
		{
			string str = string.Join(" ", args.Parameters);
			TSPlayer.All.SendMessage("[i:3925] [c/595959:;] [c/27AD34:Konsola] [c/595959:;] [c/27AD34:Iwobos][c/595959::] " + str, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}


		public static void Broadcast(CommandArgs args)
		{
			string str = string.Join(" ", args.Parameters);
			TShock.Log.Info("OGLOSZENIE " + args.Player.Name + ": " + str);

			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null)
				{
					player.SendData(PacketTypes.CreateCombatTextExtended, "Ogloszenie!", (int) new Color(255, 68, 68).PackedValue, player.TPlayer.Center.X, player.TPlayer.Center.Y, 0.0f, 0);
					player.SendMessage("[i:1526] [c/595959:;] [c/ff2222:Ogloszenie] [c/595959:;] " + str, new Color((int) byte.MaxValue, 68, 68));
				}
			}
		}
		
		public static void ChangePassword(CommandArgs args)
		{

			if (args.Parameters.Count < 2)
			{
				args.Player.SendErrorMessage("Uzycie: /changepassword <nick> <nowe haslo>");
				return;
			}
			var account = new UserAccount();
			account.Name = args.Parameters[0];

			TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);

			try
			{
				TShock.UserAccounts.SetUserAccountPassword(account, args.Parameters[1]);
				TShock.Log.ConsoleInfo(args.Player.Name + " zmienil haslo dla " + account.Name);
				args.Player.SendSuccessMessage("Pomyslne zmieniono haslo dla " + account.Name + ".");
			}
			catch (UserAccountNotExistException)
			{
				args.Player.SendErrorMessage("Gracz " + account.Name + " nie istnieje!");
			}
			catch (UserAccountManagerException e)
			{
				args.Player.SendErrorMessage("Cos poszlo nie tak przy zmianie hasla " + account.Name + "! Zglos to dla Administratora!");
				TShock.Log.ConsoleError(e.ToString());
			}
			catch (ArgumentOutOfRangeException)
			{
				args.Player.SendErrorMessage("Haslo musi posiadac wiecej lub " + TShock.Config.MinimumPasswordLength + " znakow.");
			}
		}
    }
}