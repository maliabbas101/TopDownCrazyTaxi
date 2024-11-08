using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class UiHandler : MonoBehaviour
{

    [FormerlySerializedAs("ExitButton")] [SerializeField]
    private Button exitButton;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(OnExitButtonPressed);

    }

    private static void OnExitButtonPressed()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        exitButton.onClick.RemoveListener(OnExitButtonPressed);
    }
    
}
