using UnityEngine;

/// <summary>
/// state that responsible to position the ai cue 
/// after we find desire direction to hit the cueball
/// </summary>
public class AiPositioningCueState : AiPlayerAbstract
{
    public static AiPositioningCueState instance;
    Vector3 dirFromCueballToAiChosenBall = default;
    [SerializeField] float aiCueYPosition = 0.4f;
    
    private void Awake()
    {
        #region
        if (instance == null)
            instance = FindObjectOfType<AiPositioningCueState>();
        #endregion
    }
    public override AiPlayerAbstract runCurrentAiPlayerUpdateState()
    {
        PositioningAiCue();
        AiHitCueBallState.instance.MethodsToEcecuteWhenSwichingToAiHitState(GameplayStatesHandler.instance.aiCue.transform.forward);
        //change state to hit the cueball
        return AiHitCueBallState.instance;
    }
    /// <summary>
    /// set position and rotation to the cue
    /// </summary>
    void PositioningAiCue()
    {
        Vector3 newAiCuePos = GameplayStatesHandler.instance.cueBall.transform.position - new Vector3(dirFromCueballToAiChosenBall.x, 0, dirFromCueballToAiChosenBall.z).normalized * 2f;
        newAiCuePos.y = aiCueYPosition;
        GameplayStatesHandler.instance.aiCue.transform.position = newAiCuePos;
        GameplayStatesHandler.instance.aiCue.transform.LookAt(GameplayStatesHandler.instance.cueBall.transform.position);
    }
 
    /// <summary>
    /// assign direction to hit the cueball according to the direction we recive in the callback
    /// </summary>
    /// <param name="dirFromCueballToAiChosenBall">
    /// the direction we find to hit the cueball
    /// </param>
    void DirFromCueballToAiChosenBall(Vector3 dirFromCueballToAiChosenBall) => this.dirFromCueballToAiChosenBall = dirFromCueballToAiChosenBall;
    public void MethodsToExecuteWhenSwichingToAiPositioningCueState(Vector3 dirFromCueballToAiChosenBall)
    {
        DirFromCueballToAiChosenBall(dirFromCueballToAiChosenBall);
    }
}

  













