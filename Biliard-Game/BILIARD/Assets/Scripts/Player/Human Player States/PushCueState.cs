using UnityEngine;
/// <summary>
///  responsible of pushing the cue back and forward
/// </summary>
public class PushCueState : HumanPlayerAbstract
{
    //instance reference of this state type
    public static PushCueState instance;
    //value to assign the initial offset between mouse 
    //position to cue position when swiching to this state
     Vector3 initialOffsetBetweenCueToMousePos = default;
    private void Awake()
    {
        #region singelton
        if (instance == null)
            instance = FindObjectOfType<PushCueState>();
        #endregion
    }
    public override HumanPlayerAbstract runCurrentHumanPlayerUpdateState()
    {
        // get the next state
        NextCueState = NewCueState();
        //check if we get differnet state and swich
        //to differnt state according this
        switch (NextCueState)
        {
            case CueState.RotateState:
                {
                    RotateTheCueAndTheCameraState.Instance.MethodsToExecuteWhenSwichingToRotatePlayerCueState();
                    return RotateTheCueAndTheCameraState.Instance;
                }
            case CueState.firstTurnState:
                {
                    BallMovementInFirstTurnState.Instance.MethodsToExecuteWhenSwichingToPlayerFirstTurnState();
                    return BallMovementInFirstTurnState.Instance;
                }
        }
        //get the desire offset to move the cue
        Vector3 offsetToMoveTheCue = OffsetToMoveTheCue(initialOffsetBetweenCueToMousePos);
        //check that the cue in screen boundries
        if (CueInScreenBoundries())
        MoveTheCue(offsetToMoveTheCue);
       // Ceck();
        return this;
    }
    /// <summary>
    /// calculate the inithile offset
    /// between the mouse and the cue when swiching to this state
    /// </summary>
  public void CalculateInithialOffsetBetweenMousePosAndCue()
    {
        Vector3 mousePosInWorldSpace = MousePositionInWorldPosition();//get the mouse position in the world space
        initialOffsetBetweenCueToMousePos = mousePosInWorldSpace - GameplayStatesHandler.instance.playerCue.transform.position;//get only the initial offset vector 
    }
    /// <summary>
    /// calculate the desire offset to move the cue according mouse position
    /// </summary>
    /// <param name="inithialOffsetBetweenCueToMousePos"></param>
    /// <returns>
    /// the desire offset to move the cue
    /// </returns>
    Vector3 OffsetToMoveTheCue(Vector3 inithialOffsetBetweenCueToMousePos)
    {
        //get mouse position relative to world space
        Vector3 MousePoseInWorldSpace = MousePositionInWorldPosition();
        //get the  offset between mouse position and the initiale mouse position every fram 
        //(the offset between mouse position to the cue when we swich to this state)
        Vector3 offsetBetweenCueToMousePos = MousePoseInWorldSpace - inithialOffsetBetweenCueToMousePos;
        //the direction and desire offset to move the cue
        Vector3 dirFromCueToMousePosition = offsetBetweenCueToMousePos -GameplayStatesHandler.instance. playerCue.transform.position;
        // project the direction vector to move the cue onto cue forward direction vector
        Vector3 offsetToMoveTheCue = Vector3.Project(dirFromCueToMousePosition,GameplayStatesHandler.instance. playerCue.transform.forward);
        return offsetToMoveTheCue;
    }
    /// <summary>
    /// check that the cue w'ont goes out off screen
    /// </summary>
    /// <returns>
    /// true if cue in screen boundries
    /// </returns>
    bool CueInScreenBoundries() => Input.mousePosition.y > 2;
    /// <summary>
    /// move the cue by offset
    /// </summary>
    /// <param name="offsetToMoveTheCue"></param>
    private void MoveTheCue(Vector3 offsetToMoveTheCue)
    {
       GameplayStatesHandler.instance. playerCue.transform.position += offsetToMoveTheCue;
    }

    
    public void MethodsToExcecuteWhenSwichingToPlayerPushCueState()
    {
        instance.CalculateInithialOffsetBetweenMousePosAndCue();
    }
  
}
  



   
        
 
   
       
            
           

           

        
    
            

            
     
   

    
    
       

  

   
        
   


