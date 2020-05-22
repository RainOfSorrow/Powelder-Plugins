using Microsoft.Xna.Framework;
using TShockAPI;

namespace SurvivalCore.Economy
{
	public class Operations
	{
		public static void sendMoney(TSPlayer sender, TSPlayer reciver, float amount)
		{
			float num = SurvivalCore.srvPlayers[sender.Index].Money;
			float num2 = SurvivalCore.srvPlayers[reciver.Index].Money;
			if (num < amount)
			{
				sender.SendErrorMessage("[c/595959:»]  Nie masz tylu pieniedzy na koncie, aby przelac.");
				sender.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Twoj stan konta][c/595959::] {0} {1}", ((int)num).ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
				return;
			}
			num -= amount;
			int num3 = (int)(amount - amount * Economy.config.Tax / 100f);
			num2 += (float)num3;
			SurvivalCore.srvPlayers[sender.Index].Money = (int)num;
			SurvivalCore.srvPlayers[reciver.Index].Money = (int)num2;
			sender.SendMessage($"[c/595959:»]  Pomyslnie przelano [c/66ff66:{num3:N0}] {Economy.config.ValueName} do [c/ff6666:{reciver.Name}].", Color.Gray);
			sender.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {0:} {1}", ((int)num).ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
			reciver.SendMessage($"[c/595959:»]  Otrzymano [c/66ff66:{num3:N0}] {Economy.config.ValueName} od [c/ff6666:{sender.Name}].", Color.Gray);
			reciver.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {0} {1}", ((int)num2).ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
		}

		public static void setMoney(TSPlayer sender, TSPlayer reciver, int amount)
		{
			SurvivalCore.srvPlayers[reciver.Index].Money = amount;
			sender.SendMessage($"[c/595959:»]  Pomyslnie ustawiono nowa ilosc srodkow na koncie gracza [c/66ff66:{reciver.Name}].", Color.Gray);
			reciver.SendMessage($"[c/595959:»]  Administrator zmienil twoja ilosc srodkow na koncie.", Color.Gray);
			reciver.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {0} {1}", amount.ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
		}

		public static void addMoney(TSPlayer sender, TSPlayer reciver, int amount)
		{
			SurvivalCore.srvPlayers[reciver.Index].Money += amount;
			sender.SendMessage($"[c/595959:»]  Pomyslnie dodano [c/66ff66:{amount}] {Economy.config.ValueName} do konta gracza [c/ff6666:{reciver.Name}].", Color.Gray);
			reciver.SendMessage($"[c/595959:»]  Administrator zmienil twoja ilosc srodkow na koncie.", Color.Gray);
			reciver.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {0} {1}", SurvivalCore.srvPlayers[reciver.Index].Money.ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
		}

		public static void takeMoney(TSPlayer sender, TSPlayer reciver, int amount)
		{
			int num = SurvivalCore.srvPlayers[reciver.Index].Money - amount;
			if (num < 0)
			{
				sender.SendErrorMessage("[c/595959:»]  Nie mozna bylo zmienic ilosci srodkow na koncie gracza {0}, poniewaz ilosc srodkow by byla mniejsza niz 0.", reciver.Name);
				return;
			}
			SurvivalCore.srvPlayers[reciver.Index].Money = num;
			sender.SendMessage(string.Format("[c/595959:»]  Pomyslnie usunieto [c/66ff66:{0}] {1} od konta gracza [c/ff6666:{2}].", amount.ToString("N0").Replace(' ', ','), Economy.config.ValueName, reciver.Name), Color.Gray);
			reciver.SendMessage($"[c/595959:»]  Administrator zmienil twoja ilosc srodkow na koncie.", Color.Gray);
			reciver.SendMessage(string.Format("[c/595959:»]  [c/66ff66:Twoj nowy stan konta][c/595959::] {0} {1}", num.ToString("N0").Replace(' ', ','), Economy.config.ValueName), Color.Gray);
		}
	}
}
