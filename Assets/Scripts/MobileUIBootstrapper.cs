using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace HollowKnightLike.UI
{
    [DefaultExecutionOrder(-50)]
    public class MobileUIBootstrapper : MonoBehaviour
    {
        [SerializeField] private bool onlyMobile = false;
        [SerializeField] private float buttonSize = 160f;
        [SerializeField] private float buttonSpacing = 24f;

        private bool _initialized;

        private void Awake()
        {
            if (_initialized)
            {
                return;
            }

            if (onlyMobile && !Application.isMobilePlatform)
            {
                return;
            }

            BuildUI();
            _initialized = true;
        }

        private void BuildUI()
        {
            var canvasGO = new GameObject("MobileControlsCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGO.transform.SetParent(transform, false);
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            EnsureEventSystem();

            CreateMovementButtons(canvasGO.transform as RectTransform);
            CreateActionButtons(canvasGO.transform as RectTransform);
            CreateInstructionText(canvasGO.transform as RectTransform);
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemGO = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            DontDestroyOnLoad(eventSystemGO);
        }

        private void CreateMovementButtons(RectTransform canvas)
        {
            float size = buttonSize;
            float spacing = buttonSpacing;

            RectTransform left = CreateButton(canvas, "MoveLeft", new Vector2(0, 0), new Vector2(size, size), new Vector2(spacing + size * 0.5f, spacing + size * 0.5f));
            var leftControl = left.gameObject.AddComponent<MobileOnScreenButton>();
            leftControl.SetControlPath("<Gamepad>/dpad/left");
            leftControl.SetPressedValue(1f);

            RectTransform right = CreateButton(canvas, "MoveRight", new Vector2(0, 0), new Vector2(size, size), new Vector2(spacing * 2f + size * 1.5f, spacing + size * 0.5f));
            var rightControl = right.gameObject.AddComponent<MobileOnScreenButton>();
            rightControl.SetControlPath("<Gamepad>/dpad/right");
            rightControl.SetPressedValue(1f);
        }

        private void CreateActionButtons(RectTransform canvas)
        {
            float size = buttonSize;
            float spacing = buttonSpacing;

            RectTransform jump = CreateButton(canvas, "Jump", new Vector2(1, 0), new Vector2(size, size), new Vector2(-(spacing + size * 0.5f), spacing + size * 0.5f));
            var jumpControl = jump.gameObject.AddComponent<MobileOnScreenButton>();
            jumpControl.SetControlPath("<Gamepad>/buttonSouth");
            jumpControl.SetPressedValue(1f);

            RectTransform dash = CreateButton(canvas, "Dash", new Vector2(1, 0), new Vector2(size * 0.9f, size * 0.9f), new Vector2(-(spacing * 2f + size * 1.5f), spacing + size * 0.75f));
            var dashControl = dash.gameObject.AddComponent<MobileOnScreenButton>();
            dashControl.SetControlPath("<Gamepad>/buttonEast");
            dashControl.SetPressedValue(1f);

            RectTransform attack = CreateButton(canvas, "Attack", new Vector2(1, 0), new Vector2(size * 0.9f, size * 0.9f), new Vector2(-(spacing * 3.5f + size * 2.2f), spacing + size * 0.75f));
            var attackControl = attack.gameObject.AddComponent<MobileOnScreenButton>();
            attackControl.SetControlPath("<Gamepad>/buttonWest");
            attackControl.SetPressedValue(1f);
        }

        private void CreateInstructionText(RectTransform canvas)
        {
            var textGO = new GameObject("Instructions", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textGO.transform.SetParent(canvas, false);
            var rect = textGO.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -40f);
            rect.sizeDelta = new Vector2(900f, 80f);

            var text = textGO.GetComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 32;
            text.text = "Toque nos botões para mover, pular, dar dash e atacar.";
            text.color = new Color(1f, 1f, 1f, 0.85f);
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private RectTransform CreateButton(RectTransform parent, string name, Vector2 anchor, Vector2 size, Vector2 anchoredPosition)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var image = go.GetComponent<Image>();
            image.color = new Color(0.15f, 0.15f, 0.18f, 0.6f);
            image.raycastTarget = true;

            return rect;
        }
    }
}
