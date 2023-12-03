using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Tools.EditorExtensions;
using UnityEngine.UIElements;

namespace Tools.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class ResizableUITK : MonoBehaviour
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

        Dictionary<ResizablePoints, Vector2> pointsToVector = new()
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

        Dictionary<ResizablePoints, Vector2> growDirection = new()
        {
            {ResizablePoints.TopLeft | ResizablePoints.BottomRight, new Vector2( 1.0f, 1.0f)},
            {ResizablePoints.Left    | ResizablePoints.Right,       new Vector2( 1.0f, 0.0f)},
            {ResizablePoints.Top     | ResizablePoints.Bottom,      new Vector2( 0.0f, 1.0f)},

        };

        [SerializeField] private string targetId = "window";

        [Header("Resizable Points")]
        [SerializeField, EnumMask] private ResizablePoints resizablePoints;
        [SerializeField] private Vector2 sensitiveArea = new(7, 7);
        [SerializeField] private Vector2 minSize = new(420, 400);
        [SerializeField] private Vector2 maxSize = new(1100, 800);

        private ResizablePoints lastUsedPoint;

        private Vector2 currentPointerPosition;
        private Vector2 previousPointerPosition;
        private Vector2 relativePointerPosition;

        private UIDocument document;
        private VisualElement target;

        private bool shouldResize;
        private bool isMouseDown;

        public Vector2 MaxSize
        {
            get => maxSize;
            set
            {
                maxSize = value;

                target.style.maxWidth = maxSize.x;
                target.style.maxHeight = maxSize.y;
            }
        }

        public Vector2 MinSize
        {
            get => minSize;
            set
            {
                minSize = value;

                target.style.minWidth = minSize.x;
                target.style.minHeight = minSize.y;
            }
        }

        void Awake()
        {

            if (!TryGetComponent(out document))
            {
                Debug.LogError("UIDocument not found");
                return;
            }

            target = document.rootVisualElement.Q<VisualElement>(targetId);
            if (target == null)
            {
                Debug.LogError("Target not found on provided UIDocument. Please check if the targetId is correct");
                return;
            }

            MinSize = new Vector2(target.style.minWidth.value.value, target.style.minHeight.value.value);
            MaxSize = new Vector2(target.style.maxWidth.value.value, target.style.maxHeight.value.value);


            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        public void OnPointerUp(PointerUpEvent data)
        {
            isMouseDown = false;
        }

        public void OnPointerDown(PointerDownEvent data)
        {
            isMouseDown = true;

            var rectTransform = target.contentContainer.contentRect;

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, Camera.current, out previousPointerPosition);
            //relativePointerPosition = (previousPointerPosition * new Vector2(1, -1)) / rectTransform.sizeDelta;
            
            shouldResize = IsValidResizablePoint();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void Update()
        {
            if (!shouldResize || !isMouseDown)
                return;


            //Vector2 sizeDelta = target.contentRect.size;

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.current, out currentPointerPosition);
            //Vector2 resizeValue = currentPointerPosition - previousPointerPosition;

            //sizeDelta += new Vector2(resizeValue.x, -resizeValue.y);


            //sizeDelta = new Vector2(
            //    Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
            //    Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
            //    );


            //Debug.Log(
            //        $"sizeDelta: {sizeDelta}\n" +
            //        $"resizeValue: {resizeValue}\n" +
            //        $"rectTransform.sizeDelta: {rectTransform.sizeDelta}\n" +
            //        "");

            //rectTransform.sizeDelta = sizeDelta;

            //previousPointerPosition = currentPointerPosition;
        }

        public bool IsValidResizablePoint()
        {
            Debug.Log("IsValidResizablePoint " + resizablePoints);
            foreach (var pVect in pointsToVector)
            {
                if ((resizablePoints & pVect.Key) == 0)
                    continue;

                Vector2 diff = (pVect.Value - relativePointerPosition) * target.contentRect.size;

                if (diff.x > 0 && diff.y > 0 && diff.x <= sensitiveArea.x && diff.y <= sensitiveArea.y)
                    return true;
            }
            return false;
        }

    }
}
