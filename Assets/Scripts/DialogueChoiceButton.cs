using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueChoiceButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI choiceText; 

    private DialogueManager _dialogueManager;
    private int _choiceIndex;

   
    public void Setup(Choice choice, DialogueManager manager)
    {
        _dialogueManager = manager;
        _choiceIndex = choice.index;

        if (choiceText != null)
        {
            choiceText.text = choice.text;
        }
    }

  
    public void SelectChoice()
    {
        if (_dialogueManager != null)
        {
            _dialogueManager.MakeChoice(_choiceIndex);
        }
    }
}