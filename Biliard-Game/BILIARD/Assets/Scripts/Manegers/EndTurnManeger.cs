using UnityEngine;
using System.Linq;

using System.Collections;
using System;


public class EndTurnManeger : MonoBehaviour
{

    public static EndTurnManeger Instance;
    public static Action OnEndGame;
    [SerializeField] int initialBallsListCount = 16;
    TurnResultState currentResult = TurnResultState.non;
   
  
    private void Awake()
    {
        #region singelton
        if (Instance == null)
            Instance = FindObjectOfType<EndTurnManeger>();
        #endregion
    }
    private void OnEnable()
    {
        GameplayStatesHandler.onEndTurn += CheckResultAndKeepToTheNextTurn;
    }

    private void OnDisable()
    {
        GameplayStatesHandler.onEndTurn -= CheckResultAndKeepToTheNextTurn;
    }


    /// <summary>
    /// check result off current turn and depending on the result keep to the next turn
    /// </summary>

    void CheckResultAndKeepToTheNextTurn()
    {
        //get result of current turn
        currentResult = TurnResult();

        if (AnyBallGoesToThePocket())
        {
            if (CueBallGoesToThePocket())
            {
                returnCueBallToActiveBallsListInTheGameEndRemoveCueballFromBallThatGoesToThePocketList();
                UnitBall cueBall = GameplayStatesHandler.instance.activeBallsInTheGame.Find(x => x.ballType == BallType.cueBall);
                //return the physical cueBall to table
                ReturnCueBallToToTable(cueBall);
            }
            //clear that list couse we already check this turn
            GameplayStatesHandler.instance.ballsThatGoesToPocets.Clear();
        }
        //clear that varible couse we already check this first touch ball
        if (GameplayStatesHandler.instance.firstBallThatTouchTheCueBall != null)
            GameplayStatesHandler.instance.firstBallThatTouchTheCueBall = null;
        KeepToNextTurn();
    }
    /// <summary>
    /// check the result of the turn
    /// </summary>
    /// <returns>
    /// return current turn result
    /// </returns>
    TurnResultState TurnResult()
    {
        if (AnyBallGoesToThePocket())
        {
            if (Ball8GoesToThePocket())
            {
                if (ItsFirstTimeFromBeginingOfTheGameThatAnyBallsGoesToThePucket()) return TurnResultState.CurrentPlayerLose;
                if (!currentPlayerAlreadyInsertAllHisBallsToThePocket()) return TurnResultState.CurrentPlayerLose;
                if (CueBallGoesToThePocket()) return TurnResultState.CurrentPlayerLose;
                if (CurrentPlayerPutOneOfHisBallsInThePockectAfterThe8Ball()) return TurnResultState.CurrentPlayerLose;
                    return TurnResultState.currentPlayerWin;
            }
            else
            {
                if (ItsFirstTimeFromBeginingOfTheGameThatAnyBallsGoesToThePucket())
                {
                    
                    if (CueBallGoesToThePocket()) return TurnResultState.CurrentPlayerPunished;
                    if (CurrentPlayerPutOpponentBallInThePocket()) return TurnResultState.CurrentPlayerPunished;
                    if (TouchOpponentBallInFirstTouch()) return TurnResultState.CurrentPlayerPunished;
                    if (Touch8BallInFirstTouch()) return TurnResultState.CurrentPlayerPunished;
                    return TurnResultState.bonusTurnAndCanNotPlaceCueBallEverywhere;
                }
                else
                {
                    if (CueBallGoesToThePocket())return TurnResultState.CurrentPlayerPunished;
                    if (CurrentPlayerPutOpponentBallInThePocket()) return TurnResultState.CurrentPlayerPunished;
                    if (TouchOpponentBallInFirstTouch()) return TurnResultState.CurrentPlayerPunished;
                    if (Touch8BallInFirstTouch()) return TurnResultState.CurrentPlayerPunished;
                    return TurnResultState.bonusTurnAndCanNotPlaceCueBallEverywhere;
                }
            }
        }
        else
        {
            if (StillNoBallGoesToThePockectFromTheBeginingOfTheGame())
            {
                if (CurrentPlayerNotTouchAnyBall()) return TurnResultState.CurrentPlayerPunished;
                if ( Touch8BallInFirstTouch()) return TurnResultState.CurrentPlayerPunished;
                return TurnResultState.keepToOpponentTurnAsUsual;
            }
            else
            {
                if (CurrentPlayerNotTouchAnyBall()) return TurnResultState.CurrentPlayerPunished;
                if (TouchOpponentBallInFirstTouch()) return TurnResultState.CurrentPlayerPunished;
                if (Touch8BallInFirstTouch()) return TurnResultState.CurrentPlayerPunished;
                return TurnResultState.keepToOpponentTurnAsUsual;
            }
        }
    }
   
