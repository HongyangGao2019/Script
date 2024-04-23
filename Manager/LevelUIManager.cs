using System.Collections;
using System.Globalization;
using Cinemachine;
using DG.Tweening;
using Drawing;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace DailyMeals
{
    public class LevelUIManager : MonoBehaviour
    {
        [Header("准备阶段的Canvas")]
        [SerializeField] private CanvasGroup prepareCanvasGroup;
        [SerializeField] private TMP_Text prepareTimeText;
        [SerializeField] private AudioClip countPrepareSfx;
        [SerializeField] private AudioClip lastPrepareSfx;

        [Header("游戏阶段的Canvas")] 
        [SerializeField] private CanvasGroup gameplayCanvasGroup;
        [SerializeField] private RectTransform encourageButtonRect;
        [SerializeField] private Vector3 startEncourageRectRotation;
        [SerializeField] private Vector3 endEncourageRectRotation;

        
        [Header("结算阶段的Canvas")]
        [SerializeField] private CanvasGroup balanceCanvasGroup;
        [SerializeField] private RectTransform replayButtonRect;
        [SerializeField] private Vector3 startReplayRectRotation;
        [SerializeField] private Vector3 endReplayRectRotation;
        private Vector3 startReplayRectPos;
        [SerializeField] private Vector3 endReplayRectPos;
        [Header("设置面板的Canvas")]
        [SerializeField] private CanvasGroup settingsCanvasGroup;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TMP_Text goldCoinCount;
        [SerializeField] private TMP_Text moneyCount;
        [SerializeField] private CanvasGroup rightPanelCanvasGroup;
        [SerializeField] private CanvasGroup downPanelCanvasGroup;
        [SerializeField] private CinemachineVirtualCamera opVCam;
        private Vector3 startTouchPos=new();
        private Vector3 endTouchPos=new();
        private Vector3 movingTouchPos = new();
        private Vector2 slideDir = new();
        private WaitForSeconds waitTime = new WaitForSeconds(1);
        private Tween replayButtonTween;
        private Tween encourageButtonTween;
        private Vector3 startReplayButtonRotation;
        private void Start()
        {
            StartCoroutine(PrepareStart());
        }

        
        private void Update()
        {
            SwipeScreen();
            
        }

        /// <summary>
        /// 划过屏幕，检测手势，对右边panel及左边panel做显隐
        /// </summary>
        private void SwipeScreen()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    //触摸开始
                    startTouchPos= Camera.main.ScreenToViewportPoint(touch.position);
                    using (Draw.WithColor(Color.red))
                    {
                        Draw.SolidBox(Camera.main.ScreenToWorldPoint(touch.position),new float3(4,4,4));
                    }
                    // Debug.Log($"startTouchPos:{startTouchPos}");
                }

                else if (touch.phase == TouchPhase.Moved)
                {
                    var draw = Draw.ingame;
                    // 触摸移动
                    movingTouchPos = Camera.main.ScreenToViewportPoint(touch.position);
                    // Debug.Log($"movingTouchPos:{movingTouchPos}");
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    endTouchPos= Camera.main.ScreenToViewportPoint(touch.position);
                    // 触摸结束
                    // Debug.Log($"endTouchPos:{endTouchPos}");
                    slideDir = new Vector2(endTouchPos.x - startTouchPos.x, endTouchPos.y - startTouchPos.y);
                    if (slideDir.magnitude > 0.1f)
                    {
                        // Debug.Log($"slideTouchDir:{slideDir}");
                        float x = Mathf.Atan(slideDir.y / slideDir.x);
                        if (Mathf.Abs(slideDir.y)<Mathf.Abs(slideDir.x))
                        {
                            //横滑
                            if (slideDir.x> 0)
                            {
                                //向右
                                rightPanelCanvasGroup.interactable = false;
                                rightPanelCanvasGroup.alpha = 0;
                                rightPanelCanvasGroup.blocksRaycasts = false;
                            }
                            else
                            {
                                //向左
                                rightPanelCanvasGroup.interactable = true;
                                rightPanelCanvasGroup.alpha = 1;
                                rightPanelCanvasGroup.blocksRaycasts = true;
                            }
                        }
                        else
                        {
                            //竖滑
                            if (slideDir.y > 0)
                            {
                                //向上
                                downPanelCanvasGroup.interactable = true;
                                downPanelCanvasGroup.alpha = 1;
                                downPanelCanvasGroup.blocksRaycasts = true;
                            }
                            else
                            {
                                //向下
                                downPanelCanvasGroup.interactable = false;
                                downPanelCanvasGroup.alpha = 0;
                                downPanelCanvasGroup.blocksRaycasts = false;
                            }
                        }
                    }
                }
            }
        }
        
        private void OnEnable()
        {
            GameManager.Singleton.OnGoldCoinCountChanged += RefreshGoldCoinUI;
            GameManager.Singleton.OnMoneyCountChanged += RefreshMoneyUI;
            GameManager.Singleton.EndLevel += ShowBalanceCanvas;
        }

        private void OnDisable()
        {
            GameManager.Singleton.OnGoldCoinCountChanged -= RefreshGoldCoinUI;
            GameManager.Singleton.OnMoneyCountChanged -= RefreshMoneyUI;
            GameManager.Singleton.EndLevel -= ShowBalanceCanvas;
        }

        private void RefreshGoldCoinUI(int _)
        {
            goldCoinCount.text = GameManager.Singleton.GoldCoin.ToString();
        }

        private void RefreshMoneyUI(float _)
        {
            moneyCount.text = GameManager.Singleton.Money.ToString(CultureInfo.CurrentCulture);
        }

        public void Home()
        {
            GameManager.Singleton.ForceEndLevelGame();
            SceneLoader.Singleton.LoadMenu();
        }

        public void OpenSettings()
        {
            AudioManager.Singleton.PlayMenuMusicByRandom();
            settingsCanvasGroup.alpha = 1;
            settingsCanvasGroup.interactable = true;
            settingsCanvasGroup.blocksRaycasts = true;
            musicVolumeSlider.value = SettingsManager.Singleton.MusicVolume;
            sfxVolumeSlider.value = SettingsManager.Singleton.SfxVolume;
        }
        
        public void CloseSettings()
        {
            AudioManager.Singleton.PlayInGameMusicByRandom();
            settingsCanvasGroup.alpha = 0;
            settingsCanvasGroup.interactable = false;
            settingsCanvasGroup.blocksRaycasts = false;
        }

        public void ShiftLanguage()
        {
            Debug.Log("切换语言 ...");
        }

        public void OpenClock()
        {
            Debug.Log("进入闹钟 ...");
        }

        public void OpenDiary()
        {
            Debug.Log("进入日记 ...");
        }

        public void OpenMall()
        {
            Debug.Log("进入商城 ...");
        }

        public void Encourage()
        {

        }
        public void ShiftOpVCam()
        {
            opVCam.Priority=(opVCam.Priority == 100) ? 10 : 100;
        }
        public void AddGoldCoin()
        {
            GameManager.Singleton.AddGoldCoin(1);
        }
        public void ModifySfxVolume(float value)
        {
            SettingsManager.Singleton.ChangeSfxVolume(value);
        } 
        public void ModifyMusicVolume(float value)
        {
            SettingsManager.Singleton.ChangeMusicVolume(value);
        }
        
        public void Replay()
        {
            HideBalanceCanvas();
            StartCoroutine(PrepareStart());
        }
        
        private IEnumerator PrepareStart()
        {
            CloseSettings();
            HideGameplayCanvas();
            HideBalanceCanvas();
            RefreshGoldCoinUI(0);
            ShowPrepareCanvas();
            prepareTimeText.text = "3";
            AudioManager.Singleton.StopAllMusic();
            AudioManager.Singleton.PlaySfx(countPrepareSfx);
            prepareTimeText.DOColor(prepareTimeText.color, 0.1f).onComplete+=
                ()=>prepareTimeText.DOColor(Color.red, .9f);
            prepareTimeText.DOScale(1.5f, 0.9f).onComplete +=
                ()=> prepareTimeText.DOScale(1, 0.1f);
            yield return waitTime;
            
            AudioManager.Singleton.PlaySfx(countPrepareSfx);
            prepareTimeText.text = "2";
            prepareTimeText.DOColor(Color.red, .1f).onComplete+=
                ()=>prepareTimeText.DOColor(new Color(1f,0.3f,0f), 0.9f);
            prepareTimeText.DOScale(1.5f, 0.9f).onComplete +=
                ()=> prepareTimeText.DOScale(1, 0.1f);
            yield return waitTime;
            
            AudioManager.Singleton.PlaySfx(countPrepareSfx);
            prepareTimeText.text = "1";
            prepareTimeText.DOColor(new Color(1f,0.3f,0f), .1f).onComplete+=
                ()=>prepareTimeText.DOColor(new Color(0.9f,0.9f,0f), .9f);
            prepareTimeText.DOScale(1.5f, 0.9f).onComplete +=
                ()=> prepareTimeText.DOScale(1, 0.1f);
            yield return waitTime;
            GameManager.Singleton.InitLevelGame();
            AudioManager.Singleton.PlaySfx(lastPrepareSfx);
            prepareTimeText.text = "Ready Go!";
            prepareTimeText.DOColor(new Color(0.9f,0.9f,0f),.1f).onComplete+=
                ()=>prepareTimeText.DOColor(new Color(0.2f,0.9f,0.3f), .9f);
            prepareTimeText.DOScale(1.3f, 0.9f).onComplete +=
                ()=> prepareTimeText.DOScale(1, 0.1f);
            yield return waitTime;
            
            StartCurrentGame();
        }
        
        private void StartCurrentGame()
        {
            ShowGameplayCanvas();
            HidePrepareCanvas();
            AudioManager.Singleton.PlayInGameMusicByRandom();
            GameManager.Singleton.StartLevelGame();
        }

        private void HidePrepareCanvas()
        {
            prepareCanvasGroup.interactable = false;
            prepareCanvasGroup.alpha = 0;
            prepareCanvasGroup.blocksRaycasts = false;
        }
        private void ShowPrepareCanvas()
        {
            prepareCanvasGroup.interactable = true;
            prepareCanvasGroup.alpha = 1;
            prepareCanvasGroup.blocksRaycasts = true;
        }
        private void HideGameplayCanvas()
        {
            if(encourageButtonTween!=null)encourageButtonTween.Kill();
            encourageButtonRect.rotation = Quaternion.Euler(startEncourageRectRotation);
            gameplayCanvasGroup.interactable = false;
            gameplayCanvasGroup.alpha = 0;
            gameplayCanvasGroup.blocksRaycasts = false;
        }
        private void ShowGameplayCanvas()
        {
            encourageButtonTween = encourageButtonRect.DORotate(endEncourageRectRotation, 0.5f).SetLoops(-1,LoopType.Yoyo);
            gameplayCanvasGroup.interactable = true;
            gameplayCanvasGroup.alpha = 1;
            gameplayCanvasGroup.blocksRaycasts = true;
        }
        private void ShowBalanceCanvas()
        {
            replayButtonRect.rotation = Quaternion.Euler(startReplayRectRotation);
            replayButtonTween=replayButtonRect.DORotate(endReplayRectRotation, 0.8f).SetLoops(-1, LoopType.Yoyo);
            replayButtonRect.position =new Vector3(Screen.width/2f,Screen.height+500f,0);
            endReplayRectPos.x = Screen.width / 2f;
            replayButtonRect.DOMove(endReplayRectPos,0.8f).SetEase(Ease.OutBounce,3f);
            balanceCanvasGroup.interactable = true;
            balanceCanvasGroup.blocksRaycasts = true;
            balanceCanvasGroup.alpha = 1;
        }
        private void HideBalanceCanvas()
        {
            if (replayButtonTween != null)
            {
                replayButtonTween.Kill();
            }
            balanceCanvasGroup.interactable = false;
            balanceCanvasGroup.blocksRaycasts = false;
            balanceCanvasGroup.alpha = 0;
        }


    }
}

