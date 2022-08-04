
using UnityEngine;


public abstract class HumanPlayerAbstract : MonoBehaviour
{
   
    public static CueState NextCueState;
    public abstract HumanPlayerAbstract runCurrentHumanPlayerUpdateState();
    /// <summary>
    /// determind  what is the next state of the player
    /// according to the player actions
    /// </summary>
    /// <returns>
    /// the next state
    /// </returns>
    public static CueState NewCueState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //if we press on the cue
            if (Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("Cue")))
                return CueState.pushState;
            //if we press on the cue ball and this the first turn
            else if (BallMovementInFirstTurnState.Instance.isFirstTurn && Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("CueBall")))
                return CueState.firstTurnState;
            //if we press on the cueball and we allowed to place the cueball everywhere
            else if (HumanBonusTurnState.instance.isBonusTurn && Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("CueBall")))
                return CueState.bonusTurnState;
            //if we press everywhere else
            else
                return CueState.RotateState;
        }
        // if we not press on the mouse return the last state that we were in it
        return NextCueState;
    }
    protected static Plane surfaceformouseDetection = new Plane(Vector3.up, Vector3.zero);
    /// <summary>
    /// converting mouse position to world position
    /// relative to surface plan in vector.up normal and vector3.ziro point
    /// </summary>
    /// <returns>
    /// mouse position in world position
    /// </returns>
    protected static Vector3 MousePositionInWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distanceFromRayOriginToIntersectionPointWithTheSurfacePlane;
        if (surfaceformouseDetection.Raycast(ray, out distanceFromRayOriginToIntersectionPointWithTheSurfacePlane))
        {
            Vector3 MousePositionInWorldSpace = ray.GetPoint(distanceFromRayOriginToIntersectionPointWithTheSurfacePlane);
            return MousePositionInWorldSpace;
        }

        return default;
    }
}
    public enum CueState
{
    MouseNotHit,
    firstTurnState,
    RotateState,
    pushState,
    bonusTurnState,
    hitState,
}
    
 




       
       


   

      