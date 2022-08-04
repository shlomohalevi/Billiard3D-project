
using UnityEngine;

public class AiStatesManeger : MonoBehaviour
{
    public static AiStatesManeger instance;
    public AiPlayerAbstract CurrentAiState;
    AiPlayerAbstract nextAiState;
    private void Awake()
    {
        #region singelton
        if (instance == null)
            instance = FindObjectOfType<AiStatesManeger>();
        #endregion
    }
    private void Start()
    {
        nextAiState = CurrentAiState;
    }
    void Update()
    {
        RunStateMachine();
    }
    private void RunStateMachine()
    {
        if (CurrentAiState == null) return;
        nextAiState = CurrentAiState.runCurrentAiPlayerUpdateState();
        if (nextAiState != CurrentAiState)
        {
            SwitchToTheNextState(nextAiState);
        }
    }
    public void SwitchToTheNextState(AiPlayerAbstract nextCueState)
    {
        CurrentAiState = nextCueState;
    }
}




