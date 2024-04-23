using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyMeals
{
    public class BowlShow : MonoBehaviour
    {
        [SerializeField] private GameObject _fullShow;
        [SerializeField] private GameObject _emptyShow;

        public void ShowFull()
        {
            _fullShow.SetActive(true);
            _emptyShow.SetActive(false);
        }

        public void ShowEmpty()
        {
            _fullShow.SetActive(false);
            _emptyShow.SetActive(true);
        }
    }
}
