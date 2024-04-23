using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DailyMeals
{
    [RequireComponent(typeof(ScrollRect))]
    public class SuperScrollRect : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
    {
        public static event Action<bool> ControlScrollRectHorizontal;
        enum PlayAnimStatus
        {
            Default,
            PlayEntering,
            PlayExiting,
        }
        [Header("Sfx")]
        [SerializeField] private AudioClip clickedSfx=default;
        [SerializeField] private AudioClip pointerEnterSfx=default;
        [Header("Animation")]
        [SerializeField] private float endScaleMulti = 0.15f;
        [SerializeField] private float animEnterDuration=0.8f;
        [SerializeField] private float animExitDuration=0.1f;
        private RectTransform _btnRectTransform;
        private bool _pointerEnterFlag = false;
        private PlayAnimStatus _playAnimStatus = PlayAnimStatus.Default;
        private Tweener _tweener;
        private ScrollRect _scrollRect;
        public static void ControlScrollRectScroll(bool value)
        {
            ControlScrollRectHorizontal?.Invoke(value);
        }
        private void Awake()
        {
            _btnRectTransform = GetComponent<RectTransform>();
            _scrollRect = GetComponent<ScrollRect>();

        }

        private void OnEnable()
        {
            ControlScrollRectHorizontal += ControlHorizontalScroll;
        }

        private void OnDisable()
        {
            ControlScrollRectHorizontal -= ControlHorizontalScroll;
        }

        private void Update()
        {
            if (Input.touchCount == 1)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {

                }
                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                }
                // if (Input.touches[0].phase == TouchPhase.Ended)
                // {
                //     _scrollRect.
                // }

            }
            if(_pointerEnterFlag)return;
            if (_playAnimStatus != PlayAnimStatus.Default) return;
            if (_btnRectTransform.localScale.normalized != Vector3.one)
            {
                _btnRectTransform.localScale=Vector3.one;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_pointerEnterFlag)
            {
                _pointerEnterFlag = true;
                _playAnimStatus = PlayAnimStatus.PlayEntering;
                StartCoroutine(EnterAnimation(Vector3.one *(1f+endScaleMulti),animEnterDuration));
                AudioManager.Singleton.PlaySfx(pointerEnterSfx);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_pointerEnterFlag)
            {
                _pointerEnterFlag = false;
                _playAnimStatus = PlayAnimStatus.PlayExiting;
                // _btnRectTransform.localScale=Vector3.one;
                StartCoroutine(ExitAnimation(Vector3.one ,animExitDuration));
                //Bug:如果在当前协程结束以前进入鼠标触发OnPointerEnter()会怎样？
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Singleton.PlaySfx(clickedSfx);
            _btnRectTransform.localScale = Vector3.one;
        }

        private IEnumerator ExitAnimation(Vector3 endScale,float duration)
        {
            _tweener=_btnRectTransform.DOScale(endScale,duration);
            yield return new WaitForSeconds(duration);
            _playAnimStatus = PlayAnimStatus.Default;
        }
        private IEnumerator EnterAnimation(Vector3 endScale,float duration)
        {
            _tweener=_btnRectTransform.DOScale(endScale,duration);
            yield return new WaitForSeconds(duration);
            _playAnimStatus = PlayAnimStatus.Default;
        }

        private void ControlHorizontalScroll(bool value)
        {
            _scrollRect.horizontal = value;
        }
        private void ControlVerticalScroll(bool value)
        {
            _scrollRect.vertical = value;
        }


    }
}

