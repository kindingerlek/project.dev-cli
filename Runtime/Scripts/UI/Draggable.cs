using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEditor;

namespace Tools.UI
{
    public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private RectTransform target;


        [SerializeField]
        private bool shouldReturn = false;

        [SerializeField]
        private bool contraintToViewPort = true;

        private bool isMouseDown = false;
        private Vector3 startMousePosition;
        private Vector3 startPosition;        

        public void OnPointerDown(PointerEventData dt)
        {
            isMouseDown = true;

            startPosition = target.position;
            startMousePosition = Input.mousePosition;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData dt)
        {
            isMouseDown = false;

            if (shouldReturn)
                target.position = startPosition;
        }

        void Reset()
        {
            target = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            if (isMouseDown)
            {
                Vector3 currentPosition = Input.mousePosition;
                Vector3 diff = currentPosition - startMousePosition;
                Vector3 pos = startPosition + diff;

                target.position = pos;
            }
        }
    } 
}