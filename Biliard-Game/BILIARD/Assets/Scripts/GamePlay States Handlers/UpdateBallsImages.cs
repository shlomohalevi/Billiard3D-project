using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
/// <summary>
/// class that responsible to display images of balls 
/// that goes to pocket
/// </summary>
public class UpdateBallsImages : MonoBehaviour
{
    public static UpdateBallsImages instance;
    [SerializeField] List<Image> PlayerBallsImaegs = new List<Image>();
    [SerializeField] List<Image> AiBallsImaegs = new List<Image>();
    private void Awake()
    {
        if (instance == null)
            instance = FindObjectOfType<UpdateBallsImages>();
    }
   public void AssignImageOfBall(UnitBall ballThatGoesToPocket)
    {
        if (ballThatGoesToPocket.ballType == BallType.cueBall || ballThatGoesToPocket.ballType == BallType.blackBall) return;
       
        if (ballThatGoesToPocket.ballType == BallType.humanPlayerBall)
            AssignPlayerBallImage(ballThatGoesToPocket);
        else
            AssignAiBallImage(ballThatGoesToPocket);
    }
    void AssignPlayerBallImage(UnitBall playerBall)
    {
        Image slotToAssignBallImage = PlayerBallsImaegs.Where(x => x.sprite == null).First();
        slotToAssignBallImage.sprite = playerBall.ballImage;
        slotToAssignBallImage.color = Color.white;
    }
    void AssignAiBallImage(UnitBall AiBall)
    {
        Image slotToAssignBallImage = AiBallsImaegs.Where(x => x.sprite == null).First();
        slotToAssignBallImage.sprite = AiBall.ballImage;
        slotToAssignBallImage.color = Color.white;
    }
}
   
  
       
       
  
   

        
        

        

        



   
        
        
