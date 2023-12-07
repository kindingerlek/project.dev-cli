using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using NaughtyAttributes;

namespace Tools.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class ResizableUITK : MonoBehaviour
    {
        const ResizablePoints ALL_RESIZABLE_POINTS = (ResizablePoints) (1 << 8) - 1;

        [System.Flags]
        private enum ResizablePoints
        {
            None        = 0,
            TopLeft     = 1 << 0,
            Top         = 1 << 1,
            TopRight    = 1 << 2,

            Left        = 1 << 3,
            Right       = 1 << 4,

            BottomLeft  = 1 << 5,
            Bottom      = 1 << 6,
            BottomRight = 1 << 7
        }

        readonly Dictionary<ResizablePoints, Vector2> pointsToVector = new()
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

        readonly Dictionary<ResizablePoints, Vector2> growDirection = new()
        {
            {ResizablePoints.TopLeft,       new Vector2(1.0f, 1.0f)},
            {ResizablePoints.Top,           new Vector2(0.0f, 1.0f)},
            {ResizablePoints.TopRight,      new Vector2(1.0f, 1.0f)},
            {ResizablePoints.Left,          new Vector2(1.0f, 0.0f)},
            {ResizablePoints.Right,         new Vector2(1.0f, 0.0f)},
            {ResizablePoints.BottomLeft,    new Vector2(1.0f, 1.0f)},
            {ResizablePoints.Bottom,        new Vector2(0.0f, 1.0f)},
            {ResizablePoints.BottomRight,   new Vector2(1.0f, 1.0f)}
        };



        [SerializeField] private string targetId = "window";

        [Header("Resizable Points")]
        [SerializeField, EnumFlags] private ResizablePoints resizablePoints = ALL_RESIZABLE_POINTS;
        [SerializeField] private Vector2 sensitiveArea = new(7, 7);

        [Header("Constraints")]
        [SerializeField] private bool enableMinSize = true;
        [SerializeField] private bool enableMaxSize = true;
        [SerializeField, EnableIf("enableMinSize")] private Vector2 minSize = new(420, 400);
        [SerializeField, EnableIf("enableMaxSize")] private Vector2 maxSize = new(1100, 800);

        private ResizablePoints lastUsedPoint;

        private Vector2 previousPointerPosition;
        private Vector2 relativePointerPosition;
        private Vector2 previousContentSize;

        private UIDocument document;
        private VisualElement target;

        private bool isMouseDown;


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

            if (enableMinSize) target.style.minWidth = minSize.x;
            if (enableMaxSize) target.style.maxWidth = maxSize.x;

            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        public void OnPointerUp(PointerUpEvent data)
        {
            isMouseDown = false;
        }

        public void OnPointerDown(PointerDownEvent data)
        {
            isMouseDown = true;

            var mousePosition = Input.mousePosition;

            previousContentSize = target.contentRect.size;
            relativePointerPosition = data.localPosition / previousContentSize;
            previousPointerPosition = new Vector2(mousePosition.x, Screen.height - mousePosition.y);

            lastUsedPoint = GetResizePoint();
        }

        public void OnPointerMove(PointerMoveEvent data)
        {
            var resizePoint = GetResizePoint();

            if(resizePoint == ResizablePoints.None)
                return;

            //TODO: Add cursor change on hover
        }



        public void Update()
        {
            if (!isMouseDown || lastUsedPoint == ResizablePoints.None)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                isMouseDown = false;
                return;
            }

            Resize();
        }

        private void Resize()
        {
            Vector2 currentMouse = new(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            Vector2 mouseDiff = (currentMouse - previousPointerPosition) / UiHelper.GetScale(document.panelSettings);
            Vector2 diff = mouseDiff * growDirection[lastUsedPoint];

            var sizeDelta = previousContentSize + diff;

            target.style.width = sizeDelta.x;
            target.style.height = sizeDelta.y;
        }

        private ResizablePoints GetResizePoint()
        {
            foreach (var pVect in pointsToVector)
            {
                if ((resizablePoints & pVect.Key) == 0)
                    continue;


                Vector2 diff = (relativePointerPosition - pVect.Value) * target.contentRect.size;

                // Make sure the diff is always positive
                diff = new Vector2(Mathf.Abs(diff.x), Mathf.Abs(diff.y));                

                if (diff.x > 0 && diff.y > 0 && diff.x <= sensitiveArea.x && diff.y <= sensitiveArea.y)
                    return pVect.Key;
            }
            return ResizablePoints.None;
        }

    }
}
