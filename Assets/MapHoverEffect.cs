using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapHoverEffect : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad;
    public float overlayAlpha = 0.6f;

    private Image darkOverlay;
    private GameObject playButton;
    private bool isHovered = false;

    void Start()
    {
        CreateOverlay();
        CreatePlayButton();
    }

    void CreateOverlay()
    {
        GameObject overlay = new GameObject("DarkOverlay");
        overlay.transform.SetParent(transform, false);
        overlay.transform.SetAsLastSibling();

        RectTransform rt = overlay.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        darkOverlay = overlay.AddComponent<Image>();
        darkOverlay.color = new Color(0, 0, 0, 0);
        darkOverlay.raycastTarget = false;
    }

    void CreatePlayButton()
    {
        playButton = new GameObject("PlayButton");
        playButton.transform.SetParent(transform, false);
        playButton.transform.SetAsLastSibling();

        RectTransform rt = playButton.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(160, 55);
        rt.anchoredPosition = Vector2.zero;

        Image img = playButton.AddComponent<Image>();
        img.color = new Color(0.08f, 0.07f, 0.08f, 1f);
        img.raycastTarget = false;

        // Texte PLAY
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(playButton.transform, false);

        RectTransform trt = textObj.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;
        trt.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();

        TMP_FontAsset cinzelFont = UnityEditor.AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/fonts/Cinzel/Cinzel-VariableFont_wght SDF.asset");
        if (cinzelFont != null)
            tmp.font = cinzelFont;

        tmp.text = "PLAY";
        tmp.fontSize = 28;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.enableVertexGradient = false;
        tmp.raycastTarget = false;
        tmp.alignment = TextAlignmentOptions.Center;
        // ← supprimé : tmp.faceColor et tmp.fontMaterial qui causaient le fond blanc

        playButton.SetActive(false);
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        RectTransform rect = GetComponent<RectTransform>();

        bool over = RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos, null);

        if (over && !isHovered)
        {
            isHovered = true;
            darkOverlay.color = new Color(0, 0, 0, overlayAlpha);
            playButton.SetActive(true);
        }
        else if (!over && isHovered)
        {
            isHovered = false;
            darkOverlay.color = new Color(0, 0, 0, 0);
            playButton.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (playButton.activeSelf)
            {
                RectTransform btnRect = playButton.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(btnRect, mousePos, null))
                {
                    if (!string.IsNullOrEmpty(sceneToLoad))
                        SceneManager.LoadScene(sceneToLoad);
                }
            }
        }
    }
}