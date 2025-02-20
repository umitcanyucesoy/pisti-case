using _Case.Scripts.Data;
using TMPro;
using UnityEngine;

namespace _Case.Scripts.UI
{
    public class GameBoardUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI botMoneyText;    
        [SerializeField] private TextMeshProUGUI playerMoneyText; 
        [SerializeField] private TextMeshProUGUI betText;

        private void Start()
        {
            botMoneyText.text = LobbyData.SelectedRoomMaxBet.ToString("N0");
            playerMoneyText.text = LobbyData.PlayerMoney.ToString("N0");
        
            int betToDisplay = LobbyData.SelectedBet > 0 ? LobbyData.SelectedBet : LobbyData.SelectedRoomMaxBet;
            betText.text = betToDisplay.ToString("N0");
        }
    }
}