    /// <returns>true if any ball goes to the pocket</returns>
    bool AnyBallGoesToThePocket()
    {
        if (GameplayStatesHandler.instance.ballsThatGoesToPocets.Count > 0) return true;
        return false;
    }
    /// <returns>true if 8 ball goes to the pocket</returns>
    bool Ball8GoesToThePocket()
    {
        if (GameplayStatesHandler.instance.ballsThatGoesToPocets.Exists(x => x.ballType == BallType.blackBall)) return true;
        return false;
    }
    bool ItsFirstTimeFromBeginingOfTheGameThatAnyBallsGoesToThePucket()
    {
        if (GameplayStatesHandler.instance.activeBallsInTheGame.Count + GameplayStatesHandler.instance.ballsThatGoesToPocets.Count == initialBallsListCount) return true;
        return false;
    }
    /// <returns>true if cue ball goes to the pocket</returns>
    bool CueBallGoesToThePocket()
    {
        if (GameplayStatesHandler.instance.ballsThatGoesToPocets.Exists(x => x.ballType == BallType.cueBall)) return true;
        return false;
    }
    bool CurrentPlayerPutOpponentBallInThePocket()
    {
        if (GameplayStatesHandler.instance.isHumanPlayerTurn && GameplayStatesHandler.instance.ballsThatGoesToPocets.Exists(x => x.ballType == BallType.computerPlayerBall)) return true;
        if( !GameplayStatesHandler.instance.isHumanPlayerTurn && GameplayStatesHandler.instance.ballsThatGoesToPocets.Exists(x => x.ballType == BallType.humanPlayerBall)) return true;
        return false;
    }
    /// <returns>true if any player touch 8 ball in first touch</returns>
    bool Touch8BallInFirstTouch()
    {
        if (GameplayStatesHandler.instance.firstBallThatTouchTheCueBall.ballType == BallType.blackBall
            && GameplayStatesHandler.instance.activeBallsInTheGame.Exists(x => x.ballType == BallType.non)) return true;
        if (GameplayStatesHandler.instance.isHumanPlayerTurn)
        {
            if (GameplayStatesHandler.instance.firstBallThatTouchTheCueBall.ballType == BallType.blackBall
                && GameplayStatesHandler.instance.activeBallsInTheGame.Exists(x => x.ballType == BallType.humanPlayerBall)) return true;
        }
        else
        {
            if (GameplayStatesHandler.instance.firstBallThatTouchTheCueBall.ballType == BallType.blackBall
              && GameplayStatesHandler.instance.activeBallsInTheGame.Exists(x => x.ballType == BallType.computerPlayerBall)) return true;
        }

        return false;
    }
     /// <returns>true if touch opponent ball in first touch</returns>
    bool TouchOpponentBallInFirstTouch()
    {
        if (GameplayStatesHandler.instance.isHumanPlayerTurn && GameplayStatesHandler.instance.firstBallThatTouchTheCueBall.ballType == BallType.computerPlayerBall) return true;
        if(!GameplayStatesHandler.instance.isHumanPlayerTurn && GameplayStatesHandler.instance.firstBallThatTouchTheCueBall.ballType == BallType.humanPlayerBall) return true;
        return false;
    }
  
    /// <returns>true if 8 ball is not the last ball that goes to the pocket in current turn</returns>
    bool CurrentPlayerPutOneOfHisBallsInThePockectAfterThe8Ball()
    {
        UnitBall lastBallThatgoesToThePocket = GameplayStatesHandler.instance.ballsThatGoesToPocets.Last();
        if (lastBallThatgoesToThePocket.ballType != BallType.blackBall) return true;
        return false;
    }
  
   

    /// <returns>true if current player alraedy insert all his ball to the pocketf</returns>
    bool currentPlayerAlreadyInsertAllHisBallsToThePocket()
    {
        if (ItsFirstTimeFromBeginingOfTheGameThatAnyBallsGoesToThePucket()) return false;
        if (GameplayStatesHandler.instance.isHumanPlayerTurn && !GameplayStatesHandler.instance.activeBallsInTheGame.Exists(x => x.ballType == BallType.humanPlayerBall)) return true;
        if(!GameplayStatesHandler.instance.isHumanPlayerTurn && !GameplayStatesHandler.instance.activeBallsInTheGame.Exists(x => x.ballType == BallType.computerPlayerBall)) return true;
        return false;
    }
    bool StillNoBallGoesToThePockectFromTheBeginingOfTheGame() => GameplayStatesHandler.instance.activeBallsInTheGame.Count == initialBallsListCount;
    bool CurrentPlayerNotTouchAnyBall() => GameplayStatesHandler.instance.firstBallThatTouchTheCueBall == null;
  
  
  
