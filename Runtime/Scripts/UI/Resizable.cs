using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Tools.EditorExtensions;

namespace Tools.UI
{
    public class Resizable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        [System.Flags]
        private enum ResizablePoints
        {
            TopLeft = 1,
            Top = 2,
            TopRight = 4,

            Left = 8,
            Right = 16,

            BottomLeft = 32,
            Bottom = 64,
            BottomRight = 128,
        }

        Dictionary<ResizablePoints, Vector2> pointsToVector = new Dictionary<ResizablePoints, Vector2>()
        {
            {ResizablePoints.TopLeft,       new Vector2( 0.0f, 0.0f)},
            {ResizablePoints.Top,           new Vector2( 0.5f, 0.0f)},
            {ResizablePoints.TopRight,      new Vector2( 1.0f, 0.0f)},
            {ResizablePoints.Left,          new Vector2( 0.0f, 0.5f)},
            {ResizablePoints.Right,         new Vector2( 1.0f, 0.5f)},
            {ResizablePoints.BottomLeft,    new Vector2( 0.0f, 1.0f)},
            {ResizablePoints.Bottom,        new Vector2( 0.5f, 1.0f)},
            {ResizablePoints.BottomRight,   new Vector2( 1.0f, 1.0f)}
        };

        Dictionary<ResizablePoints, Vector2> growDirection = new Dictionary<ResizablePoints, Vector2>()
        {
            {ResizablePoints.TopLeft
                | ResizablePoints.BottomRight,       new Vector2( 1.0f, 1.0f)},

            { ResizablePoints.Left
                | ResizablePoints.Right,           new Vector2( 1.0f, 0.0f)},

            { ResizablePoints.Top
                | ResizablePoints.Bottom,       new Vector2(0.0f, 1.0f) },

        };

        [SerializeField, EnumMask] private ResizablePoints resizablePoints;
        [SerializeField] private Vector2 sensitiveArea = new Vector2(7, 7);
        [SerializeField] private Vector2 minSize;
        [SerializeField] private Vector2 maxSize;

        private ResizablePoints lastUsedPoint;
        private Vector2 currentPointerPosition;
        private Vector2 previousPointerPosition;
        private RectTransform rectTransform;
        private Vector2 relativePointerPosition;
        private bool shouldResize;
        private bool isMouseDown;

        void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
        }

        public void OnPointerUp(PointerEventData data)
        {
            isMouseDown = false;
        }

        public void OnPointerDown(PointerEventData data)
        {
            isMouseDown = true;

            rectTransform.SetAsLastSibling();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out previousPointerPosition);
            relativePointerPosition = (previousPointerPosition * new Vector2(1, -1)) / rectTransform.sizeDelta;
            shouldResize = isValidResizablePoint();
        }

        public void Update()
        {
            if (rectTransform == null || !shouldResize || !isMouseDown)
                return;

            Vector2 sizeDelta = rectTransform.sizeDelta;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.current, out currentPointerPosition);
            Vector2 resizeValue = currentPointerPosition - previousPointerPosition;

            sizeDelta += new Vector2(resizeValue.x, -resizeValue.y);


            sizeDelta = new Vector2(
                Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
                Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
                );

            rectTransform.sizeDelta = sizeDelta;

            previousPointerPosition = currentPointerPosition;
        }

        public bool isValidResizablePoint()
        {
            Vector2 diff = sensitiveArea * 2;

            foreach (var pVect in pointsToVector)
            {
                if ((resizablePoints & pVect.Key) == 0)
                    continue;

                diff = (pVect.Value - relativePointerPosition) * rectTransform.sizeDelta;

                if (diff.x > 0 && diff.y > 0 && diff.x <= sensitiveArea.x && diff.y <= sensitiveArea.y)
                    return true;
            }
            return false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }
    } 
}
