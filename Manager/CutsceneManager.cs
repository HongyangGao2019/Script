using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DailyMeals
{
    public class CutsceneManager : MonoBehaviour
    {
        [SerializeField] private TimelineAsset currentSceneTimelineAsset;
        
        [SerializeField] private PlayableDirector director;
        public static CutsceneManager Singleton { get; private set; }
        public event UnityAction CutscenePlay;
        public event UnityAction CutsceneFinish;
        
        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            PlayCurrentWelcome();
            CutscenePlay += PlayCutscene;
            CutsceneFinish += FinishCutscene;
        }

        private void OnDestroy()
        {
            CutscenePlay -= PlayCutscene;
            CutsceneFinish -= FinishCutscene;
        }

        public void PlayTimeline(TimelineAsset asset)
        {
            CutscenePlay?.Invoke();
            StartCoroutine(UntilFinishedCutscene(asset));
        }
        private void PlayCurrentWelcome()
        {
            PlayTimeline(currentSceneTimelineAsset);
        }
        

        private IEnumerator UntilFinishedCutscene(TimelineAsset timelineAsset)
        {
            director.Play(timelineAsset);
            yield return new WaitForSeconds((float)timelineAsset.duration);
            CutsceneFinish?.Invoke();
        }

        private void PlayCutscene()
        {
            Debug.Log("开始播放Cutscene");
        }
        private void FinishCutscene()
        {
            Debug.Log("结束播放Cutscene");
        }
    }
}
