using UnityEngine;
using TMPro;

public class CodeManager : MonoBehaviour
{
    public static CodeManager instance;
    public string fullCode = "5821"; // L'code s-7i7
    private char[] displayCode = { '*', '*', '*', '*' }; // Chnu kiban f l'UI
    public TextMeshProUGUI codeUIText;

    void Awake() { instance = this; }

    void Start() { UpdateUI(); }

    public void CollectDigit(int digit, int position)
    {
        // Beddel '*' b l'raqm l'7aqiqi f l'blassa dyalo
        displayCode[position] = digit.ToString()[0];
        UpdateUI();

        // Check ila l'code t-kammel
        string currentCode = new string(displayCode);
        if (currentCode == fullCode)
        {
            Debug.Log("Code t-kammel! Tqdri t-dkheli l'Castle");
        }
    }

    void UpdateUI()
    {
        codeUIText.text = "CODE: " + new string(displayCode);
    }
}