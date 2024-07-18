using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ClockUI : MonoBehaviour
{
    // Singular Cache Variable
    TextMeshProUGUI textBox;

    // Start is called before the first frame update
    void Start()
    {
        textBox = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textBox.text = "<b>Day: " + universalClock.getDay() + "<br><br>Time: " + universalClock.mainGameTime.ToString() + "</b><br><br>";
    }
}
