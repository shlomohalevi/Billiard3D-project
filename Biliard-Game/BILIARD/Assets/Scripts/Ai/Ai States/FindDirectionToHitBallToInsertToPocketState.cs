using UnityEngine;
using System.Linq;
/// <summary>
/// ai state to find the best direction to hit the cueball
/// </summary>
public class FindDirectionToHitBallToInsertToPocketState : AiPlayerAbstract
{
    public static FindDirectionToHitBallToInsertToPocketState instance;
    [Tooltip("data to determind game difficulty")]
    [SerializeField] DifficultyGameData difficultyLevelData = default;
    [Tooltip("ai direction daviation amount from original hit directon")]
    [SerializeField] float aiDaviationAmountFromOriginalDirection = default;
   
    private void Awake()
    {
        #region
        if (instance == null)
            instance = FindObjectOfType<FindDirectionToHitBallToInsertToPocketState>();
        #endregion
    }
  
    public override AiPlayerAbstract runCurrentAiPlayerUpdateState()
    {
        //get the dirction to hit the cueball
        Vector3 dirFromCueBallToChosenBall = DirToAiBall();
        //if we find direction to hit
        if (dirFromCueBallToChosenBall != default)
        {
            // swich to positioninf ai cue state
            AiPositioningCueState.instance.MethodsToExecuteWhenSwichingToAiPositioningCueState(dirFromCueBallToChosenBall);
            return AiPositioningCueState.instance;
        }
          
        //if we arn't find any dir to hit the cueball swich to defult state to find direction to hit the cueball
        else
        {
            return FindDirectionToHitBallState.instance;
        }
          
    }
           
            
    /// <summary>
    /// calculations to find ball to hit and pocket position to Insert the ball into it
    /// and by it to find the direction to hit the cueball
    /// </summary>
    /// <returns>
    /// direction to hit the cueball
    /// </returns>
    Vector3 DirToAiBall()
    {
        //declaring on max values for calculations to find the position ball to hit
        //the max values refer to the sum between cueball to ball and from ball to pocket
        float maxAngel = default;
        float maxDistance = default;
        float maxDaviation = default;
        // initilize max values
        InithilizMaxValus(ref maxAngel, ref maxDaviation, ref maxDistance);
        //declaring on small data structure that will hold the result of the desire ball end pocket
        (UnitBall ball, Transform pocket) bestBallAndPocketData = default;
        // as long as we are not finding direction to hit the cueball and max angel smaller than certain value
        while (bestBallAndPocketData == default && maxAngel <= 88)
        {
            //try to find the best ball and pocket to insert the ball 
            bestBallAndPocketData = FindBestBallEndHoleCalculation(ref maxAngel, ref maxDaviation, ref maxDistance);
            //if we not find ball and pocket end max angel smaller from certain angel
            if (bestBallAndPocketData == default)
                //increas max values
                IncreasValus(ref maxAngel, ref maxDistance, ref maxDaviation);
        }
        //if we not finding ball and pocket
        if (bestBallAndPocketData == default) return default;
        //getting the desire direction to hit the cue ball
        Vector3 dirToAiBall = CalculateDirection(bestBallAndPocketData);
        //add direction deviation amount to the original direction from cueball to chosen ball according to game difficulty
        dirToAiBall = dirFromCueBallToChosenBallAfterCalculationOfDeviationAmountFromOriginaldirection(dirToAiBall);
        //calculate the force to add to the cue
        float forceToAddToTheCue = CalculateTheForceToAddToTheCueBall(bestBallAndPocketData.ball.transform.position,
        bestBallAndPocketData.pocket.position, GameplayStatesHandler.instance.cueBall.transform.position);
        AiHitCueBallState.instance.GetDirAndForceToAddToTheCue(dirToAiBall, forceToAddToTheCue);
        return dirToAiBall;
    }

    /// <summary>
    /// initilize max values that refer to the sum value between cueball to ball to pockect
    /// </summary>
    void InithilizMaxValus(ref float maxAngel, ref float maxDaviation, ref float maxDistance)
    {
        maxAngel = 45f;
        maxDistance = 4f;
        maxDaviation =  0.5f;
    }
    /// <summary>
    /// increase max values that refer to the sum value between cueball to ball to pockect
    /// </summary>
    void IncreasValus(ref float maxAngel, ref float maxDaviation, ref float maxDistance)
    {
        maxDistance += 2f;
        maxAngel += 2.5f;
        maxDaviation += 0.05f;
    }
     
