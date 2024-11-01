using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UiHandler : MonoBehaviour
{

    [SerializeField] Button ExitButton;
    // Start is called before the first frame update


    private void OnEnable()
    {
        ExitButton.onClick.AddListener(OnExitButtonPressed);

    }

    private void OnExitButtonPressed()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        ExitButton.onClick.RemoveListener(OnExitButtonPressed);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
