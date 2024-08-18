using System.Collections.Generic;
using Manager;
using Mirror;
using UnityEngine;

namespace Net
{
    public class ClientRpc
    {
        public ClientRpc()
        {
            
        }
        
        public void SendMessage(bool isServer, UIManager uiManager, string message)
        {
            if (!isServer)
                SendMessageCmd(uiManager, message);
            else
            {
                uiManager.ShowMessage(message);
                SendMessageRpc(uiManager, message);
            }
                
        }
        
        [Command(requiresAuthority = false)]
        private void SendMessageCmd(UIManager uiManager, string message)
        {
            SendMessageRpc(uiManager, message);
        }
        
        [ClientRpc]
        private void SendMessageRpc(UIManager uiManager, string message)
        {
            uiManager.ShowMessage(message);
        }
    }
}