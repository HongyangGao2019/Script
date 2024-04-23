using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DailyMeals
{
    public class MainMenuUI : MonoBehaviour
    {
        public void StartGame()
        {
            SceneLoader.Singleton.LoadLevel(1);
        }
        
        public void EndGame()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
#else
            Application.Quit();   
#endif
        }
    }
}

