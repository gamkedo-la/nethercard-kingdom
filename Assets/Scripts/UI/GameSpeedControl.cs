using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSpeedControl : MonoBehaviour
{
    public TextMeshProUGUI gameSpeedLabel;

    public void SetGameSpeed(float value)
    {
        value = Mathf.Clamp(value, 0f, 1f);


        if (value != 0)
            Time.timeScale = 0.5f + (value / 2);
        else
            Time.timeScale = 0.5f;

        //masterSoundBus.setVolume( value ); // Used by FMOD

        if (gameSpeedLabel)
            gameSpeedLabel.text = $"{100 * (0.5f + (value / 2))}%";
    }
}
