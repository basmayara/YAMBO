using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;

    void Start()
    {
        settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    { 
        settingsPanel.SetActive(false);
    }
}