    /// <returns>
    /// the best ball to hit and pockect to insert the ball 
    /// </returns>
    (UnitBall ball, Transform pocket) FindBestBallEndHoleCalculation(ref float maxAngel, ref float maxDaviation, ref float maxDistance)
    {
        (UnitBall ball, Transform pocket) bestBallAndPocketData = default;
        float lastDistanceBetweenCueBallBallAndHole = Mathf.Infinity;
        float lastangelBetweenCueBallBallAndHole = Mathf.Infinity;
        for (int b = 0; b < GameplayStatesHandler.instance.activeBallsInTheGame.Count; b++)
        {
            for (int p = 0; p < GameplayStatesHandler.instance.pockets.Count; p++)
            {
                UnitBall ball = GameplayStatesHandler.instance.activeBallsInTheGame[b];
                GameObject pocket = GameplayStatesHandler.instance.pockets[p];
                if (!ThisAllowedBallTypeToHit(ball)) continue;
                // calculate where the point the cueball need to hit
                Vector3 positionToHitAiBall = PositionToHitAiBall
                (ball.transform.position, pocket.transform.position);
               
                if (!AllowedHit(positionToHitAiBall,ball.transform.position, pocket.transform.position,maxDaviation)) continue;
               
                float currentAngel = CalculateAngelBetweenDirections( ball.transform.position, pocket.transform.position);
                float currentDistance = CalculateSumDistanceOfTwoDirections(ball.transform.position, pocket.transform.position);
                

                if (currentAngel < lastangelBetweenCueBallBallAndHole && currentDistance < maxDistance ||
                    currentDistance < lastDistanceBetweenCueBallBallAndHole && currentAngel < maxAngel)
                {
                    
                    lastangelBetweenCueBallBallAndHole = currentAngel;
                    lastDistanceBetweenCueBallBallAndHole = currentDistance;
                    bestBallAndPocketData.ball = ball;
                    bestBallAndPocketData.pocket = pocket.transform;

                }
            }
        }
        return bestBallAndPocketData;
    }
    /// <param name="chosenBallPosition"></param>
    /// <param name="pocketPosition"></param>
    /// <returns>
    /// position that needed to hit the ai ball
    /// </returns>
    Vector3 PositionToHitAiBall(Vector3 chosenBallPosition, Vector3 pocketPosition)
    {
        Vector3 dirFromPocketToAiBall = chosenBallPosition - pocketPosition;
        Vector3 dirFomPocketToAiBallWithMagnitudeOfCueBallDiameter = dirFromPocketToAiBall.normalized * UnitBall.radius * 2;
        Vector3 pointToHitAiBall = pocketPosition + dirFromPocketToAiBall + dirFomPocketToAiBallWithMagnitudeOfCueBallDiameter;
        return pointToHitAiBall;
    }
    
    /// <param name="positionToHitAiChosenBall">
    /// the point where the cue ball need to hit the chosen ball
    /// </param>
    /// <param name="chosenBallPosition">
    /// chosen ball position
    /// </param>
    /// <param name="pocketPosition">
    /// chosen pocket position
    /// </param>
    /// <param name="maxDeviation">
    /// max allowed angel deviation
    /// </param>
    /// <returns>
    /// true if this allowed ball to hit
    /// </returns>
    bool AllowedHit(Vector3 positionToHitAiChosenBall,Vector3 chosenBallPosition , Vector3 pocketPosition, float maxDeviation)
    {
        //ray from cueball position to the position where ew need to hit the cueball
        Ray rayFromCueBallPositionToPointPositionToHitAiBall = NewRay(GameplayStatesHandler.instance.cueBall.transform.position, positionToHitAiChosenBall);
        //check if there is pocket in the direction between cueball to chosen ball
        if (ThereIsPocketInTheDirectionBetweenCueBallAndAiHitPoint(rayFromCueBallPositionToPointPositionToHitAiBall,positionToHitAiChosenBall)) return false;
        //check the nearest ball in the direction between cueball to chosen ball is not allowed ball to hit
        if (NearestBallInTheDirectionFromOriginToTargetIsNotAllowedBallTypeToHit(rayFromCueBallPositionToPointPositionToHitAiBall)) return false;
        //check if there is balls in the direction from cueball to chosen ball that deviate the cueball
        if (TooMuchDeviationBetweenOriginToChosenBall(rayFromCueBallPositionToPointPositionToHitAiBall,positionToHitAiChosenBall, maxDeviation)) return false;
        Ray rayFromPocketToChosenBallPosition = NewRay(pocketPosition, chosenBallPosition);
        //check if there is balls in the direction from chosen pocket to chosen ball that deviate the chosenball
        if (TooMuchDeviationBetweenOriginToChosenBall(rayFromPocketToChosenBallPosition,positionToHitAiChosenBall, maxDeviation)) return false;
        //check if the nearest ball in the direction from chosen pocket to chosen ball is not allowed type ball to hit
        if (NearestBallInTheDirectionFromOriginToTargetIsNotAllowedBallTypeToHit(rayFromPocketToChosenBallPosition)) return false;
        return true;
        
    }

    
    /// <returns>
    /// ray from origin to target
    /// </returns>
    Ray NewRay(Vector3 originPosition, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - originPosition;
        Ray ray = new Ray(originPosition, direction);
        return ray;
    }
   
