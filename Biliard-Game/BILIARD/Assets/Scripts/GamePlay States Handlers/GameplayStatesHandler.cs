using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

/// <summary>
/// class that rellevant for both
/// player and ai during the states
/// </summary>
public class GameplayStatesHandler : MonoBehaviour
{
    public static GameplayStatesHandler instance;
    public static Action onEndTurn;
    public List<UnitBall> activeBallsInTheGame = new List<UnitBall>();
    public List<GameObject> pockets = new List<GameObject>();
    public List<UnitBall> ballsThatGoesToPocets = new List<UnitBall>();
    public GameObject aiCue;
    public GameObject playerCue;
    public UnitBall cueBall;
    public UnitBall firstBallThatTouchTheCueBall = null;
    public bool isHumanPlayerTurn = true;
    [SerializeField] AudioClip collisionSound = default;
    bool weAlreadySortedTheBallsToAiAndPlayer = false;
    float minMagnitudeVelocity = 0.02f;

    private void Awake()
    {
        #region singelton
        if (instance == null)
            instance = FindObjectOfType<GameplayStatesHandler>();
        #endregion
     
    }
   public void AddBallToBallGoesToPocketList(UnitBall ballThatGoasToThePocket) => ballsThatGoesToPocets.Add(ballThatGoasToThePocket);
   public void RemoveBallFromActivateBallsINGameList(UnitBall ballInPocket) => activeBallsInTheGame.Remove(ballInPocket);
   public void FirstBallThatTochedTheCueBall(UnitBall ballThatTouchCueBall)
    {
        if (firstBallThatTouchTheCueBall != null) return;
        firstBallThatTouchTheCueBall = ballThatTouchCueBall;
    }
    /// <summary>
    /// assign ball type to human and ai after the first ball goes to the pocket
    /// </summary>
   public void AssignBallTypeToPlayerAndAi(UnitBall ballThatGoesToThePocket)
    {
        if (weAlreadySortedTheBallsToAiAndPlayer || ballThatGoesToThePocket.ballType == BallType.blackBall || ballThatGoesToThePocket.ballType == BallType.cueBall) return;
        weAlreadySortedTheBallsToAiAndPlayer = true;
        if (ballThatGoesToThePocket.ballTextureType == ballTextureType.fullColor)
        {
            if (isHumanPlayerTurn)
            {
                SortBallsToHumanAndAi(BallType.humanPlayerBall, BallType.computerPlayerBall,
                ballTextureType.fullColor, ballTextureType.stripedColor);
            }
            else
            {
                SortBallsToHumanAndAi(BallType.computerPlayerBall, BallType.humanPlayerBall,
                ballTextureType.fullColor, ballTextureType.stripedColor);
            }
        }
        else if(ballThatGoesToThePocket.ballTextureType == ballTextureType.stripedColor)
        {
            if (isHumanPlayerTurn)
            {
                SortBallsToHumanAndAi(BallType.humanPlayerBall, BallType.computerPlayerBall,
                ballTextureType.stripedColor, ballTextureType.fullColor);
            }
            else
            {
                SortBallsToHumanAndAi(BallType.computerPlayerBall, BallType.humanPlayerBall,
                ballTextureType.stripedColor, ballTextureType.fullColor);
            }

        }
    }
    /// <summary>
    /// assign ball type to human and ai accordint to who is now current turn and the texture of the ball that goes to the pocket
    /// </summary>

    void SortBallsToHumanAndAi(BallType ballTypeOfThoseWhosCurrentTurn, BallType ballTypeOfThoseWhosNotCurrentTurn
        , ballTextureType ballTextureTypeOfWhosCurrentTurn, ballTextureType ballTextureTypeOfWhosNotCurrentTurn)
    {
        //assign ball type to who is now current turn
        activeBallsInTheGame.Where(x => x.ballTextureType == ballTextureTypeOfWhosCurrentTurn)
       .ToList().ForEach(x => x.ballType = ballTypeOfThoseWhosCurrentTurn);
        ballsThatGoesToPocets.Where(x => x.ballTextureType == ballTextureTypeOfWhosCurrentTurn)
       .ToList().ForEach(x => x.ballType = ballTypeOfThoseWhosCurrentTurn);
        //assign ball type to who is not now current turn
        activeBallsInTheGame.Where(x => x.ballTextureType == ballTextureTypeOfWhosNotCurrentTurn)
       .ToList().ForEach(x => x.ballType = ballTypeOfThoseWhosNotCurrentTurn);
        ballsThatGoesToPocets.Where(x => x.ballTextureType == ballTextureTypeOfWhosNotCurrentTurn)
       .ToList().ForEach(x => x.ballType = ballTypeOfThoseWhosNotCurrentTurn);

    }
    /// <summary>
    /// wait until all balls stops moving and than swich state
    /// </summary>
   public IEnumerator WaiteToTheEndOfTheTurn()
    {
        yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() =>
        activeBallsInTheGame.TrueForAll(x => x.GetComponent<Rigidbody>().velocity.magnitude < minMagnitudeVelocity));
        activeBallsInTheGame.ToList().ForEach(x => x.GetComponent<Rigidbody>().velocity = Vector3.zero);
        activeBallsInTheGame.ToList().ForEach(x => x.GetComponent<Rigidbody>().angularVelocity = Vector3.zero);
        yield return new WaitForSeconds(2f);
        onEndTurn?.Invoke();
    }
    public void PlayCollisionHitSound(float volumAmount, Vector3 positionToPlaySound)
    {
        float volumeAmountToPlay = Mathf.InverseLerp(2, 15, volumAmount);
        AudioSource.PlayClipAtPoint(collisionSound, positionToPlaySound, volumeAmountToPlay);
    }
}
public enum TurnResultState
{
    non,
    keepToOpponentTurnAsUsual,
    CurrentPlayerPunished,
    bonusTurnAndCanNotPlaceCueBallEverywhere,
    CurrentPlayerLose,
    currentPlayerWin,
}




