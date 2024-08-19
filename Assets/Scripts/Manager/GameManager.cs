using System;
using System.Collections.Generic;
using Enums;
using Flag;
using Mirror;
using Net;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public GameObject flag;
        public Material[] materialColor;
        public ColorEnum[] party;
        
        public event Action OnAddPlayerToCollections;
        private const int requiredPlayers = 3;
        public List<Player.Player> players = new List<Player.Player>(3);
        
        private void Start()
        {
            OnAddPlayerToCollections += () =>
            {
                //Debug.Log("player connect");
                
                if (players.Count == requiredPlayers)
                {
                    StartGame();
                }
            };
        }

        public void AddPlayerCollections(Player.Player player)
        {
            players.Add(player);
            OnAddPlayerToCollections?.Invoke();
        }

        private void StartGame()
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].ColorPlayers(party[i]);
                players[i].CreateFlag();
            }
        }
    }
}
