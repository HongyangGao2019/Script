using UnityEngine;
using UnityEngine.UI;

namespace DailyMeals
{
    [CreateAssetMenu(menuName = "SO/Food",fileName = "Food")]
    public class Food:ScriptableObject,IFood
    {
        [SerializeField] private Sprite _foodImage;
        [SerializeField] private string _foodName = "Default Name";
        [SerializeField] private float _price = 10f;
        [SerializeField] private float _nutrition = 10f;
        [SerializeField] private float _eatDuration = 1f;
        
        public Sprite FoodImage { get=>_foodImage; }
        public string FoodName { get => _foodName; }
        public float Price { get=>_price; }
        public float Nutrition { get=>_nutrition; }
        public float EatDuration { get=>_eatDuration; }
    }
}