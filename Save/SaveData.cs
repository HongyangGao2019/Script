using System;

namespace DailyMeals
{

    [Serializable]
    public class SaveData
    {
        public int goldCoin;
        public float money;

        public SaveData(int goldCoin, float money)
        {
            this.goldCoin = goldCoin;
            this.money = money;
        }
    }
}
