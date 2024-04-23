using System.Collections;
using System.Collections.Generic;
using DailyMeals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DailyMeals
{
    public class SuperSlider : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
    {
        [SerializeField] private AudioClip pointerUpSfx;
        [SerializeField] private AudioClip pointerDownSfx;
    

        public void OnPointerUp(PointerEventData eventData)
        {
            if (pointerUpSfx != null)
            {
                AudioManager.Singleton.PlaySfx(pointerUpSfx);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (pointerDownSfx != null)
            {
                AudioManager.Singleton.PlaySfx(pointerDownSfx);
            }
        }
    }

}
