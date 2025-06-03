using DG.Tweening;
using UnityEngine;

namespace Master.Presentation.Animations
{
    public class Animation_Tutorial : MonoBehaviour
    {
        private Vector3 _minScale;
        private Vector3 _maxScale;
        private float _animationDuration;

        private Tween _tutorialTween;

        private Vector3 _startingScale;

        void Awake()
        {
            _startingScale = transform.localScale;

            _minScale = _startingScale * 1f;
            _maxScale = _startingScale * 1.1f;
            _animationDuration = 0.5f;
            transform.localScale = _minScale;
        }

        void OnEnable()
        {
            transform.localScale = _minScale;

            _tutorialTween = transform
                .DOScale(_maxScale, _animationDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        void OnDisable()
        {
            if (_tutorialTween != null)
                _tutorialTween.Kill();
        }
    }
}