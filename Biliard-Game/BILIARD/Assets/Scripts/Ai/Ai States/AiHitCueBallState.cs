using System.Collections;
using UnityEngine;
using Cinemachine;
/// <summary>
/// state that responsible on hitting the cueball , this script attached to the ai cue object
/// </summary>
public class AiHitCueBallState : AiPlayerAbstract
{
    public static AiHitCueBallState instance;
    Vector3 dirToAiBall = default;
    float forceToAddToTheCue = default;
    [SerializeField] Animator aiCueAnimator = default;
    [SerializeField] bool isActive;
    private void Awake()
    {
        #region singelton
        if (instance == null)
            instance = FindObjectOfType<AiHitCueBallState>();
        #endregion
    }
    private void Start()
    {
        //deactivate aicue couse player begin the first turn
        gameObject.SetActive(isActive);
    }
    private void OnTriggerEnter(Collider other)
    {
        //if we cue hit the cueball
        if (other.CompareTag("CueBall"))
        {
            //reset push cue animation
            aiCueAnimator.SetBool("isPushCue", false);
            //play collision sound
            GameplayStatesHandler.instance.PlayCollisionHitSound(forceToAddToTheCue,transform.position+transform.forward*1.25f);
            AddForceToAiBall();
            gameObject.SetActive(false);
            GameplayStatesHandler.instance.StartCoroutine(GameplayStatesHandler.instance.WaiteToTheEndOfTheTurn());
        }
    }
    public override AiPlayerAbstract runCurrentAiPlayerUpdateState()
    {
        if (Camera.main.GetComponent<CinemachineBrain>().IsBlending) return this;
        //make push cue animation
        aiCueAnimator.SetBool("isPushCue", true);
        return null;//waiting to the hit after pushing the cue
    }
    /// <summary>
    /// recive the direction of the cueball 
    /// and the force desire to hit the cueball
    /// </summary>
    public void GetDirAndForceToAddToTheCue(Vector3 dirToAiBall,float forceToAddToTheCue)
    {
        this.dirToAiBall = dirToAiBall;
        this.forceToAddToTheCue = forceToAddToTheCue + forceToAddToTheCue;   
    }
    /// <summary>
    /// calclculate the force and aplly the force  on the cueball 
    /// </summary>
    void AddForceToAiBall()
    {
        GameplayStatesHandler.instance.cueBall.GetComponent<Rigidbody>().AddForce(dirToAiBall.normalized * forceToAddToTheCue, ForceMode.Impulse);
    }
 
    public void MethodsToEcecuteWhenSwichingToAiHitState(Vector3 dirToBlendTheVcamera)
    {
        //activate Ai Cue 
        gameObject.SetActive(true);
        //blend vcams
        BlendingVcamsHandler.instance.BlendingVcams(dirToBlendTheVcamera);

    }
}


          
    


       
   

    
 

       
       
        





    



  

