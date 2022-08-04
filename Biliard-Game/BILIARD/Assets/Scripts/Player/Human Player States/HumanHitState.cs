using UnityEngine;
public class HumanHitState :HumanPlayerAbstract
{
    public static HumanHitState instance;
    float cueForce;
    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<HumanHitState>();
    }
    private void FixedUpdate()
    {
        if (NextCueState != CueState.pushState) return;
            CalculateCueForce();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CueBall"))
        {
            HumanPlayerStateManeger.instance.SwitchToTheNextState(this);
        }
    }
    public override HumanPlayerAbstract runCurrentHumanPlayerUpdateState()
    {
        GameplayStatesHandler.instance.PlayCollisionHitSound(cueForce,transform.position + transform.forward * 1.25f);
        AddForceToTheCueBall();
        MethodsToExecuteAfterWeHittingTheCueball();
        GameplayStatesHandler.instance.StartCoroutine(GameplayStatesHandler.instance.WaiteToTheEndOfTheTurn());
        return null;
    }
    void CalculateCueForce()
    {
        float force = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
        if (force == 0) return;
        cueForce = force;
    }
    void AddForceToTheCueBall()
    {
        Vector3 dirToHitTheCueBall = new Vector3(transform.forward.x, 0, transform.forward.z);
        GameplayStatesHandler.instance.cueBall.GetComponent<Rigidbody>().AddForce(dirToHitTheCueBall.normalized * cueForce, ForceMode.Impulse);
    }
    void MethodsToExecuteAfterWeHittingTheCueball()
    {
        gameObject.SetActive(false);
        TrajectoryLine.instance.RemoveVisualAimingMarks();
        HumanBonusTurnState.instance.isBonusTurn = false;
        BallMovementInFirstTurnState.Instance.isFirstTurn = false;
        BlendingVcamsHandler.instance.BlendingVcams(-GameplayStatesHandler.instance.playerCue.transform.forward);
    }
}


    




           
  
    
  


       
        
   

      






