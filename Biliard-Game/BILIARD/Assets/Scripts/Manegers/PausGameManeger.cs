using UnityEngine;
using UnityEngine.UI;
public class PausGameManeger : MonoBehaviour
{   
    [SerializeField] bool isGamePaused = false;
    [SerializeField] Image playAgainImage = default;
    private void Start() => EndTurnManeger.OnEndGame += DisablePauseGameOptionByUserAndTurnOnPlayAgainImage;
    private void OnDisable() => EndTurnManeger.OnEndGame -= DisablePauseGameOptionByUserAndTurnOnPlayAgainImage;
    private void Update() => PauseAndUnpausGame();
    void PauseAndUnpausGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isGamePaused = !isGamePaused;
            Time.timeScale = isGamePaused ? 0 : 1;
            playAgainImage.gameObject.SetActive(isGamePaused);
        }
    }
    /// <summary>
    /// when the game ended show play again end exit options end disallow to activate/deactivate 
    /// play again image by user
    /// </summary>
    void DisablePauseGameOptionByUserAndTurnOnPlayAgainImage()
    {
        enabled = false;
        //return if the play again image already active in the scene
        if (isGamePaused) return;
        playAgainImage.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}
   
   



   