    /// <returns>
    /// true if there is pocket in direction between two positions
    /// </returns>
    bool ThereIsPocketInTheDirectionBetweenCueBallAndAiHitPoint(Ray rayFromCueBallToAiHitPoint,Vector3 chosenBallPosition)
    {
        float distanceBetweenCueballToAiHitPoint = Vector3.Distance(chosenBallPosition, GameplayStatesHandler.instance.cueBall.transform.position);
        if (Physics.SphereCast(rayFromCueBallToAiHitPoint, UnitBall.radius,
        distanceBetweenCueballToAiHitPoint, LayerMask.GetMask("Pocket"))) return true;
        return false;
    }

   
    /// <returns>
    /// true if there is not allowed ball type in the direction
    /// </returns>
    bool NearestBallInTheDirectionFromOriginToTargetIsNotAllowedBallTypeToHit(Ray rayFromOriginToTarget)
    {
        if (Physics.SphereCast(rayFromOriginToTarget, UnitBall.radius, out RaycastHit hitHinfo, Mathf.Infinity, LayerMask.GetMask("Ball")))
        {
            UnitBall ballInTheDirectionBeteweenCueBallToAiHitPoint = hitHinfo.transform.GetComponent<UnitBall>();
            if (!ThisAllowedBallTypeToHit(ballInTheDirectionBeteweenCueBallToAiHitPoint))return true ;
            return false;
        }
        return false;
    }


        /// <summary>
        /// check davietion between origin to target ball 
        /// </summary>
        /// <param name="rayFromOriginToChosenBall">
        /// ray from origin to ball target position
        /// </param>
        /// <param name="maxDeviationFromOriginToTarget">
        /// max deviation that allowed to be between origin target ball
        /// </param>
        /// <returns>
        /// true if there is to much deviation
        /// </returns>
    bool TooMuchDeviationBetweenOriginToChosenBall(Ray rayFromOriginToChosenBall,Vector3 targetPosToHitTheChosenBall, float maxDeviationFromOriginToTarget)
    {
       
        float angelBetweenTwoDirections = default;
        float distanceBetweenOriginToTarget = Vector3.Distance(targetPosToHitTheChosenBall,rayFromOriginToChosenBall.origin);
        //send ray to check how much the balls will deviate the direction from origin to target
        RaycastHit[] ballsInTheDirectionFromOriginToChosenBall = Physics.SphereCastAll(rayFromOriginToChosenBall, UnitBall.radius,
        distanceBetweenOriginToTarget, LayerMask.GetMask("Ball")).
        Where(x => x.transform.GetComponent<UnitBall>().ballType != BallType.cueBall).ToArray();
        //in the first loop check the daviation between direction from origin to chosenball
        //to the direction from origin to each not chosen ball in the direction to chosen ball
        //in the second we loop check the daviation between direction from origin to one not chosen ball in the direction to chosen ball
        // to direction from origin to different one not chosen ball in the directions to chosen ball
        
        for (int i = 0; i < ballsInTheDirectionFromOriginToChosenBall.Length; i++)
        {

            Vector3 dirFromRayOriginToNotChosenBall = ballsInTheDirectionFromOriginToChosenBall[i].transform.position - rayFromOriginToChosenBall.origin;
            angelBetweenTwoDirections = Vector3.Angle(rayFromOriginToChosenBall.direction, dirFromRayOriginToNotChosenBall);
            if (angelBetweenTwoDirections > maxDeviationFromOriginToTarget) return true;
            for (int h = 0; h < ballsInTheDirectionFromOriginToChosenBall.Length; h++)
            {
                Vector3 directionFromOriginToNotChosenBall = rayFromOriginToChosenBall.origin - ballsInTheDirectionFromOriginToChosenBall[i].transform.position;
                Vector3 directionFromOriginToDifferentNotChosenBall = rayFromOriginToChosenBall.origin - ballsInTheDirectionFromOriginToChosenBall[h].transform.position;
                angelBetweenTwoDirections = Vector3.Angle(directionFromOriginToNotChosenBall, directionFromOriginToDifferentNotChosenBall);
                if (angelBetweenTwoDirections > maxDeviationFromOriginToTarget) return true;
            }
        }
        return false;
    }

