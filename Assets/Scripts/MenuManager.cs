using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("=== PANELS ===")]
    public GameObject menuPanel;
    public GameObject profilPanel;
    public GameObject settingsPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("MissionScene");
    }

    public void OpenMissions()
    {
        SceneManager.LoadScene("MissionsScene");
    }

    public void OpenScores()
    {
        SceneManager.LoadScene("ScoresScene");
    }

    public void OpenShop()
    {
        SceneManager.LoadScene("ShopScene");
    }

    public void OpenSettings()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenProfil()
    {
        Debug.Log("Profil button clicked");

        menuPanel.SetActive(false);
        profilPanel.SetActive(true);

        // FORCE le panel devant tous les autres
        profilPanel.transform.SetAsLastSibling();

    }
    public void CloseProfil()
    {
        profilPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void Logout()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}