using UnityEngine;

public class ClosePopup : MonoBehaviour
{
    public GameObject popupToClose;

    public void Close()
    {
        if (popupToClose != null)
        {
            popupToClose.SetActive(false);
            Debug.Log("✅ Popup fermé");
        }
    }
}