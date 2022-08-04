using UnityEngine;
/// <summary>
/// ai abstract state
/// </summary>
public abstract class AiPlayerAbstract : MonoBehaviour
{
    public abstract AiPlayerAbstract runCurrentAiPlayerUpdateState();
    protected static bool ThisAllowedBallTypeToHit(UnitBall ballToCheck)
    {
        if (ballToCheck.ballType == BallType.humanPlayerBall) return false;
        if (ballToCheck.ballType == BallType.cueBall) return false;
        if (ballToCheck.ballType == BallType.blackBall
            &&GameplayStatesHandler.instance. activeBallsInTheGame.Exists(x => x.ballType == BallType.computerPlayerBall)) return false;
        if (ballToCheck.ballType == BallType.blackBall
            && GameplayStatesHandler.instance.activeBallsInTheGame.Exists(x => x.ballType == BallType.non)) return false;
        return true;
    }
}
  
   
    
    
    
