
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// this class is resposible to run and switch the states 
/// of the human player 
/// </summary>
public class HumanPlayerStateManeger : MonoBehaviour
{
    //declere on reference of this class 
    public static HumanPlayerStateManeger instance;
    // current update state the player running on
    public HumanPlayerAbstract currentCueState;
    HumanPlayerAbstract nextCueState;
    private void Awake()
    {
    #region singelton
        if (instance == null)
            instance = FindObjectOfType<HumanPlayerStateManeger>();
    #endregion
    }
    private void Start()
    {
        nextCueState = currentCueState;
    }
    void Update()
    {
        if (WePressOnUiElement()) return;
        //running the states of the player
        RunStateMachine();
    }
    /// <summary>
    /// run and swiching between the states of the player
    /// </summary>
    private void RunStateMachine()
    {
        if (currentCueState == null) return;
        //run the current state and assign it to varible from the same type
        nextCueState = currentCueState?.runCurrentHumanPlayerUpdateState();
        //if current state returning different state from current state
        if (nextCueState != currentCueState)
        {
            //swith the player the next state
            SwitchToTheNextState(nextCueState);
        }
    }
        
    /// <summary>
    /// swithing from the current state of the player to different state
    /// </summary>
    public void SwitchToTheNextState(HumanPlayerAbstract nextCueState)
    {
        currentCueState = nextCueState;
    }
    /// <summary>
    /// check whether we press on ui element or not
    /// </summary>
    /// <returns></returns>
    bool WePressOnUiElement()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        return false;
    }
}
  

 

   
   
    

   

    
   










