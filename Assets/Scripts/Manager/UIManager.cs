using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;

namespace Manager
{
    public class UIManager : NetworkBehaviour
    {
        public TMP_Text messageText;

        public void EndGame(string message)
        {
            messageText.text = message;
        }
        
        public void ShowMessage(string message)
        {
            messageText.text = message;
            StartCoroutine(ClearMessageAfterDelay(1f));
        }

        IEnumerator ClearMessageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            messageText.text = "";
        }
    }
}