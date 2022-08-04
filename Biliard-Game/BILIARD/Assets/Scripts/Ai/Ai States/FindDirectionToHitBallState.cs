using UnityEngine;
/// <summary>
/// state that responsible to find any ai ball regeardles to pocket if
/// ai not find pocket to insert the ai ball onto
/// </summary>
public class FindDirectionToHitBallState : AiPlayerAbstract
{
    public static FindDirectionToHitBallState instance;
    [SerializeField] float radiusToCheck = 13;
    private void Awake()
    {
        #region
        if (instance == null)
            instance = FindObjectOfType<FindDirectionToHitBallState>();
        #endregion
    }
    public override AiPlayerAbstract runCurrentAiPlayerUpdateState()
    {
        //find direction to hit the cueball
        Vector3 dirToHitAiBall = DirToHitAiBall();
        //if we find direction
        if (dirToHitAiBall != default)
        {
            AiPositioningCueState.instance.MethodsToExecuteWhenSwichingToAiPositioningCueState(dirToHitAiBall);
        }
        else
        {
            //find random direction to hit the cueball as the last defult case
            AiPositioningCueState.instance.MethodsToExecuteWhenSwichingToAiPositioningCueState(RandomDirToHitTheCueBall());
        }
            return AiPositioningCueState.instance;
    }


