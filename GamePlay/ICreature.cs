using System.Collections.Generic;
using UnityEngine;

namespace DailyMeals
{
    public interface IRenew
    {
        public void Renew();
    }
    public interface ICustomer
    {
        float Bill { get; }
        /// <summary>
        /// 点单
        /// </summary>
        /// <param name="foodMenu">菜单种类</param>
        /// <param name="orderIndexes">点单菜品序号</param>
        /// <returns>是否点单成功</returns>
        void OrderMeals(FoodMenu foodMenu,params int[] orderIndexes);
        void OrderMeal(Food food);
        
        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="boss"></param>
        /// <param name="payMoney"></param>
        void CheckOut(Boss boss);
    }
    
    public interface IBoss
    {
        /// <summary>
        /// Boss记账
        /// </summary>
        /// <param name="customer">顾客</param>
        /// <param name="bill">账单</param>
        void CheckCharge(Customer customer,float bill);
    }

    public interface IFoodMenu
    {
        public string FoodMenuName { get; }
        public List<Food> FoodList { get; }
    }

    public interface IFood
    {
        Sprite FoodImage { get; }
        string FoodName { get; }
        float Price { get; }
        float Nutrition { get; }
        float EatDuration { get; }
    }

    public interface IWallet
    {
        public float Cash { get; }//检查现金余额
        public float Card { get; }//检查银行卡余额
    }
    
    public interface IState
    {
        void OnEnter();
        void OnUpdate();
        void OnFixedUpdate();
        void OnExit();
    }
}