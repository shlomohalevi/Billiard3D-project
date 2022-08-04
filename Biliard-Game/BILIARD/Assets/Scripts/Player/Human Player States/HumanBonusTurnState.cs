using Cinemachine;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// state to move the ball by the mouse everywhere
/// </summary>
public class HumanBonusTurnState : HumanPlayerAbstract
{
    public static HumanBonusTurnState instance;
    [Header("cueball min and max pos valuse")]
    [SerializeField] float minX = default, maxX = default;
    [SerializeField] float minZ = default, maxZ = default;
    [Header("cueball components")]
    [SerializeField] NavMeshAgent cueBallAgent = default;
    [SerializeField] Rigidbody cueBallRifidbody = default;
    //magnitude value when we swiching to this state
    float MagnitudeBetweenCueBallToCamera = default;
    //magnitude value
    float MagnitudeBetweenCueBallToCue = 1.5f;
    // if false we cant switch to this state
    public bool isBonusTurn = false;
    private void Awake()
    {
        #region singelton
        if (instance == null)
            instance = this;
        #endregion
    }
 
    /// <summary>
    /// bonus turn state
    /// </summary>
    /// <returns>
    /// this state
    /// </returns>
    public override HumanPlayerAbstract runCurrentHumanPlayerUpdateState()
    {
        // if we are trying to move the cueball
        if (WeTringMoveTheCueBall(out Vector3 mouseHitPosition))
        {
            //move the cueball to where we are trying to move it
            MoveCueBallAcordingMouseAlongXAndZAxis(mouseHitPosition);
            //move the camera
            CameraMovement();
        }
        //else if not
        else if (WeAreNotTryingToMoveTheCueBall())
        {
            //reset the navmesh of the cueball
            ResetNavmeshAgentAndActiveCueBallDinamicRigidbodyOfCueball();
            //positining the player cue 
            RepositionTheCue();
            ActivateCue();
            //switch to the next state and invok the desire events
            RotateTheCueAndTheCameraState.Instance.MethodsToExecuteWhenSwichingToRotatePlayerCueState();
            NextCueState = CueState.RotateState;
            return RotateTheCueAndTheCameraState.Instance;

        }

        return this;
    }
    /// <param name="mouseHitPosition">
    /// mouse hit position in world space if we are trying move the cueball
    /// </param>
    /// <returns>
    /// true if we are trying move the cueball
    /// </returns>
    bool WeTringMoveTheCueBall(out Vector3 mouseHitPosition)
    {
        //if we are trying to move the cueball
        if (Input.GetMouseButton(0))
        {
            //convert mouse hit to world space 
            mouseHitPosition = MousePositionInWorldPosition();
            return true;
        }
        mouseHitPosition = default;
        return false;
    }
    /// <returns>
    /// true when mouse button releasing
    /// </returns>
    bool WeAreNotTryingToMoveTheCueBall() => Input.GetMouseButtonUp(0);
    /// <summary>
    /// move the cueball acorrding mouse position 
    /// exept y axis with clamping values
    /// </summary>
    /// <param name="mouseHitHinfoPos">
    /// mouse position in world space
    /// </param>
    void MoveCueBallAcordingMouseAlongXAndZAxis(Vector3 mouseHitHinfoPos)
    {   //calculate mouse position with clamping values
        float xMousePosWithClamp = Mathf.Clamp(mouseHitHinfoPos.x, minX, maxX);
        float zMousePosWithClamp = Mathf.Clamp(mouseHitHinfoPos.z, minZ, maxZ);
        //define cueball movement according mouse position with clamping values
        Vector3 cueBallPosWithMouseXPositionAndZpos = new Vector3(xMousePosWithClamp, GameplayStatesHandler.instance.cueBall.transform.position.y, zMousePosWithClamp);
        //move the cueball 
        cueBallAgent.SetDestination(cueBallPosWithMouseXPositionAndZpos);
    }
    /// <summary>
    /// camera movement calculations to follow the cueball
    /// </summary>
    void CameraMovement()
    {
        //the direction vector between camera and cueball every frame
        Vector3 currentVectorBetweenCameraToCueBall = BlendingVcamsHandler.instance.doNothingVcam.transform.position - GameplayStatesHandler.instance.cueBall.transform.position;
        //calculate the new position of the camera exept y position acorrding to the current normlized direction and 
        //the magnitude betweeen the camera and the cueball when we switching to this state
        Vector3 cameraNewPos = GameplayStatesHandler.instance.cueBall.transform.position + currentVectorBetweenCameraToCueBall.normalized
        * MagnitudeBetweenCueBallToCamera;
        cameraNewPos.y = BlendingVcamsHandler.instance.doNothingVcam.transform.position.y;
        BlendingVcamsHandler.instance.doNothingVcam.transform.position = cameraNewPos;
        //camera rotation
        BlendingVcamsHandler.instance.doNothingVcam.transform.LookAt(new Vector3(GameplayStatesHandler.instance.cueBall.transform.position.x, Vector3.up.y * 0.7f, GameplayStatesHandler.instance.cueBall.transform.position.z));
    }
    /// <summary>
    /// reposition the cue calculations 
    /// </summary>
    void RepositionTheCue()
    {
        //take the direction from cue ball to the camera exept y direction
        Vector3 dir = new Vector3(BlendingVcamsHandler.instance.doNothingVcam.transform.position.x,GameplayStatesHandler.instance. cueBall.transform.position.y, BlendingVcamsHandler.instance.doNothingVcam.transform.position.z) -GameplayStatesHandler.instance. cueBall.transform.position;
        //clamp this direction to the magnitude between cue and cueball
        dir = Vector3.ClampMagnitude(dir, MagnitudeBetweenCueBallToCue);
        // define new position to the cue 
        Vector3 cuenewPos = GameplayStatesHandler.instance. cueBall.transform.position + dir;
        cuenewPos.y = 0.5f;
        GameplayStatesHandler.instance. playerCue.transform.position = cuenewPos;
        //define rotation ti cue
        GameplayStatesHandler.instance.playerCue.transform.LookAt(GameplayStatesHandler.instance.cueBall.transform.position);
    }
    /// <summary>
    /// enable cueball navmesh agent component
    /// </summary>
    void EnableCueBallNavmeshAgent() => cueBallAgent.enabled = true;
    /// <summary>
    /// deactivate cue gameobject
    /// </summary>
    void DeactivateCue() => GameplayStatesHandler.instance.playerCue.transform.gameObject.SetActive(false);
    /// <summary>
    /// disable cue ball dinamic rigidbody component
    /// </summary>
    void DisableCueBallDinamicRigidbody() => cueBallRifidbody.isKinematic = true;
    /// <summary>
    /// activate cue gameobject
    /// </summary>
    void ActivateCue() => GameplayStatesHandler.instance.playerCue.gameObject.SetActive(true);
    /// <summary>
    /// activate cue ball dinamic rigidbody component
    /// </summary>
    void activateCueBallDinamicRigidbody() => cueBallRifidbody.isKinematic = false;
    /// <summary>
    /// reset navmesh agent component properties and 
    /// activate dinamic rigidbody of cueball component
    /// </summary>
    void ResetNavmeshAgentAndActiveCueBallDinamicRigidbodyOfCueball()
    {
        cueBallAgent.velocity = Vector3.zero;
        cueBallAgent.ResetPath();
        cueBallAgent.enabled = false;
        activateCueBallDinamicRigidbody();
    }
    /// <summary>
    /// calculate the magnitude between cueball to camera
    /// </summary>

    void CalculateMagnitudeBetweenCueBallToCamera() => MagnitudeBetweenCueBallToCamera = Vector3.Distance(BlendingVcamsHandler.instance.doNothingVcam.transform.position, GameplayStatesHandler.instance.cueBall.transform.position);

    public void MethodsToExecuteWhenSwichingToPlayerBonusState()
    {
        TrajectoryLine.instance.RemoveVisualAimingMarks();
        EnableCueBallNavmeshAgent();
        DeactivateCue();
        DisableCueBallDinamicRigidbody();
        CalculateMagnitudeBetweenCueBallToCamera();
    }
}



  
   



       

        
           
               
        

            
         
        
    
  
            
            
            
                


       





       



 
   
































