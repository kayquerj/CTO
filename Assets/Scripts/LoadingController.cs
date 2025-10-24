using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    [SerializeField]
    private string sceneToLoad = string.Empty;

    [SerializeField, Range(0.5f, 10f)]
    private float fallbackDuration = 2.5f;

    private Image progressFill;
    private Text statusLabel;
    private GameObject loadingCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        BuildUserInterface();
    }

    private void OnEnable()
    {
        StartCoroutine(BeginLoading());
    }

    private IEnumerator BeginLoading()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            yield return LoadSceneAsync(sceneToLoad);
        }
        else
        {
            yield return SimulateLoading();
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation;
        try
        {
            operation = SceneManager.LoadSceneAsync(sceneName);
        }
        catch
        {
            yield return SimulateLoading();
            yield break;
        }

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);
            UpdateProgress(progress);

            if (progress >= 1f)
            {
                yield return new WaitForSeconds(0.2f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        yield return null;
        Cleanup();
    }

    private IEnumerator SimulateLoading()
    {
        var time = 0f;
        while (time < fallbackDuration)
        {
            time += Time.deltaTime;
            UpdateProgress(Mathf.Clamp01(time / fallbackDuration));
            yield return null;
        }

        UpdateProgress(1f);
        yield return new WaitForSeconds(0.5f);
        Cleanup();
    }

    private void UpdateProgress(float value)
    {
        if (progressFill != null)
        {
            progressFill.fillAmount = value;
        }

        if (statusLabel != null)
        {
            statusLabel.text = $"Carregando {(value * 100f):0}%";
        }
    }

    private void BuildUserInterface()
    {
        var canvasGo = new GameObject("LoadingCanvas");
        loadingCanvas = canvasGo;
        var uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer < 0)
        {
            uiLayer = 0;
        }

        canvasGo.layer = uiLayer;
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        var background = new GameObject("Background");
        background.transform.SetParent(canvas.transform, false);
        var bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = background.AddComponent<Image>();
        bgImage.color = new Color32(13, 17, 23, 230);

        var container = new GameObject("LoadingContainer");
        container.transform.SetParent(canvas.transform, false);
        var containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(420, 180);
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;

        CreateText(containerRect, "Title", "Preparando jogo", 24, FontStyle.Bold, new Vector2(0, 50));
        statusLabel = CreateText(containerRect, "Status", "Carregando 0%", 18, FontStyle.Normal, new Vector2(0, -10));

        var barBackground = new GameObject("ProgressBackground");
        barBackground.transform.SetParent(containerRect, false);
        var barRect = barBackground.AddComponent<RectTransform>();
        barRect.sizeDelta = new Vector2(360, 14);
        barRect.anchorMin = new Vector2(0.5f, 0.5f);
        barRect.anchorMax = new Vector2(0.5f, 0.5f);
        barRect.anchoredPosition = new Vector2(0, -60);
        var barBgImage = barBackground.AddComponent<Image>();
        barBgImage.color = new Color32(240, 246, 252, 35);

        var fill = new GameObject("ProgressFill");
        fill.transform.SetParent(barBackground.transform, false);
        var fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        var fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color32(88, 166, 255, 255);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 0f;
        progressFill = fillImage;

        SetLayerRecursively(canvasGo, uiLayer);
        DontDestroyOnLoad(canvasGo);
    }

    private Text CreateText(RectTransform parent, string name, string content, int fontSize, FontStyle style, Vector2 anchoredPosition)
    {
        var textGo = new GameObject(name);
        textGo.transform.SetParent(parent, false);
        var rect = textGo.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(360, 32);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;

        var text = textGo.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color32(240, 246, 252, 255);
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        if (text.font == null)
        {
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        return text;
    }

    private void Cleanup()
    {
        if (loadingCanvas != null)
        {
            Destroy(loadingCanvas);
            loadingCanvas = null;
        }

        Destroy(gameObject);
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        for (int i = 0; i < go.transform.childCount; i++)
        {
            SetLayerRecursively(go.transform.GetChild(i).gameObject, layer);
        }
    }
}
