using System;
using TMPro;
using UnityEngine;

namespace Players
{
    public class NameField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameField;

        private void Awake()
        {
            _nameField.onEndEdit.AddListener(EndEdit);
        }

        private void EndEdit(string arg0)
        {
            FindObjectOfType<NameHolder>().PlayerName = arg0;
            _nameField.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _nameField.onEndEdit.RemoveAllListeners();
        }
    }
}