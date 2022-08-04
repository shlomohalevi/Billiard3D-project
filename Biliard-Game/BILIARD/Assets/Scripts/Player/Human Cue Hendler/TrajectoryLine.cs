using UnityEngine;
/// <summary>
/// responsible class on the trajectory line 
/// from cueball to hit point
/// </summary>
public class TrajectoryLine : MonoBehaviour
{
    public static TrajectoryLine instance;
    [Tooltip("trajectory line from cue ball to hit point")]
    [SerializeField] LineRenderer lr1 = default;
    [Tooltip("trajectory line from hit point to reflected hit point")]
    [SerializeField] LineRenderer lr2 = default;
    [SerializeField] MeshRenderer cueBallMark = default;
    [SerializeField] MeshRenderer hitPointMark = default;
    [SerializeField] LayerMask ballAndWallLayers = default;
    [SerializeField] float radius = 0.11f;
    [SerializeField] float lnYpos = default;
    private void Awake()
    {
        #region singelton
        if (instance == null)
            instance = FindObjectOfType<TrajectoryLine>();
        #endregion
    }
  
    void Update()
    {
       if (HumanPlayerAbstract.NextCueState != CueState.RotateState && HumanPlayerAbstract.NextCueState != CueState.firstTurnState) return;

        RaycastHit HitDataOfTrajectoryLineFromCueBall;
        if (WeHitBallOrTableWallWithRaycastFromTHeCueBall(out HitDataOfTrajectoryLineFromCueBall))
        {//calculate and set positions from cueball to hit point
            (Vector3 startLnPos,  Vector3 endLnPos) PositionsForLnFromCueBallToHitPoint =
            CalculatePositionsFromCueBallToHitPoint(HitDataOfTrajectoryLineFromCueBall);
            SetStartAndEndLnPos(PositionsForLnFromCueBallToHitPoint.startLnPos, PositionsForLnFromCueBallToHitPoint.endLnPos,lr1);
            SetHitPointMarkPosition(PositionsForLnFromCueBallToHitPoint.endLnPos);

            if (WeHitGivenLayer(HitDataOfTrajectoryLineFromCueBall,"Ball"))
            {//calculate ans set the reflaction vector from ball we hit
                (Vector3 startLnPosOfReflactionVectorFromHitBall, Vector3 endLnPosOfReflactionVectorFromHitBall)
                ReflactionVectorPositionsfromHitBallData = StartAndEndPointOfReflectionVectorFromHitBall(HitDataOfTrajectoryLineFromCueBall);//data of second vector
                SetStartAndEndLnPos(ReflactionVectorPositionsfromHitBallData.startLnPosOfReflactionVectorFromHitBall,
                ReflactionVectorPositionsfromHitBallData.endLnPosOfReflactionVectorFromHitBall,lr2);
            }
            else if (WeHitGivenLayer(HitDataOfTrajectoryLineFromCueBall,"WallColliders"))
            {//calculate and set the reflaction vector from the wall we hit
                (Vector3 startLnPosOfReflactionVectorFromHitWall, Vector3 endLnPosOfReflactionVectorFromHitWall) ReflactionPositionsfromHitWallData =
                StartAndEndPointOfReflectionVectorFromHitWall(HitDataOfTrajectoryLineFromCueBall.normal,
                HitDataOfTrajectoryLineFromCueBall.point);
                SetStartAndEndLnPos(ReflactionPositionsfromHitWallData.startLnPosOfReflactionVectorFromHitWall,
                ReflactionPositionsfromHitWallData.endLnPosOfReflactionVectorFromHitWall,lr2);
            }
        }
    }
    /// <returns> true if we hit somthing with raycast from the cueball</returns>
    bool WeHitBallOrTableWallWithRaycastFromTHeCueBall(out RaycastHit hitDataOftrajectoryLineFromCueBallOutParam)
    {
        Ray rayFromCueBallToHitPoint = new Ray(GameplayStatesHandler.instance.cueBall.transform.position, new Vector3(transform.forward.x, 0, transform.forward.z));
        if (Physics.SphereCast(rayFromCueBallToHitPoint, radius, out hitDataOftrajectoryLineFromCueBallOutParam, Mathf.Infinity, ballAndWallLayers))
            return true;
        return false;
    }
        
    /// <returns>start position of trajectory line from cueBall and the trajectory vector from cueball</returns>
        
