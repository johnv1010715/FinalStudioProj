using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Slider : MonoBehaviour
{

    TMP_Text sliderText;

    // Start is called before the first frame update
    void Start()
    {
        sliderText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    public void PercentageText(float value)
    {
        sliderText.text = Mathf.RoundToInt(value * 100) + "%";
    }
}
