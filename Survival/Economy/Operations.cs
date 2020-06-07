using Microsoft.Xna.Framework;
using TShockAPI;

namespace SurvivalCore.Economy
{
	public class Operations
	{
		public static void SendMoney(TSPlayer sender, TSPlayer receiver, float amount)
		{
			float num = SurvivalCore.SrvPlayers[sender.Index].Money;
			float num2 = SurvivalCore.SrvPlayers[receiver.Index].Money;
			if (num < amount)
			{
				sender.SendErrorMessage("Nie masz tylu pieniedzy na koncie, aby przelac.");
				sender.SendInfoMessage(string.Format("Twoj stan konta: {0} {1}", ((int)num).ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
				return;
			}
			num -= amount;
			int num3 = (int)(amount - amount * Economy.Config.Tax / 100f);
			num2 += (float)num3;
			SurvivalCore.SrvPlayers[sender.Index].Money = (int)num;
			SurvivalCore.SrvPlayers[receiver.Index].Money = (int)num2;
			
			sender.SendSuccessMessage($"Pomyslnie przelano {num3:N0} {Economy.Config.ValueName} do {receiver.Name}.");
			sender.SendInfoMessage(string.Format("Twoj nowy stan konta: {0:} {1}", ((int)num).ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
			
			receiver.SendInfoMessage($"Otrzymano {num3:N0} {Economy.Config.ValueName} od {sender.Name}", Color.Gray);
			receiver.SendInfoMessage(string.Format("Twoj nowy stan konta: {0} {1}", ((int)num2).ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
		}

		public static void SetMoney(TSPlayer sender, TSPlayer receiver, int amount)
		{
			SurvivalCore.SrvPlayers[receiver.Index].Money = amount;
			
			sender.SendSuccessMessage($"Pomyslnie ustawiono nowa ilosc srodkow na koncie gracza {receiver.Name}.");
			
			receiver.SendWarningMessage($"Administrator zmienil twoja ilosc srodkow na koncie.");
			receiver.SendInfoMessage(string.Format("Twoj nowy stan konta: {0} {1}", amount.ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
		}

		public static void AddMoney(TSPlayer sender, TSPlayer receiver, int amount)
		{
			SurvivalCore.SrvPlayers[receiver.Index].Money += amount;
			sender.SendSuccessMessage($"Pomyslnie dodano {amount} {Economy.Config.ValueName} do konta gracza {receiver.Name}.");
			receiver.SendWarningMessage($"Administrator zmienil twoja ilosc srodkow na koncie.");
			receiver.SendInfoMessage(string.Format("Twoj nowy stan konta: {0} {1}", SurvivalCore.SrvPlayers[receiver.Index].Money.ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
		}

		public static void TakeMoney(TSPlayer sender, TSPlayer receiver, int amount)
		{
			int num = SurvivalCore.SrvPlayers[receiver.Index].Money - amount;
			if (num < 0)
			{
				sender.SendErrorMessage("Nie mozna bylo zmienic ilosci srodkow na koncie gracza {0}, poniewaz ilosc srodkow by byla mniejsza niz 0.", receiver.Name);
				return;
			}
			SurvivalCore.SrvPlayers[receiver.Index].Money = num;
			sender.SendSuccessMessage(string.Format("Pomyslnie usunieto {0} {1} od konta gracza {2}.", amount.ToString("N0").Replace(' ', ','), Economy.Config.ValueName, receiver.Name));
			receiver.SendWarningMessage($"Administrator zmienil twoja ilosc srodkow na koncie.");
			receiver.SendInfoMessage(string.Format("Twoj nowy stan konta: {0} {1}", num.ToString("N0").Replace(' ', ','), Economy.Config.ValueName));
		}
	}
}