    (Vector3 startLnPos, Vector3 endElPos)
        CalculatePositionsFromCueBallToHitPoint(RaycastHit hitDataOfTrajectoryLineFromCueBall)
    {
        Vector3 startLnPos = new Vector3(GameplayStatesHandler.instance.cueBall.transform.position.x, lnYpos, GameplayStatesHandler.instance.cueBall.transform.position.z);
        Vector3 endLnPos = new Vector3(hitDataOfTrajectoryLineFromCueBall.point.x, lnYpos, hitDataOfTrajectoryLineFromCueBall.point.z);
        Vector3 lnVector = endLnPos - startLnPos;
        Vector3 projectionVectorFromCueForwardOntoLnVector = Vector3.Project(lnVector, new Vector3(transform.forward.x, 0, transform.forward.z));
        endLnPos = startLnPos + projectionVectorFromCueForwardOntoLnVector;
        return (startLnPos, endLnPos);
    }
    void SetStartAndEndLnPos(Vector3 startLnPoint, Vector3 endLnPos,LineRenderer lineRenderer)
    {
        lineRenderer.SetPosition(0, new Vector3(startLnPoint.x,lnYpos,startLnPoint.z));
        lineRenderer.SetPosition(1, new Vector3(endLnPos.x, lnYpos, endLnPos.z));
    }
   
    /// <param name="hitData">object we hit</param>
    /// <param name="layerName">layer name of what we hit</param 
    bool WeHitGivenLayer(RaycastHit hitData,string layerName) => hitData.transform.gameObject.layer == LayerMask.NameToLayer(layerName);
    /// <summary>calculate start end end point of reflection vector from hit ball </summary>
    (Vector3 startpoint, Vector3 endPoint) StartAndEndPointOfReflectionVectorFromHitBall(RaycastHit hitBallData)
    {
        Vector3 dirFromHitballPointToCenterHitball = hitBallData.transform.position
        - hitBallData.point;// dir from hit ball to reflaction vector
        dirFromHitballPointToCenterHitball = dirFromHitballPointToCenterHitball.normalized;
        Ray reflactionDirFromHitBall = new Ray(hitBallData.transform.position, dirFromHitballPointToCenterHitball);//create ray represent the reflection direction
        if (Physics.SphereCast(reflactionDirFromHitBall, radius, out RaycastHit HitDataOfReflactionVector,Mathf.Infinity,ballAndWallLayers))
            return (hitBallData.transform.position, HitDataOfReflactionVector.point);//return start and end positions for the reflaction line renderer
        return default;
    }
    void SetHitPointMarkPosition(Vector3 endLnPos)
    {
        hitPointMark.transform.position = new Vector3(endLnPos.x,lnYpos+0.005f,endLnPos.z);
    }
    /// <summary>calculate start and end positions of trajectory reflection vector from wall</summary>
    (Vector3 startpoint, Vector3 endPoint) StartAndEndPointOfReflectionVectorFromHitWall
    ( Vector3 hitWallNormal, Vector3 startPointOfReflactionVectorFromWall)
    {
        Vector3 dirFromCueBallToHitWallPoint = startPointOfReflactionVectorFromWall - GameplayStatesHandler.instance.cueBall.transform.position;
        Vector3 reflectVectorfromWall = Vector3.Reflect(dirFromCueBallToHitWallPoint, hitWallNormal);
        Ray rayFromReflactionStartPoint = new Ray(startPointOfReflactionVectorFromWall, reflectVectorfromWall);
        if (Physics.SphereCast(rayFromReflactionStartPoint,radius, out RaycastHit hitDataOfReflactionVectorData,ballAndWallLayers))
            return (startPointOfReflactionVectorFromWall, hitDataOfReflactionVectorData.point);
        return default;
    }
   public void AddLnPositionAimingMarksAndSetCueBallAndHitMarksPositionAndRotation()
    {
        if (cueBallMark.enabled) return;
        lr1.positionCount = 2;
        lr2.positionCount = 2;
        Vector3 cueBallMarkPosition = cueBallMark.gameObject.transform.position = new Vector3(GameplayStatesHandler.instance.cueBall.transform.position.x,
        -0.11f, GameplayStatesHandler.instance.cueBall.transform.position.z);
        Quaternion cueBallRotation = Quaternion.Euler(90, 0, 0);
        cueBallMark.transform.SetPositionAndRotation(cueBallMarkPosition, cueBallRotation);
        hitPointMark.transform.rotation = Quaternion.Euler(90, 0, 0);
        cueBallMark.enabled = true;
        hitPointMark.enabled = true;
    }
   public void RemoveVisualAimingMarks()
    {
        if (!cueBallMark.enabled) return;
        lr1.positionCount = 0;
        lr2.positionCount = 0;
        cueBallMark.enabled = false;
        hitPointMark.enabled = false;
    }
}





   


   

       



   


   


    

        
        


        
        


    
    
    

        
        

   
   











  

        
    

