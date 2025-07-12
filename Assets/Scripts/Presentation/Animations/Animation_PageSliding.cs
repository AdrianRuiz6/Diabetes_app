// code from https://pressstart.vip/tutorials/2019/06/1/96/level-selector.html

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Master.Presentation.Animations
{
    public class Animation_PageSliding : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        private Vector2 panelLocation;
        [SerializeField] private float percentThreshold = 0.2f;
        [SerializeField] private float dragSensitivity = 1f;
        [SerializeField] private float easing = 0.2f;
        [SerializeField] private int totalPages = 5;
        [SerializeField] private int initialPage = 3;
        private int currentPage = 1;
        private bool _isWorking = true;

        private RectTransform _rectTransform;

        public static Animation_PageSliding Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            currentPage = initialPage;
            panelLocation = _rectTransform.anchoredPosition;
        }
        public void OnDrag(PointerEventData data)
        {
            if (!_isWorking)
                return;

            float difference = (data.pressPosition.x - data.position.x) * dragSensitivity;
            _rectTransform.anchoredPosition = panelLocation - new Vector2(difference, 0);
        }
        public void OnEndDrag(PointerEventData data)
        {
            if (!_isWorking)
                return;

            float width = _rectTransform.rect.width;
            float percentage = (data.pressPosition.x - data.position.x) / width;

            if (Mathf.Abs(percentage) >= percentThreshold)
            {
                Vector2 newLocation = panelLocation;
                if (percentage > 0 && currentPage < totalPages)
                {
                    currentPage++;
                    newLocation += new Vector2(-width, 0);
                }
                else if (percentage < 0 && currentPage > 1)
                {
                    currentPage--;
                    newLocation += new Vector2(width, 0);
                }
                StartCoroutine(SmoothMove(_rectTransform.anchoredPosition, newLocation, easing));
                panelLocation = newLocation;
            }
            else
            {
                StartCoroutine(SmoothMove(_rectTransform.anchoredPosition, panelLocation, easing));
            }
        }
        IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
        {
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                _rectTransform.anchoredPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        public void MoveToInitialPage()
        {
            int pageDifference = initialPage - currentPage;
            float width = _rectTransform.rect.width;

            Vector2 newLocation = panelLocation;
            currentPage = initialPage;
            newLocation += new Vector2(-width * pageDifference, 0);

            StartCoroutine(SmoothMove(transform.position, newLocation, easing));

            panelLocation = newLocation;
        }

        public void ActivatePageSliding()
        {
            _isWorking = true;
        }

        public void DeactivatePageSliding()
        {
            _isWorking = false;
        }
    }
}