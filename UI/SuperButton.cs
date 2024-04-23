using System;
using System.Collections;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DailyMeals
{
    [RequireComponent(typeof(Button))]
    public class SuperButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
    {
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
        private void Awake()
        {
            _btnRectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
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
    }
}

