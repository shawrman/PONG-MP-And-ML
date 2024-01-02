using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class setText : MonoBehaviour
{
    public TextMeshProUGUI MainText;
    public TextMeshProUGUI SubText;

    public void SetText(string mainText,string subText)
    {
        MainText.text = mainText;
        SubText.text = subText;
    }
}
