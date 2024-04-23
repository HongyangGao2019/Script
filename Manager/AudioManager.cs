using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace DailyMeals
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> menuMusicList;
        [SerializeField] private List<AudioClip> inGameMusicList;
        [Header("结算音乐")]
        [SerializeField] private AudioClip loseMusicClip;
        [SerializeField] private AudioClip winMusicClip;
        [Header("组件及设置")]
        [SerializeField] private AudioSource musicAudioSource;

        [SerializeField] private AudioSource sfxPlayerPrefab;

        [SerializeField] private int maxPoolSize = 10;
        private IObjectPool<AudioSource> sfxPlayerPool;

        public IObjectPool<AudioSource> SfxPlayerPool
        {
            get
            { 
                return sfxPlayerPool;
            }
        }


        public static AudioManager Singleton { get; private set; }
        public event UnityAction PlayMenuMusic;//播放音乐
        public event UnityAction PlayIngameMusic;//播放音乐
        public event UnityAction StopMusic;//停止音乐
        public event UnityAction ResumeMusic;//继续音乐
        public event UnityAction FinishMusic;//结束音乐

        private int _currentMainMusicIndex;
        private int _currentInGameMusicIndex;


        private void Awake()
        { 
            DontDestroyOnLoad(gameObject);
            Singleton = this;
            //初始Sfx Player Pool
            sfxPlayerPool = new ObjectPool<AudioSource>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, default, default, maxPoolSize);
        }


        private void Start()
        {
            SettingsManager.Singleton.OnMusicVolumeChanged += ChangeMusicAudioSource;
            // PlayMenuMusicByRandom();
        }

        private void OnDestroy()
        {
            SettingsManager.Singleton.OnMusicVolumeChanged -= ChangeMusicAudioSource;

        }

        public void PlayMenuMusicByRandom()
        {
            _currentMainMusicIndex=Random.Range(0, menuMusicList.Count - 1);
             musicAudioSource.clip = menuMusicList[_currentMainMusicIndex];
             musicAudioSource.Play();
             PlayMenuMusic?.Invoke();
        }

        public void PlayMainMusic(int index)
        {
            musicAudioSource.clip = menuMusicList[index];
            musicAudioSource.Play();
            PlayMenuMusic?.Invoke();
        }


        public void PlayInGameMusicByRandom()
        {
            _currentInGameMusicIndex=Random.Range(0, inGameMusicList.Count - 1);
            musicAudioSource.clip = inGameMusicList[_currentInGameMusicIndex];
            musicAudioSource.Play();
            PlayIngameMusic?.Invoke();
        }
        public void PlayInGameMusic(int index)
        {
            musicAudioSource.clip = inGameMusicList[index];
            musicAudioSource.Play();
            PlayIngameMusic?.Invoke();
        }
        public void PlayWinGameMusic()
        {
            musicAudioSource.clip = winMusicClip;
            musicAudioSource.Play();
        }
        public void PlayLoseGameMusic()
        {
            musicAudioSource.clip = loseMusicClip;
            musicAudioSource.Play();
        }
        
        public void StopAllMusic()
        {
            // musicAudioSource.DOPause();
            musicAudioSource.Pause();
        }
        public void ResumeAllMusic()
        {
            musicAudioSource.UnPause();
        }
        public void PlaySfx(AudioClip audioClip,Vector3 playPos=new(),float volume=1)
        {
            AudioSource sfxPlayer=sfxPlayerPool.Get();
            sfxPlayer.clip = audioClip;
            sfxPlayer.transform.position = playPos;
            sfxPlayer.volume = volume*SettingsManager.Singleton.SfxVolume;
            StartCoroutine(OnAudioSourcePlay(sfxPlayer));
        }
        
        private void ChangeMusicAudioSource(float value)
        {
            musicAudioSource.volume = value;
        }
        
        private AudioSource CreatePooledItem()
        {
            AudioSource sfxPlayer=Instantiate(sfxPlayerPrefab, this.transform);
            sfxPlayer.name= "Pooled Sfx Player";
            sfxPlayer.Stop();
            sfxPlayer.loop = false;
            return sfxPlayer;
        }

        private void OnTakeFromPool(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(AudioSource audioSource)
        {
            audioSource.gameObject.SetActive(false);
        }
        private void OnDestroyPoolObject(AudioSource audioSource)
        {
            Destroy(audioSource.gameObject);
        }
        private IEnumerator OnAudioSourcePlay(AudioSource sfxPlayer)
        {
            sfxPlayer.Play();
            yield return new WaitForSeconds(sfxPlayer.clip.length);
            sfxPlayerPool.Release(sfxPlayer);
        }
    }
}
