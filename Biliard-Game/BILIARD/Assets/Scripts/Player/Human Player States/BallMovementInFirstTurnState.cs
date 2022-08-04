using UnityEngine;


/// <summary>
/// the state and movement in the first turn
/// </summary>
public class BallMovementInFirstTurnState : HumanPlayerAbstract
{
    //instance reference of this state type
    public static BallMovementInFirstTurnState Instance;
    [Tooltip("cue ball minimum movement boundry along xAxis")]
    [SerializeField] float minX = default;
    [Tooltip("cue ball maximum movement boundry along xAxis")]
    [SerializeField] float maxX = default;
    //the manitude offset between cue and cueball when we swiching to this state
    Vector3 initialOffsetBetweenCueBallToCue = default;
    //the vector offset between the camera and cue when we swiching to this state
    Vector3 initialOffsetBetweenCameraToCue = default;
    public bool isFirstTurn = true;
    private void Awake()
    {
        #region singelton
        if (Instance == null)
            Instance = this;
        #endregion
    }
    public override HumanPlayerAbstract runCurrentHumanPlayerUpdateState()
    {
        // get the next state
        NextCueState =  NewCueState();
        //check if we get differnet state and swich
        //to differnt state according this
        switch (NextCueState)
        {
            case CueState.RotateState:
                {
                    RotateTheCueAndTheCameraState.Instance.MethodsToExecuteWhenSwichingToRotatePlayerCueState();
                    return  RotateTheCueAndTheCameraState.Instance;
                }
            case CueState.pushState:
                {
                    PushCueState.instance.MethodsToExcecuteWhenSwichingToPlayerPushCueState();
                    return PushCueState.instance;
                }
        }
        if (Input.GetMouseButton(0))
        {
            //get mouse pos in world space
            Vector3 mousePosHinfo = MousePositionInWorldPosition();
            //move the cueball along xAxis
            MoveCueBallAccordingMousePosAlongXAxis(mousePosHinfo);
            //move the cue along xAxis
            MoveCueAcoordingCueBallXAxis();
            //move the camera along xAxis
            CalculateCameraMovementAccordingMouseXaxis();
        }
        
        return this;
    }
    /// <summary>
    /// move the cueball with maximum and minimum clamping position valus along xAxis
    /// </summary>
    /// <param name="mousePosinfo"></param>
    void MoveCueBallAccordingMousePosAlongXAxis(Vector3 mousePosinfo)
    {   // get mouse x position value with clamping
        float MousPosAlongXaxisWithClamp = Mathf.Clamp(mousePosinfo.x, minX, maxX);
        //set new position vector, cueball position with mouse x position info
        Vector3 cueBallPosWithMouseXPosition = new Vector3(MousPosAlongXaxisWithClamp,GameplayStatesHandler.instance. cueBall.transform.position.y, GameplayStatesHandler.instance.cueBall.transform.position.z);
        //set cue ball new position
      GameplayStatesHandler.instance.  cueBall.transform.position = Vector3.MoveTowards(GameplayStatesHandler.instance.cueBall.transform.position, cueBallPosWithMouseXPosition, 2 * Time.deltaTime);
    }
   /// <summary>
   /// calculation of cue movement along xAxis
   /// </summary>
    void MoveCueAcoordingCueBallXAxis()
    {
        //set new cue position each frame
        Vector3 cueNewPosition =GameplayStatesHandler.instance. cueBall.transform.position + initialOffsetBetweenCueBallToCue;
        GameplayStatesHandler.instance. playerCue.transform.position = cueNewPosition;
    }
    /// <summary>
    /// calculations of camera movement along xAxis
    /// </summary>
    void CalculateCameraMovementAccordingMouseXaxis()
    {  
        //set new camera position each frame
        Vector3 vcam1NewPosition = GameplayStatesHandler.instance.playerCue.transform.position + initialOffsetBetweenCameraToCue;
        BlendingVcamsHandler.instance.doNothingVcam.transform.position = vcam1NewPosition;
    }

 
   
    /// <summary>
    /// the initial offset magnitude between 
    /// cue to cueball when swiching to this state
    /// </summary>
     void UpdateOffsetMagnitudeBeteweenCueBallToBall() => initialOffsetBetweenCueBallToCue = GameplayStatesHandler.instance. playerCue.transform.position - GameplayStatesHandler.instance. cueBall.transform.position;

    /// <summary>
    /// the initial offset vector between 
    /// cue to camera when swiching to this state
    /// </summary>
     void UpdateOffsetMagnitudeBeteweenCueToVcam1() => initialOffsetBetweenCameraToCue = BlendingVcamsHandler.instance.doNothingVcam.transform.position - GameplayStatesHandler.instance. playerCue.transform.position;
        
        
    public void MethodsToExecuteWhenSwichingToPlayerFirstTurnState()
    {
        UpdateOffsetMagnitudeBeteweenCueBallToBall();
        UpdateOffsetMagnitudeBeteweenCueToVcam1();
    }
}
   
  




      


         




            
    
           
       
         

  

   


   











               
        


  
    
    

                
                

       



            
        
     

