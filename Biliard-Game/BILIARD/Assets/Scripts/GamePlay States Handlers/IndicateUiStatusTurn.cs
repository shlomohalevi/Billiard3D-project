using System.Collections;
using UnityEngine;
using TMPro;
/// <summary>
/// class that responsible to update image text of
/// the status of the turn
/// </summary>

public class IndicateUiStatusTurn : MonoBehaviour
{
    public static IndicateUiStatusTurn instance;
    [SerializeField] TextMeshProUGUI foulText = default, aiWinText = default, playerWinText = default;
    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<IndicateUiStatusTurn>();
    }
    public IEnumerator UpdateStatusTurnImage(TurnResultState resultOfCurrentTurn)
    {
        if (resultOfCurrentTurn != TurnResultState.CurrentPlayerPunished) yield break;
        foulText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.8f);
        foulText.gameObject.SetActive(false);
    }
    public void ShowWinText(bool isHumanPlayerWin)
    {
        if (isHumanPlayerWin)
            playerWinText.gameObject.SetActive(true);
        else
            aiWinText.gameObject.SetActive(true);
    }
}






    
       
        
        

