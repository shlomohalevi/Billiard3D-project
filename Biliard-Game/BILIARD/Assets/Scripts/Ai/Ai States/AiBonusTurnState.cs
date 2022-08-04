using UnityEngine;
/// <summary>
/// state that ai can place the cue ball everywhere on the table to hit the cueball
/// </summary>
public class AiBonusTurnState : AiPlayerAbstract
{
    public static AiBonusTurnState instance;
    
    private void Awake()
    {
        #region singelton
        if (instance == null)
            instance = this;
        #endregion
    }
    public override AiPlayerAbstract runCurrentAiPlayerUpdateState()
    {
        (Vector3 newCueBallPos, Vector3 dir) newPosForCueBallAndDirToHitTheCueBall = NewPosToCueBallAndDirToHit();
        GameplayStatesHandler.instance.cueBall.gameObject.SetActive(true);
        if(newPosForCueBallAndDirToHitTheCueBall!=default)
        {
            GameplayStatesHandler.instance.cueBall.transform.position = newPosForCueBallAndDirToHitTheCueBall.newCueBallPos;
            AiPositioningCueState.instance.MethodsToExecuteWhenSwichingToAiPositioningCueState(newPosForCueBallAndDirToHitTheCueBall.dir);
            return AiPositioningCueState.instance;
        }
        return FindDirectionToHitBallToInsertToPocketState.instance;

    }
     /// <returns>
     /// new position for the cueball and direction to hit
     /// </returns>
    
    (Vector3 NewPosForTheCueBall, Vector3 dirToHit) NewPosToCueBallAndDirToHit()
    {
        (Vector3 aiBallPos, Vector3 pocketPos) bestPocketPosAndAiBallPosToHitData = default;
        float lastSumDistanceBetweenCueBallBallAndPocket = Mathf.Infinity;
        for (int p = 0; p < GameplayStatesHandler.instance.pockets.Count; p++)
        {
            for (int b = 0; b < GameplayStatesHandler.instance.activeBallsInTheGame.Count; b++)
            {
                UnitBall chosenBall = GameplayStatesHandler.instance.activeBallsInTheGame[b];
                GameObject chosenPocket = GameplayStatesHandler.instance.pockets[p];
                //if this allowed hit for the cueball
                if (!AllowedHit(chosenBall,chosenPocket.transform.position)) continue;
                //check if this batter option from the last option we find
                if(DistanceBetweenChosenBallAndChosenPocket(chosenBall, chosenPocket.transform.position) < lastSumDistanceBetweenCueBallBallAndPocket)
                {
                    lastSumDistanceBetweenCueBallBallAndPocket = DistanceBetweenChosenBallAndChosenPocket(chosenBall, chosenPocket.transform.position);
                    bestPocketPosAndAiBallPosToHitData.aiBallPos = chosenBall.transform.position;
                    bestPocketPosAndAiBallPosToHitData.pocketPos = chosenPocket.transform.position;
                }
            }
        }
        //if we find new pos for the cueball and direction to hit the cueball
        if(bestPocketPosAndAiBallPosToHitData!=default)
        {
            //calculate the dir to hit the cueball and the new pos for the cue ball
           ( Vector3 newPosForTheCueBall, Vector3 dirForTheCueBall) newPosToCueBallAndDirToHit = NewCueBallPosAndDirToHitTheCueBallData(bestPocketPosAndAiBallPosToHitData.aiBallPos, bestPocketPosAndAiBallPosToHitData.pocketPos);
            //calculate the force amount that required to add to the cueball
            float forceToHitTheCueBall = CalculateCueBallForceToHit(bestPocketPosAndAiBallPosToHitData.aiBallPos, bestPocketPosAndAiBallPosToHitData.pocketPos,newPosToCueBallAndDirToHit.newPosForTheCueBall);
            AiHitCueBallState.instance.GetDirAndForceToAddToTheCue(newPosToCueBallAndDirToHit.dirForTheCueBall, forceToHitTheCueBall);
            return newPosToCueBallAndDirToHit;
        }
        return default;
    }
        
