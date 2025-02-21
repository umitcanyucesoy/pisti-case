using System.Collections.Generic;
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
            // Oyuncuyu buluyoruz
            Player player = PlayerManager.Instance.players.Find(p => !p.isBot);
            if (player == null) return;
            
            string message = "";
            
            if (LobbyData.SelectedPlayersCount == 2)
            {
                // 1v1 senaryosu: tek bot ile karşılaştırma
                Player bot = PlayerManager.Instance.players.Find(p => p.isBot);
                if (bot == null) return;
                
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
                    if (LobbyData.PlayerMoney < 0)
                        LobbyData.PlayerMoney = 0;
                }
                else
                {
                    message = "Draw!";
                }
                
                scoreText.text = $"Player: {player.score}  |  Bot: {bot.score}";
            }
            else if (LobbyData.SelectedPlayersCount == 4)
            {
                List<Player> botPlayers = PlayerManager.Instance.players.FindAll(p => p.isBot);
                int highestBotScore = 0;
                foreach (Player bot in botPlayers)
                {
                    if (bot.score > highestBotScore)
                        highestBotScore = bot.score;
                }
                
                if (player.score > highestBotScore)
                {
                    message = "You Win!";
                    LobbyData.PlayerMoney += LobbyData.SelectedBet;
                    LobbyData.PlayerWins++;
                }
                else if (player.score < highestBotScore)
                {
                    message = "You Lose!";
                    LobbyData.PlayerMoney -= LobbyData.SelectedBet;
                    LobbyData.PlayerLosses++;
                    if (LobbyData.PlayerMoney < 0)
                        LobbyData.PlayerMoney = 0;
                }
                else
                {
                    message = "Draw!";
                }
                
                scoreText.text = $"Player: {player.score}  |  Highest Bot: {highestBotScore}";
            }
            
            if (resultMessageText)
                resultMessageText.text = message;
            
            if (resultPanel)
                resultPanel.SetActive(true);
        }
        
        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }
        
        private void OnBackToLobby()
        {
            // Lobby sahnesine geçişte, SalonManager LobbyData.PlayerMoney değerini güncel olarak gösterecektir.
            SceneManager.LoadScene("Lobby");
        }
    }
}
