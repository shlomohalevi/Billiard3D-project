using System.Collections;
using UnityEngine;
using System.Linq;
using Cinemachine;
using System;

/// <summary>
/// this state is responsible to the initial positioning 
/// of the cue the camera and the cueBall
/// </summary>
public class HumanPositioningState : HumanPlayerAbstract
{
    public static HumanPositioningState instance;
    [Tooltip("initial y pos of the cue")]
    [SerializeField] float cuePosOnyAxis = default;
    // varible to assign the nearest ball to the cueball
    UnitBall nearestBallToTheCueBall = default;
    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<HumanPositioningState>();
    }

    public override HumanPlayerAbstract runCurrentHumanPlayerUpdateState()
    {
        //activate the first virtual camera and the cue objects
        GameplayStatesHandler.instance.playerCue.gameObject.SetActive(true);
        BlendingVcamsHandler.instance.doNothingVcam.gameObject.SetActive(true);
        //find the closest ball to the cue ball
        nearestBallToTheCueBall = FindNearestBallToCurBall();
        //draw the hit point mark where the trajectory line from the cueball intersact with the nearest ball and draw
        //this mark point depending the ball type of the nearest ball
        HitPointMark.instance.DrawHitPoint(nearestBallToTheCueBall.ballType);
        //inithlize cue position and rotation
        SetCuePositionAndRotation(nearestBallToTheCueBall, GameplayStatesHandler.instance.cueBall, GameplayStatesHandler.instance.playerCue);
        //inithilize camera position and rotation
        SetCameraPositionAndRotation(GameplayStatesHandler.instance.playerCue, GameplayStatesHandler.instance.cueBall,BlendingVcamsHandler.instance.doNothingVcam);
        //wait until the camera reaches its position
        StartCoroutine(WaitUntilCameraEndBlendingMotionAndThanChangeState());
        return null;
    }
        


    /// <summary>
    /// order the balls by distance to the cueball in ascending order
    /// </summary>
    /// <returns>
    /// the nearest ball to the cueball
    /// </returns>

    UnitBall FindNearestBallToCurBall()
    {
        UnitBall nearestBall = GameplayStatesHandler.instance.activeBallsInTheGame.OrderBy(x => Vector3.Distance(GameplayStatesHandler.instance.cueBall.transform.position, x.transform.position))
       .Where(x => x != GameplayStatesHandler.instance.cueBall).First();
        return nearestBall;
    }
    /// <summary>
    /// calculations to set initial cue position and rotation
    /// </summary>
    /// <param name="nearestBallToCueBall"></param>
    /// <param name="cueBall"></param>
    /// <param name="cue"></param>

    public void SetCuePositionAndRotation(UnitBall nearestBallToCueBall, UnitBall cueBall, GameObject cue)
    {
        //calculate the dir from cueball to his closest ball
        Vector3 dirFromCueBallToNearestBall = nearestBallToCueBall.transform.position - cueBall.transform.position;
        //normlize it to get only the dir without the original magnitude
        dirFromCueBallToNearestBall = dirFromCueBallToNearestBall.normalized;
        //inithilze some magnitude offset;
        float offset = 1.5f;
        //create the new pos for the cue  
        Vector3 newPosition = cueBall.transform.position - dirFromCueBallToNearestBall.normalized * offset;
        newPosition.y = cuePosOnyAxis;
        cue.transform.position = newPosition;
        //set rotation for the cue 
        cue.transform.LookAt(cueBall.transform);
    }
    /// <summary>
    /// calcultions to set initial camera position and rotation
    /// </summary>
    /// <param name="cue"></param>
    /// <param name="cueBall"></param>
    /// <param name="vcam1"></param>
    public  void SetCameraPositionAndRotation(GameObject cue, UnitBall cueBall, CinemachineVirtualCamera vcam1)
    {
        //set new position for the camera
        Vector3 cameraPosition = new Vector3(cueBall.transform.position.x, 0, cueBall.transform.position.z)
          - new Vector3(cue.transform.forward.x, 0, cue.transform.forward.z) * 3.5f;
        cameraPosition.y = 1.8f;
        vcam1.transform.position = cameraPosition;
        //set new rotation for the camera 
        vcam1.transform.LookAt(new Vector3(cueBall.transform.position.x, Vector3.up.y * 0.7f, cueBall.transform.position.z));
    }
    /// <summary>
    /// wait until the camera reaches its position end than swich to another state
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitUntilCameraEndBlendingMotionAndThanChangeState()
    {
        //dont check if the camera ended its blending befour the camera begining its blending
        yield return new WaitForEndOfFrame();
        //if the camera reaches its position
        yield return new WaitUntil(() => Camera.main.GetComponent<CinemachineBrain>().IsBlending == false);
        NextCueState = CueState.RotateState;
        RotateTheCueAndTheCameraState.Instance.MethodsToExecuteWhenSwichingToRotatePlayerCueState();
        HumanPlayerStateManeger.instance.SwitchToTheNextState(RotateTheCueAndTheCameraState.Instance);
    }
  
}
  

       

   





 
    
 
    

    

    
    