    /// <returns>
    /// angel between to directions
    /// </returns>
    float CalculateAngelBetweenDirections(Vector3 chosenBallPosition, Vector3 pocketPosition)
    {
        Vector3 dirFromCueBallToChosenBall = chosenBallPosition - GameplayStatesHandler.instance.cueBall.transform.position;
        Vector3 DirFromChosenBallToChosenPocket = pocketPosition - chosenBallPosition;
        float angel = Vector3.Angle(dirFromCueBallToChosenBall, DirFromChosenBallToChosenPocket);
        return angel;
    }
  
    /// <returns>
    /// sum distance of two not normelized directions
    /// </returns>
    float CalculateSumDistanceOfTwoDirections(Vector3 chosenBallPosition, Vector3 chosenPocketPosition)
    {
        float distanceBetweenCueBallToChosenBall = Vector3.Distance(GameplayStatesHandler.instance.cueBall.transform.position, chosenBallPosition);
        float distanceFromChosenBallToChosenPocket = Vector3.Distance(chosenBallPosition, chosenPocketPosition);
        return distanceBetweenCueBallToChosenBall + distanceFromChosenBallToChosenPocket;
    }
    /// <summary>
    /// calculate force direction which needed to aplly to the cueball
    /// according chosen ball and pocket data
    /// </summary>
    /// <returns>
    /// direction to apply force on the cueball
    /// </returns>
    Vector3 CalculateDirection( (UnitBall ChosenBall, Transform chosenPocket) data)
    {
        Vector3 directionFromChosenPocketToChosenBall = data.ChosenBall.transform.position - data.chosenPocket.position;
        Vector3 ballDiameterOffsetInTheDirectionFromPocketToChosenBall = directionFromChosenPocketToChosenBall.normalized * UnitBall.radius * 2;
        Vector3 pointPosititionToHitTheChosenBall = data.chosenPocket.position + directionFromChosenPocketToChosenBall + ballDiameterOffsetInTheDirectionFromPocketToChosenBall;
        Vector3 dirFromCueBallPointToHitTheChosenBall = pointPosititionToHitTheChosenBall - GameplayStatesHandler.instance.cueBall.transform.position;
        return dirFromCueBallPointToHitTheChosenBall;
    }
    
    /// <summary>
    /// calculate the deviation amount of the direction from cueball to chosenball
    /// according the settings of the game difficulty
    /// </summary>
    /// <returns>
    /// the direction from cueball to chosen ball after calculate deviation from original direction
    /// </returns>
    Vector3 dirFromCueBallToChosenBallAfterCalculationOfDeviationAmountFromOriginaldirection(Vector3 originalDirectionFromCueBallToChosenBall)
    {
        // Determines the level of ai accuracy from original direction according the game level difficulty
        aiDaviationAmountFromOriginalDirection = Random.Range(-difficultyLevelData.daviatinAmountFromXaxisOfOriginalDir,
        difficultyLevelData.daviatinAmountFromXaxisOfOriginalDir);
        //define the direction of the deviation vector and multiply this vector by some ammount according game difficulty
        Vector3 daviationVector = Vector3.Cross(Vector3.up, originalDirectionFromCueBallToChosenBall);
        Vector3 dirFromCueBallToChosenBall = originalDirectionFromCueBallToChosenBall += daviationVector.normalized * aiDaviationAmountFromOriginalDirection;
        return dirFromCueBallToChosenBall;
    }
   float CalculateTheForceToAddToTheCueBall(Vector3 chosenBallPosToHit,Vector3 chosenPocketPosToEnterTheChosenBall,Vector3 cueBallPos)
    {
        return Vector3.Distance(chosenBallPosToHit, chosenPocketPosToEnterTheChosenBall)+
            Vector3.Distance(cueBallPos, chosenBallPosToHit);
            
    }
}










   



    
   







        


    




