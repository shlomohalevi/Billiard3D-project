using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayerStatesManeger : MonoBehaviour
{
    public static AiPlayerStatesManeger AiPlayerStatesManegerInstance;
    public AiPlayerAbstract CurrentAiState;
    void Start()
    {
        if (AiPlayerStatesManegerInstance == null)
            AiPlayerStatesManegerInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        RunStateMachine();
    }
    private void RunStateMachine()
    {
        if (CurrentAiState == null) return;

        AiPlayerAbstract nextAiState = CurrentAiState.runCurrentAiPlayerUpdateState();
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