    /// <returns>
    /// return true if this allowed hit
    /// </returns>
    bool AllowedHit(UnitBall chosenBall,Vector3 pocketPos)
    {
        if (!ThisAllowedBallTypeToHit(chosenBall)) return false;
        if (ThereIsBallBetweenChosenBallToThePocket(chosenBall, pocketPos)) return false;
        if (OverlapingWithAnotherBall(chosenBall, pocketPos)) return false;
        return true;
    }
    /// <summary>
    /// chek if there is another ball between chosen ball to 
    /// chosen pocket that deviate the chosen ball from original direction
    /// </summary>
    bool ThereIsBallBetweenChosenBallToThePocket(UnitBall chosenBall, Vector3 chosenPocket)
    {
        Vector3 dirFromCosenBallToThePocket = chosenPocket - chosenBall.transform.position;
        if (Physics.SphereCast(new Ray(chosenBall.transform.position, dirFromCosenBallToThePocket), UnitBall.radius,
            dirFromCosenBallToThePocket.magnitude, LayerMask.GetMask("Ball"))) return true;
        return false;
    }
   
   /// <summary>
   /// check if we overlaping with another ball in the new position we find
   /// </summary>
  
    bool OverlapingWithAnotherBall(UnitBall chosenBall, Vector3 chosenPocketPos)
    {
      
        Vector3 dirFromPocketToTheBall = chosenBall.transform.position - chosenPocketPos;
        //the little offset needed to place the cue ball not exactly on chosen ball position
        Vector3 dirFromPocketToCueBallWithOffset = dirFromPocketToTheBall+dirFromPocketToTheBall.normalized * 0.35f;
        Vector3 cueBallNewPos = chosenPocketPos + dirFromPocketToCueBallWithOffset;
        cueBallNewPos.y = GameplayStatesHandler.instance.cueBall.transform.position.y;
        //check if we overlaping with another ball
        if (Physics.CheckSphere(cueBallNewPos, UnitBall.radius, LayerMask.GetMask("Ball"))) return true;
        return false;
    }
    
    /// <returns>
    /// sqr magnitude distance
    /// </returns>
    float DistanceBetweenChosenBallAndChosenPocket(UnitBall chosenBall, Vector3 chosenpocketPos) => Vector3.Distance(chosenpocketPos,chosenBall.transform.position);
    /// <summary>
    /// calculate the new position of the cueball and direction according the data we recive 
    /// from the chosen ball and pocket position 
    /// </summary>
    (Vector3 NewPosForTheCueBall, Vector3 dirToHit) NewCueBallPosAndDirToHitTheCueBallData(Vector3 chosenBallPos, Vector3 chosenPocketPos)
    {
        (Vector3 NewPos, Vector3 dirToHit) newPosAndDirToCueBall = default;
        Vector3 dirFromPocketToTheChosenBall = chosenBallPos - chosenPocketPos;
        //calculate the new position for cueball with a little offset to avoid placing the cueball in the chosen ball position
        Vector3 dirFromPocketToCueBallWithOffset = dirFromPocketToTheChosenBall + dirFromPocketToTheChosenBall.normalized * 0.35f;
        newPosAndDirToCueBall.NewPos = chosenPocketPos + dirFromPocketToCueBallWithOffset;
        Vector3 dirToHitTheCueBall = chosenBallPos - newPosAndDirToCueBall.NewPos;
        newPosAndDirToCueBall.dirToHit = dirToHitTheCueBall;
        return newPosAndDirToCueBall;
    }
   
    /// <returns>the force that required to add to the cueball</returns>
    float CalculateCueBallForceToHit(Vector3 chosenBallPos,Vector3 ChosenPocketPos,Vector3 newCueBallPos)
    {
        float disFromChosenBallToChosenPocket= Vector3.Distance(chosenBallPos, ChosenPocketPos);
        float disFromChosenBallToCueBall = Vector3.Distance(chosenBallPos, newCueBallPos);
        return disFromChosenBallToChosenPocket + disFromChosenBallToCueBall;
    }
}
   
          





    
    
     

     

    
   
    
       
