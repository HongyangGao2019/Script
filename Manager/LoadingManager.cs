using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace DailyMeals
{
    
    public class LoadingManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup loadingCanvasGroup;
        [SerializeField] private Slider loadingSlider;
        [Header("进度定位图标动画")]
        [SerializeField] private RectTransform progressLocationIconRect;
        // [SerializeField] private Image iconImage;
        [SerializeField] private Vector3 startRotateAngle;
        [SerializeField] private Vector3 endRotateAngle;
        
        public event UnityAction StartLoading;
        public event UnityAction EndLoading;
        
        private AsyncOperation _loadingSceneOperation;
        private int _loadingProgress=0;
        public static LoadingManager Singleton { get; private set; }


        private void Awake()
        {
            if (Singleton == null)
            {
                DontDestroyOnLoad(this.gameObject);
                Singleton = this;
            }
        }

        private void Start()
        {
            InitLoadingCanvas();
        }

        private void OnEnable()
        {
            StartLoading +=ActiveLoadingCanvas;
            EndLoading += InactiveLoadingCanvas;
        }

        private void OnDisable()
        {
            StartLoading -=ActiveLoadingCanvas;
            EndLoading -= InactiveLoadingCanvas;
            
        }

        private Tweener _rotateTweener;
        // private Tweener _colorTweener;
        // private Color _endColor = new Color(0.5f, 0.5f, 0.5f);
        public IEnumerator DoLoading(int sceneIndex)
        {
            StartLoading?.Invoke();
            _loadingProgress = 0;
            _loadingSceneOperation= SceneManager.LoadSceneAsync(sceneIndex);
            _loadingSceneOperation.completed += CompleteLoading;
            _rotateTweener=progressLocationIconRect.DORotate(endRotateAngle, 0.2f).SetLoops(-1,LoopType.Yoyo);
            // _colorTweener=iconImage.DOColor(_endColor, 0.2f).SetLoops(-1, LoopType.Yoyo);
            
            while (!_loadingSceneOperation.isDone||_loadingProgress<100)
            {
                //如果完成了加载，就应该加快速度，这里是3
                if (_loadingSceneOperation.isDone)
                {
                    _loadingProgress += 2;
                }
                else
                {
                    ++_loadingProgress;
                }
                loadingSlider.value = _loadingProgress;
                yield return new WaitForEndOfFrame();
            }
            _rotateTweener.Kill();
            // _colorTweener.Kill();
            progressLocationIconRect.rotation = Quaternion.Euler(startRotateAngle);
            EndLoading?.Invoke();
        }

        private void CompleteLoading(AsyncOperation asyncOperation)
        {
            Debug.Log("Scene loading is finished");
            _loadingSceneOperation.completed -= CompleteLoading;
        }
        private void InitLoadingCanvas()
        {
            InactiveLoadingCanvas();
            progressLocationIconRect.rotation = Quaternion.Euler(startRotateAngle);
        }
        private void ActiveLoadingCanvas()
        {
            Debug.Log("start loading");
            loadingSlider.value = 0f;
            loadingCanvasGroup.alpha = 1f;
            loadingCanvasGroup.interactable = true;
            loadingCanvasGroup.blocksRaycasts = true;
        }
        private void InactiveLoadingCanvas()
        {
            Debug.Log("End loading");
            loadingSlider.value = 100f;
            loadingCanvasGroup.alpha = 0f;
            loadingCanvasGroup.interactable = false;
            loadingCanvasGroup.blocksRaycasts = false;
        }

    }
}