    void returnCueBallToActiveBallsListInTheGameEndRemoveCueballFromBallThatGoesToThePocketList()
    {
        UnitBall cueBall = GameplayStatesHandler.instance.ballsThatGoesToPocets.Find(x => x.ballType == BallType.cueBall);
        GameplayStatesHandler.instance.activeBallsInTheGame.Add(cueBall);
        GameplayStatesHandler.instance.ballsThatGoesToPocets.Remove(cueBall);
    }
    /// <summary>
    /// return cueBall to the table after cueball goes to the pocket
    /// </summary>
    void ReturnCueBallToToTable(UnitBall cueBall)
    {
        RepositioningCueBall(cueBall);
        //if its going to be ai turn we activate the cueball after we find direction so we we do not need 
        // to activate the cueball now
        if (GameplayStatesHandler.instance.isHumanPlayerTurn) return;
        ActivateCueBall(cueBall);
    }
    void RepositioningCueBall(UnitBall  cueBall)
    {
        float zPosValue = -4;
        Vector3 newCueBallPos = new Vector3(0, 0, zPosValue);
        while(true)
        {
            if(!Physics.CheckSphere(newCueBallPos,UnitBall.radius,LayerMask.GetMask("Ball")))
            {
                cueBall.GetComponent<Collider>().isTrigger = false;
                cueBall.transform.position = newCueBallPos;
                cueBall.GetComponent<Rigidbody>().useGravity = true;
                break;
            }
            newCueBallPos = new Vector3(newCueBallPos.x,newCueBallPos.y, zPosValue += 0.05f);
        }
    }
    void ActivateCueBall(UnitBall CueBall) => CueBall.gameObject.SetActive(true);
  /// <summary>
  /// keep to the next turn according current turn result
  /// </summary>

    void KeepToNextTurn()
    {
        //update turn state ui
        IndicateUiStatusTurn.instance.StartCoroutine(IndicateUiStatusTurn.instance.UpdateStatusTurnImage(currentResult));
        if (currentResult == TurnResultState.keepToOpponentTurnAsUsual)
        {
            if (GameplayStatesHandler.instance.isHumanPlayerTurn)
            {
                GameplayStatesHandler.instance.isHumanPlayerTurn = false;
                AiStatesManeger.instance.SwitchToTheNextState(FindDirectionToHitBallToInsertToPocketState.instance);return;
            }
            else
            {
                GameplayStatesHandler.instance.isHumanPlayerTurn = true;
                HumanPlayerStateManeger.instance.SwitchToTheNextState(HumanPositioningState.instance);return;
            }
        }
       
        else if(currentResult == TurnResultState.bonusTurnAndCanNotPlaceCueBallEverywhere)
        {
            if (GameplayStatesHandler.instance.isHumanPlayerTurn)
            {
                HumanPlayerStateManeger.instance.SwitchToTheNextState(HumanPositioningState.instance); return;
            }
            else
            {
                AiStatesManeger.instance.SwitchToTheNextState(FindDirectionToHitBallToInsertToPocketState.instance); return;
            }
        }
        else if(currentResult == TurnResultState.CurrentPlayerPunished)
        {
            if (GameplayStatesHandler.instance.isHumanPlayerTurn)
            {
                GameplayStatesHandler.instance.isHumanPlayerTurn = false;
                AiStatesManeger.instance.SwitchToTheNextState(AiBonusTurnState.instance); return;
            }
            else
            {
                GameplayStatesHandler.instance.isHumanPlayerTurn = true;
                HumanBonusTurnState.instance.isBonusTurn = true;
                HumanPlayerStateManeger.instance.SwitchToTheNextState(HumanPositioningState.instance); return;

            }
        }
        else if(currentResult == TurnResultState.CurrentPlayerLose || currentResult == TurnResultState.currentPlayerWin)
        {
            OnEndGame?.Invoke();
            if (GameplayStatesHandler.instance.isHumanPlayerTurn)
            {
                if (currentResult == TurnResultState.currentPlayerWin)
                    IndicateUiStatusTurn.instance.ShowWinText(true);
                else
                    IndicateUiStatusTurn.instance.ShowWinText(false);
            }
            else
            {
                if (currentResult == TurnResultState.currentPlayerWin)
                    IndicateUiStatusTurn.instance.ShowWinText(false);
                else
                    IndicateUiStatusTurn.instance.ShowWinText(true);

            }
        }
      
    }

}

  


   
  

    
   





    
    
    
    

    
            




   
   


   



   




  





















                   




                   









