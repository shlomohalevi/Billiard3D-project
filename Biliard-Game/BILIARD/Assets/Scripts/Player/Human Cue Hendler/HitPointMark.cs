using UnityEngine;
/// <summary>
/// responsible to change hit mark color of the cueBall
/// </summary>

public class HitPointMark : MonoBehaviour
{
    public static HitPointMark instance;
    [SerializeField] float redColor = default;
    [SerializeField] float greenColor = default;
    [SerializeField] float whiteColor = default;
     Material hitMarkMaterial = default;
    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<HitPointMark>() ;
    }
    private void Start()
    {
        hitMarkMaterial = GetComponent<MeshRenderer>().material;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            BallType ballTypeOfTheBallWeCollideWith = other.GetComponent<UnitBall>().ballType;
            DrawHitPoint(ballTypeOfTheBallWeCollideWith);
        }
        else
            if(hitMarkMaterial.color != new Color(whiteColor, 1, 1, 1f))
        {

            hitMarkMaterial.color = new Color(whiteColor, 1, 1, 1f);
        }
    }
   public void DrawHitPoint(BallType ballType)
    {
        if (ballType == BallType.non || ballType == BallType.humanPlayerBall)
            hitMarkMaterial.color = new Color(0, greenColor, 0, 1f);
        else if (ballType == BallType.computerPlayerBall)
            hitMarkMaterial.color = new Color(redColor, 0, 0, 1f);
        else if (ballType == BallType.blackBall)
        {
            bool isHumanPlayerNotInserstAllHisBalls = GameplayStatesHandler.instance.activeBallsInTheGame.
                 Exists(x => x.ballType == BallType.humanPlayerBall || x.ballType == BallType.non);
            if (isHumanPlayerNotInserstAllHisBalls)
                hitMarkMaterial.color = new Color(redColor, 0, 0, 1f);
            else
                hitMarkMaterial.color = new Color(0, greenColor, 0, 1f);
        }
      
    }
}

   

  


    





       




