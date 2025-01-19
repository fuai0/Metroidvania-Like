using TMPro;
using UnityEngine;

public class UI_StatToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;

    public void ShowToolTip(string _text)
    {
        description.text = _text;

        gameObject.SetActive(true);
    }

    public void HideToolTip()
    {
        description.text = "";
        gameObject.SetActive(false);
    }
}
