using System.Collections.Generic;
using UnityEngine;

namespace DailyMeals
{
    [CreateAssetMenu(menuName = "SO/FoodMenu",fileName = "FoodMenu")]
    public class FoodMenu:ScriptableObject,IFoodMenu
    {
        [SerializeField] private string _foodMenuName;
        [SerializeField] private List<Food> _foodList;
        public string FoodMenuName { get=>_foodMenuName; }
        public List<Food> FoodList { get=>_foodList; }
    }
}