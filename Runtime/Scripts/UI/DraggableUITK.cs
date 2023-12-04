using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;

namespace Tools.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class DraggableUITK : MonoBehaviour
    {
        [SerializeField]
        private string targetAreaId = "window-bar";

        [SerializeField]
        private string targetId = "window";

        [SerializeField]
        private bool contraintToViewPort = true;

        private bool isMouseDown = false;
        private Vector2 startMousePosition;
        private Vector2 rectStartPosition;
        private Vector2 targetPosition;

        private VisualElement targetArea;
        private VisualElement target;

        private PanelSettings panelSettings;

        public void Start()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("UIDocument not found");
                return;
            }

            if (panelSettings == null)
                panelSettings = uiDocument.panelSettings;

            target = uiDocument.rootVisualElement.Q<VisualElement>(targetId);
            if (target == null)
            {
                Debug.LogError("Target not found on provided UIDocument. Please check if the targetId is correct");
                return;
            }

            targetArea = uiDocument.rootVisualElement.Q<VisualElement>(targetAreaId);
            if (targetArea == null)
            {
                Debug.LogError("Target area not found on provided UIDocument. Please check if the targetAreaId is correct");
                return;
            }

            targetArea.RegisterCallback<PointerDownEvent>(OnPointerDown);

            uiDocument.rootVisualElement.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        public void OnPointerDown(PointerDownEvent evt)
        {
            isMouseDown = true;

            startMousePosition = Input.mousePosition;
            rectStartPosition = new Vector2(target.style.marginLeft.value.value, target.style.marginTop.value.value);
        }

        public void OnPointerUp(PointerUpEvent evt)
        {
            isMouseDown = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isMouseDown)
                return;

            // Force release the mouse capture even if the mouse is not over the element anymore
            if (Input.GetMouseButtonUp(0))
            {
                isMouseDown = false;
                return;
            }

            Move();
        }

        public void Move()
        {
            Vector2 mouseDiff = (Vector2)Input.mousePosition - startMousePosition;
            Vector2 mouseDiffScreen = new(mouseDiff.x, -mouseDiff.y);

            Vector2 contentSize = target.contentRect.size;

            var scale = UiHelper.GetScale(panelSettings);
            Vector2 viewPortSize = UiHelper.GetScaledViewport(panelSettings);

            targetPosition = rectStartPosition + (mouseDiffScreen / scale);            

            if (contraintToViewPort)
            {
                if (targetPosition.x < 0)
                    targetPosition.x = 0;
                else if (targetPosition.x + contentSize.x > viewPortSize.x)
                    targetPosition.x = viewPortSize.x - contentSize.x;

                if (targetPosition.y < 0)
                    targetPosition.y = 0;
                else if (targetPosition.y + contentSize.y > viewPortSize.y)
                    targetPosition.y = viewPortSize.y - contentSize.y;
            }


            //TODO: apply learping
            target.style.marginLeft = targetPosition.x;
            target.style.marginTop = targetPosition.y;
        }
    }
}