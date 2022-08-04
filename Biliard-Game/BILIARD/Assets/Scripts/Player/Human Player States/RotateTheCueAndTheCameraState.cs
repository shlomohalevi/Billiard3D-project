using Cinemachine;
using UnityEngine;


/// <summary>
/// this state is responsible to roatate the cue and the camera around the cueball
/// </summary>

public class RotateTheCueAndTheCameraState :HumanPlayerAbstract
{   //reference to this state to assign singelton
    public static RotateTheCueAndTheCameraState Instance;
    private void Awake()
    {
        #region singelton
        if (Instance == null)
            Instance = FindObjectOfType<RotateTheCueAndTheCameraState>();
        #endregion
    }
    public override HumanPlayerAbstract runCurrentHumanPlayerUpdateState()
    {
        //get the next state
        NextCueState = NewCueState();
        //check if we get differnet state and swich
        //to differnt state according this
        switch(NextCueState)
        {
            case CueState.pushState:
                {
                    PushCueState.instance.MethodsToExcecuteWhenSwichingToPlayerPushCueState();
                    return PushCueState.instance;
                }
            case CueState.bonusTurnState:
                {
                    HumanBonusTurnState.instance.MethodsToExecuteWhenSwichingToPlayerBonusState();
                    return HumanBonusTurnState.instance;
                }
            case CueState.firstTurnState:
                {
                    BallMovementInFirstTurnState.Instance.MethodsToExecuteWhenSwichingToPlayerFirstTurnState();
                    return BallMovementInFirstTurnState.Instance;
                }
                
        }
        RotataeTheCueAndTheCameraAroundTheCueBall(GameplayStatesHandler.instance.playerCue.transform,GameplayStatesHandler.instance. cueBall, BlendingVcamsHandler.instance.doNothingVcam);
        return this;
        
    }
           
  /// <summary>
  /// rotate the cue and the camera horizontally around the cue ball
  /// </summary>
  /// <param name="cueTransform"></param>
  /// <param name="cueBall"></param>
  /// <param name="Vcam1"></param>
    public  void RotataeTheCueAndTheCameraAroundTheCueBall(Transform cueTransform, UnitBall cueBall, CinemachineVirtualCamera Vcam1)
    {
        if (Input.GetMouseButton(0))
        {
            // rotation amount to rotate
            float rotationAmount = Input.GetAxis("Mouse X") * 50 * Time.deltaTime;
            //rotate the cue and the camera around the cueball
            cueTransform.RotateAround(cueBall.transform.position, Vector3.up, rotationAmount);
            Vcam1.transform.RotateAround(cueBall.transform.position, Vector3.up, rotationAmount);
        }
    }
    public void MethodsToExecuteWhenSwichingToRotatePlayerCueState()
    {
        //set trajectory line
        TrajectoryLine.instance.AddLnPositionAimingMarksAndSetCueBallAndHitMarksPositionAndRotation();
    }
}
  
   
 
   
   
    






                 
                    
                    
            
   


