using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfilPanelLayout : MonoBehaviour
{
    void Awake() { ArrangeLayout(); }

    public void ArrangeLayout()
    {
        SetColor(gameObject, "1A1F3A");
        SetRect("ProfileTitle", 0, 270, 300, 30);
        SetTextStyle("ProfileTitle", "PROFIL", "FFFFFF", 14, false, 3f);
        SetRect("CloseButton", 170, 270, 28, 28);
        StyleCloseButton();
        SetRect("AvatarRing", 0, 190, 96, 96);
        SetRect("ProfilAvatar", 0, 190, 84, 84);
        SetColor("ProfilAvatar", "5DCAA5");
        SetRect("PlayerName", 0, 128, 280, 30);
        SetTextStyle("PlayerName", null, "FFFFFF", 18, true, 0);
        CreateSeparator(0, 103, 340, 1, "3A4060");
        SetRect("StatCard_Score", -83, 45, 150, 68);
        SetRect("StatCard_Best", 83, 45, 150, 68);
        SetRect("StatCard_Lives", -83, -35, 150, 68);
        SetRect("StatCard_Level", 83, -35, 150, 68);
        SetColor("StatCard_Score", "242A4A");
        SetColor("StatCard_Best", "242A4A");
        SetColor("StatCard_Lives", "242A4A");
        SetColor("StatCard_Level", "242A4A");
        SetRectInParent("StatCard_Score", "LabelScore", 0, 20, 138, 18, 10);
        SetRectInParent("StatCard_Score", "ScoreText", 0, -8, 138, 28, 18);
        SetRectInParent("StatCard_Best", "LabelBest", 0, 20, 138, 18, 10);
        SetRectInParent("StatCard_Best", "BestScoreText", 0, -8, 138, 28, 18);
        SetRectInParent("StatCard_Lives", "LabelLives", 0, 20, 138, 18, 10);
        SetRectInParent("StatCard_Lives", "LivesText", 0, -8, 138, 28, 18);
        SetRectInParent("StatCard_Level", "LabelLevel", 0, 20, 138, 18, 10);
        SetRectInParent("StatCard_Level", "LevelText", 0, -8, 138, 28, 18);
        SetTextStyle("LabelScore", "SCORE", "8A90B0", 10, false, 1.5f);
        SetTextStyle("LabelBest", "MEILLEUR", "8A90B0", 10, false, 1.5f);
        SetTextStyle("LabelLives", "VIES", "8A90B0", 10, false, 1.5f);
        SetTextStyle("LabelLevel", "NIVEAU", "8A90B0", 10, false, 1.5f);
        SetTextStyle("ScoreText", null, "BA7517", 18, true, 0);
        SetTextStyle("BestScoreText", null, "BA7517", 18, true, 0);
        SetTextStyle("LivesText", null, "5DCAA5", 18, true, 0);
        SetTextStyle("LevelText", null, "7F77DD", 18, true, 0);
        CreateSeparator(0, -85, 340, 1, "3A4060");
        SetRect("XPText", 0, -105, 290, 22);
        SetRect("XPSlider", 0, -130, 290, 6);
        SetTextStyle("XPText", null, "8A90B0", 11, false, 0);
        SetRect("GamesPlayedText", 0, -600, 0, 0);
        StyleButton("ProfilPlayButton", "Jouer", "534AB7", "EEEDFE", 0, -178, 290, 44);
        StyleButton("ProfilSettingsButton", "Paramètres", "242A4A", "8A90B0", 0, -230, 290, 38);
        StyleButton("ProfilLogoutButton", "Déconnexion", "1A1F3A", "E24B4A", 0, -276, 290, 38);
        Debug.Log("Layout appliqué !");
    }

    void SetRect(string n, float x, float y, float w, float h)
    {
        Transform t = FindDeep(n);
        if (t == null) return;
        var rt = t.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(w, h);
    }

    void SetRectInParent(string parent, string child, float x, float y,
                         float w, float h, float fs = 0)
    {
        var p = transform.Find(parent);
        if (p == null) return;
        var t = p.Find(child);
        if (t == null) return;
        var rt = t.GetComponent<RectTransform>();
        if (rt != null) { rt.anchoredPosition = new Vector2(x, y); rt.sizeDelta = new Vector2(w, h); }
        if (fs > 0) { var tmp = t.GetComponent<TextMeshProUGUI>(); if (tmp != null) { tmp.fontSize = fs; tmp.alignment = TextAlignmentOptions.Center; } }
    }

    void SetColor(string n, string hex)
    {
        var t = FindDeep(n); if (t == null) return; SetColor(t.gameObject, hex);
    }

    void SetColor(GameObject go, string hex)
    {
        var img = go.GetComponent<Image>(); if (img == null) return;
        Color c; if (ColorUtility.TryParseHtmlString("#" + hex, out c)) img.color = c;
    }

    void SetTextStyle(string n, string text, string hex, float size, bool bold, float spacing)
    {
        foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (tmp.gameObject.name != n) continue;
            if (text != null) tmp.text = text;
            Color c; if (ColorUtility.TryParseHtmlString("#" + hex, out c)) tmp.color = c;
            tmp.fontSize = size;
            tmp.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            tmp.characterSpacing = spacing;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Ellipsis;
            return;
        }
    }

    void StyleButton(string n, string text, string bgHex, string textHex,
                     float x, float y, float w, float h)
    {
        var t = FindDeep(n); if (t == null) return;
        var rt = t.GetComponent<RectTransform>();
        if (rt != null) { rt.anchoredPosition = new Vector2(x, y); rt.sizeDelta = new Vector2(w, h); }
        var img = t.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = null; img.type = Image.Type.Simple;
            Color c; if (ColorUtility.TryParseHtmlString("#" + bgHex, out c)) img.color = c;
        }
        var tmp = t.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = text; tmp.fontSize = 14;
            tmp.fontStyle = FontStyles.Normal;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.characterSpacing = 0;
            Color c; if (ColorUtility.TryParseHtmlString("#" + textHex, out c)) tmp.color = c;
        }
    }

    void StyleCloseButton()
    {
        var t = FindDeep("CloseButton"); if (t == null) return;
        var img = t.GetComponent<Image>();
        if (img != null) { img.sprite = null; Color c; if (ColorUtility.TryParseHtmlString("#242A4A", out c)) img.color = c; }
        var tmp = t.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) { tmp.text = "X"; tmp.fontSize = 13; tmp.fontStyle = FontStyles.Normal; tmp.alignment = TextAlignmentOptions.Center; Color c; if (ColorUtility.TryParseHtmlString("#8A90B0", out c)) tmp.color = c; }
    }

    void CreateSeparator(float x, float y, float w, float h, string hex)
    {
        string n = "Sep_" + y;
        foreach (Transform child in transform) if (child.name == n) return;
        var go = new GameObject(n); go.transform.SetParent(transform, false);
        var rt = go.AddComponent<RectTransform>(); rt.anchoredPosition = new Vector2(x, y); rt.sizeDelta = new Vector2(w, h);
        var img = go.AddComponent<Image>(); Color c; if (ColorUtility.TryParseHtmlString("#" + hex, out c)) img.color = c;
    }

    void CreateAvatarRing()
    {
        if (transform.Find("AvatarRing") != null) return;
        var ring = new GameObject("AvatarRing"); ring.transform.SetParent(transform, false);
        var rt = ring.AddComponent<RectTransform>(); rt.anchoredPosition = new Vector2(0, 190); rt.sizeDelta = new Vector2(96, 96);
        var img = ring.AddComponent<Image>(); Color c; if (ColorUtility.TryParseHtmlString("#7F77DD", out c)) img.color = c;
        var avatar = transform.Find("ProfilAvatar");
        if (avatar != null) ring.transform.SetSiblingIndex(avatar.GetSiblingIndex());
    }

    Transform FindDeep(string n)
    {
        foreach (var t in GetComponentsInChildren<Transform>(true))
            if (t.name == n) return t;
        return null;
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ProfilPanelLayout))]
public class ProfilPanelLayoutEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(8);
        if (GUILayout.Button("Appliquer Layout", GUILayout.Height(32)))
            ((ProfilPanelLayout)target).ArrangeLayout();
    }
}
#endif