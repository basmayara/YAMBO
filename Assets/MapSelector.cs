using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelector : MonoBehaviour
{
    public void LoadMap1()
    {
        SceneManager.LoadScene("Map1");
    }

    public void LoadMap2()
    {
        SceneManager.LoadScene("Map2");
    }
    public void LoadMenuMaps()
    {
        SceneManager.LoadScene("MenuMaps");
    }
}