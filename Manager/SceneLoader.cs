using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DailyMeals
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Singleton { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Singleton = this;
        }

        private void Start()
        {
            SceneManager.LoadSceneAsync(1);
            AudioManager.Singleton.PlayMenuMusicByRandom();
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadingManager.Singleton.DoLoading(1));
            AudioManager.Singleton.PlayMenuMusicByRandom();
        }

        /// <summary>
        /// 加载关卡
        /// </summary>
        /// <param name="levelIndex">使用自己Level scene中的index，而不是Build Level中的index（直接使用）</param>
        public void LoadLevel(int levelIndex)
        {
            StartCoroutine(LoadingManager.Singleton.DoLoading(levelIndex + 1));
        }
    }

}
