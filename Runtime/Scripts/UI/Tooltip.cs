using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tools.UI
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, Multiline]
        private string tooltipText = "Tooltip";
        [SerializeField]
        private float delayToShow = 0.75f;
        [SerializeField]
        private float delayToHide = 8f;

        private TooltipManager tooltipManager;
        

        private bool pointerEnter;
        private float pointerEnterTime;

        private void Awake()
        {
            tooltipManager = GameObject.FindObjectOfType<TooltipManager>();
        }

        private void Update()
        {
            if (!pointerEnter)
                return;

            if (Time.unscaledTime >= pointerEnterTime + delayToShow + delayToHide)
                tooltipManager.Hide();

            else if (!tooltipManager.IsShowing && Time.unscaledTime >= pointerEnterTime + delayToShow)
                tooltipManager.Show(tooltipText,Input.mousePosition);
        }

        private void Reset()
        {
            var gObj = GameObject.FindObjectOfType<TooltipManager>()?.gameObject ?? new GameObject("TooltipManager", typeof(TooltipManager));

            tooltipManager = gObj.GetComponent<TooltipManager>();
            tooltipManager.Hide();
        }

        

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerEnter = true;
            pointerEnterTime = Time.unscaledTime;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerEnter = false;
            pointerEnterTime = 0;
            tooltipManager.Hide();
        }

    } 
}
