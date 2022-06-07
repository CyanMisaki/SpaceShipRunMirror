using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class NameField : MonoBehaviour
{
    [SerializeField]private TMP_InputField _inputField;
    public string PlayerName {get; private set;}
    
    private void Start()
    {
        _inputField.onEndEdit.AddListener(SavePlayerName);
    }

    private void SavePlayerName(string input)
    {
        PlayerName = input;
    }

    public void DisableField()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _inputField.onEndEdit.RemoveAllListeners();
    }
}