    /// <returns>
    /// direction to hit the cueball
    /// </returns>
    Vector3 DirToHitAiBall()
    {
        // three direction options to hit each chosen ball the left side sueface of chosen ball
        //  the right side and the center surface of chosen ball
        int PotentialDirectionsFromCueBallToEachAiBall = 3;
        for (int currentDirectionOptionNumber = 0; currentDirectionOptionNumber < PotentialDirectionsFromCueBallToEachAiBall; currentDirectionOptionNumber++)
        {
            for (int b = 0; b < GameplayStatesHandler.instance.activeBallsInTheGame.Count; b++)
            {
                UnitBall chosenBall = GameplayStatesHandler.instance.activeBallsInTheGame[b];
               // if (!chosenBall.gameObject.activeInHierarchy) continue;
                if (!ThisAllowedBallTypeToHit(chosenBall))continue;
                //define the direction to the chosen ball according the current direction Option
                Vector3 dirFromCueBallToAiBall = DirFromCueBallToAiBall(currentDirectionOptionNumber, chosenBall);
                if (!IsAllowedDirectionToHit(dirFromCueBallToAiBall)) continue;
                Vector3 dirToHitAiBall = dirFromCueBallToAiBall;
                float forceToAddToTheAiCue = dirToHitAiBall.magnitude;
                AiHitCueBallState.instance.GetDirAndForceToAddToTheCue(dirToHitAiBall, forceToAddToTheAiCue);
                return dirToHitAiBall;
            }

        }
        return default;
    }
    /// <summary>
    /// define the direction to the chosen ball according to the number of the direction option
    /// </summary>
    /// <param name="dirOptionCount">
    /// the number of the direction option to the chosen ball
    /// </param>
    /// <param name="chosenBall"></param>
    /// <returns>
    /// the direction from cue ball to chosen ball
    /// </returns>
    Vector3 DirFromCueBallToAiBall(int dirOptionCount,UnitBall chosenBall)
    {
        // define the direction from cueball to chosen ball
        Vector3 dirFromCueBallToAiBall = chosenBall.transform.position - GameplayStatesHandler.instance.cueBall.transform.position;
        Vector3 rightDirRelativeToForwardDirFromCueBallToAiBall = Vector3.Cross(Vector3.up, dirFromCueBallToAiBall);
        //if we are in second loop than create the second direction option
        if (dirOptionCount == 1)
        {
            //define the direction to the right surface of the chosen ball
            Vector3 RightSideSurfaceOfAiBallPos = chosenBall.transform.position + rightDirRelativeToForwardDirFromCueBallToAiBall.normalized * 0.15f;
            dirFromCueBallToAiBall = RightSideSurfaceOfAiBallPos - GameplayStatesHandler.instance.cueBall.transform.position;//dir to the right side of the ai ball
        }
        //if we are in third loop than create the third direction option
        else if (dirOptionCount == 2)
        {
            //define the direction to the left surface of the chosen ball
            Vector3 leftSideSurfaceOfAiBallPos = chosenBall.transform.position + -rightDirRelativeToForwardDirFromCueBallToAiBall.normalized * 0.15f;
            dirFromCueBallToAiBall = leftSideSurfaceOfAiBallPos - GameplayStatesHandler.instance.cueBall.transform.position;//dir to the left side of the ai ball
        }
        return dirFromCueBallToAiBall;
    }
    /// <returns>
    /// true if i'ts allowed direction to hit the cueball
    /// </returns>
    bool IsAllowedDirectionToHit(Vector3 dirToChosenBall)
    {
        if (ThereIsPocketBetweenCueBallToChosenBall(dirToChosenBall)) return false;
        if (ThereIsNotAllowedBallToHItInTheDirToTheChosenBall(dirToChosenBall)) return false;
        return true;

    }
    /// <returns>
    /// return true if there is pocket in the direction to chosen ball
    /// </returns>
    bool ThereIsPocketBetweenCueBallToChosenBall(Vector3 dirFromCueBallToChosenBall)
    {
        float distanceBetweenCueballToChosenBall =
        Vector3.Distance(GameplayStatesHandler.instance.cueBall.transform.position, GameplayStatesHandler.instance.cueBall.transform.position+dirFromCueBallToChosenBall);
        if (Physics.SphereCast(new Ray(GameplayStatesHandler.instance.cueBall.transform.position,dirFromCueBallToChosenBall), radiusToCheck,
        distanceBetweenCueballToChosenBall, LayerMask.GetMask("Pocket"))) return true;
        return false;
    }
    /// <returns>
    /// return true if there is not allowed ball type to hit in 
    /// the direction to the chosen ball
    /// </returns>
    bool ThereIsNotAllowedBallToHItInTheDirToTheChosenBall(Vector3 dirFromCueBallToChosenBall)
    {
        float distanceBetweenCueballToChosenBall =
        Vector3.Distance(GameplayStatesHandler.instance.cueBall.transform.position, GameplayStatesHandler.instance.cueBall.transform.position + new Vector3(dirFromCueBallToChosenBall.x,0,dirFromCueBallToChosenBall.z));
        if (Physics.SphereCast(new Ray(GameplayStatesHandler.instance.cueBall.transform.position, dirFromCueBallToChosenBall), radiusToCheck,out RaycastHit hitHinfo,
         distanceBetweenCueballToChosenBall, LayerMask.GetMask("Ball")))
        {
            UnitBall ballInTheDirectionToChosenBall = hitHinfo.transform.GetComponent<UnitBall>();
            if (ballInTheDirectionToChosenBall.ballType != BallType.computerPlayerBall
                && ballInTheDirectionToChosenBall.ballType != BallType.non) return true;
        }
        return false;
    }
    /// <returns>
    /// random direction to hit the cueball
    /// </returns>
    Vector3 RandomDirToHitTheCueBall()
    {
        Vector3 randomNumInUnitSpher = GameplayStatesHandler.instance.cueBall.transform.position + Random.insideUnitSphere;
        randomNumInUnitSpher.y = GameplayStatesHandler.instance.cueBall.transform.position.y;
        Vector3 randomDirToHitTheCueBall = randomNumInUnitSpher - GameplayStatesHandler.instance.cueBall.transform.position;
        float forceToAddToAiCue = randomDirToHitTheCueBall.magnitude;
        AiHitCueBallState.instance.GetDirAndForceToAddToTheCue(randomDirToHitTheCueBall, forceToAddToAiCue);

        return randomDirToHitTheCueBall;
    }
}


        
   
  
    
   

        


  

            
            
             
