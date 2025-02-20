using System;
using _Case.Scripts.Cards;
using _Case.Scripts.Data;
using _Case.Scripts.Players;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Case.Scripts.UI
{
    public class ResultPanelManager : MonoBehaviour
    {
        [Header("----- UI References -----")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI resultMessageText;  
        [SerializeField] private TextMeshProUGUI scoreText;         
        [SerializeField] private Button backToLobbyButton;  
        [SerializeField] private Button exitButton;  
        
        public static ResultPanelManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (backToLobbyButton != null)
                backToLobbyButton.onClick.AddListener(OnBackToLobby);
            
            if (exitButton != null)
                exitButton.onClick.AddListener(ClosePopup);
        }

        public void ShowResult()
        {
            Player player = PlayerManager.Instance.players.Find(p => !p.isBot);
            Player bot = PlayerManager.Instance.players.Find(p => p.isBot);

            if (player == null || bot == null) return;

            string message = "";
            if (player.score > bot.score)
            {
                message = "You Win!";
        
                LobbyData.PlayerMoney += LobbyData.SelectedBet;
                LobbyData.PlayerWins++;
            }
            else if (player.score < bot.score)
            {
                message = "You Lose!";

                LobbyData.PlayerMoney -= LobbyData.SelectedBet;
                LobbyData.PlayerLosses++;
        
                if (LobbyData.PlayerMoney < 0) LobbyData.PlayerMoney = 0;
            }
            else
            {
                message = "Draw!";
            }

            if (resultMessageText)
                resultMessageText.text = message;

            if (scoreText)
                scoreText.text = $"Player: {player.score}  |  Bot: {bot.score}";

            if (resultPanel)
                resultPanel.SetActive(true);
        }

        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        private void OnBackToLobby()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}