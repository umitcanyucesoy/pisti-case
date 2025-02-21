using _Case.Scripts.Data;
using TMPro;
using UnityEngine;

namespace _Case.Scripts.UI
{
    public class GameBoardUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] botMoneyTexts;    
        [SerializeField] private TextMeshProUGUI playerMoneyText; 
        [SerializeField] private TextMeshProUGUI betText;

        private void Start()
        {
            if (LobbyData.SelectedPlayersCount == 4 && botMoneyTexts != null)
            {
                foreach (TextMeshProUGUI botText in botMoneyTexts)
                {
                    botText.text = LobbyData.SelectedRoomBet.ToString("N0");
                }
            }
            else if (botMoneyTexts != null && botMoneyTexts.Length > 0)
            {
                botMoneyTexts[0].text = LobbyData.SelectedRoomBet.ToString("N0");
            }
            
            playerMoneyText.text = LobbyData.PlayerMoney.ToString("N0");

            int betToDisplay = LobbyData.SelectedBet > 0 
                ? LobbyData.SelectedBet 
                : LobbyData.SelectedRoomBet;

            betText.text = betToDisplay.ToString("N0");
        }
    }
}