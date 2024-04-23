using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyMeals
{
    public class Seat : MonoBehaviour
    {
        [SerializeField] private Transform _sitTransform;
        [SerializeField] private Transform _afterSitTransform;
        private Quaternion _afterSitOrientation;
        
        public void Sit(Transform seatHolder,out Seat seat,ref Quaternion seatHolderOrientation)
        {
            GameManager.Singleton.UnregisterFreeSeat(this);
            GameManager.Singleton.BowlShows[this].ShowFull();
            seatHolder.position = _sitTransform.position;
            seatHolder.rotation = _sitTransform.rotation;
            _afterSitOrientation = seatHolderOrientation;
            seat = this;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seatHolder">占位者</param>
        public void Leave(Transform seatHolder)
        {
            GameManager.Singleton.BowlShows[this].ShowEmpty();
            Transform holder = seatHolder.transform;
            holder.position = _afterSitTransform.position;
            holder.rotation = _afterSitOrientation;
            GameManager.Singleton.RegisterFreeSeat(this);
        }
    }
}
