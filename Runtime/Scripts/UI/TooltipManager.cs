using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.UI
{
    public class TooltipManager : MonoBehaviour
    {
        private const string UILayerName = "UI";
        private const string StandardSpritePath = "UI/Skin/UISprite.psd";
        private const string BackgroundSpriteResourcePath = "UI/Skin/Background.psd";

        private static Color panelColor = new Color(1f, 1f, 1f, 1f);
        private static Color textColor = new Color(0.2f, 0.2f, 0.2f, 1f);


        [SerializeField]
        private Canvas canvas;
        
        [SerializeField]
        private Image background;
        
        [SerializeField]
        private Text text;

        public bool IsShowing { get => background.gameObject.activeSelf; }


        private void Awake()
        {
            canvas = GetCanvas();
            background = GetBackgroundImage(canvas.transform);
            text = GetTooltipText(background.transform);
        }

        private void Reset()
        {
            canvas = GetCanvas();
            background = GetBackgroundImage(canvas.transform);
            text = GetTooltipText(background.transform);

            GetHorizontalLayoutGroup();
            GetContentSizeFitter();
        }

        public void Show(string message, Vector2 position)
        {
            text.text = message;
            background.rectTransform.position = position + (Vector2.down * 16);
            
            background.gameObject.SetActive(true);
        }
        public void Hide()
        {
            text.text = "";
            background.rectTransform.position = Vector2.zero;
            background.gameObject.SetActive(false);
        }

        private Canvas GetCanvas()
        {
            GameObject gObj = GameObject.Find("TooltipCanvas") ?? new GameObject("TooltipCanvas", typeof(Canvas));

            if(Application.isPlaying)
                DontDestroyOnLoad(gObj);

            canvas = gObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
            canvas.pixelPerfect = true;

            var canvasScaler = gObj.GetComponent<CanvasScaler>() ?? gObj.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = 1;
            canvasScaler.referencePixelsPerUnit = 100;

            return canvas;
        }

        private Image GetBackgroundImage(Transform parent)
        {
            GameObject gObj = canvas.transform.Find("Background")?.gameObject ?? new GameObject("Background", typeof(RectTransform));

            gObj.transform.SetParent(parent);

            if (gObj.GetComponent<CanvasRenderer>() == null)
                gObj.AddComponent<CanvasRenderer>();

            var rect = gObj.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(80, 30);
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0f, 1);
            rect.position = new Vector2(0, 0);

            var background = rect.gameObject.GetComponent<Image>() ?? rect.gameObject.AddComponent<Image>();
            background.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(BackgroundSpriteResourcePath);
            background.type = Image.Type.Sliced;
            background.color = panelColor;
            background.raycastTarget = false;


            return background;
        }

        private Text GetTooltipText(Transform parent)
        {
            GameObject gObj = canvas.transform.Find("Background/Text")?.gameObject ?? new GameObject("Text", typeof(RectTransform));
            gObj.transform.SetParent(parent);

            if (gObj.GetComponent<CanvasRenderer>() == null)
                gObj.AddComponent<CanvasRenderer>();

            var rect = gObj.GetComponent<RectTransform>();

            rect.sizeDelta = new Vector2(80, 30);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(5, 5);
            rect.offsetMax = new Vector2(-5, -5);

            var text = rect.gameObject.GetComponent<Text>() ?? rect.gameObject.AddComponent<Text>();

            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.text = "Tooltip";
            text.color = textColor;
            text.fontSize = 12;

            return text;
        }
        private ContentSizeFitter GetContentSizeFitter()
        {
            var contentSizeFitter = background.gameObject.GetComponent<ContentSizeFitter>() ?? background.gameObject.AddComponent<ContentSizeFitter>();

            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return contentSizeFitter;
        }

        private HorizontalLayoutGroup GetHorizontalLayoutGroup()
        {
            var horizontalLayout = background.gameObject.GetComponent<HorizontalLayoutGroup>() ?? background.gameObject.AddComponent<HorizontalLayoutGroup>();

            horizontalLayout.padding.left = 10;
            horizontalLayout.padding.right = 10;
            horizontalLayout.padding.top = 3;
            horizontalLayout.padding.bottom = 3;

            horizontalLayout.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayout.childControlHeight = true;
            horizontalLayout.childControlWidth = true;

            return horizontalLayout;
        }
    }
}
