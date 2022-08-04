using UnityEngine;
using Cinemachine;
/// <summary>
/// class that responsible for cinemachine operations
/// </summary>
public class BlendingVcamsHandler : MonoBehaviour
{
    public static BlendingVcamsHandler instance;
    [Tooltip("this vcam only for player and not ai")]
    public CinemachineVirtualCamera doNothingVcam = default;
    [Header("blending vcams")]
    public CinemachineVirtualCamera vcamBlending1 = default;
    public CinemachineVirtualCamera vcamBlending2 = default;
    private void Awake()
    {
        #region singelton
        if (instance == null)
            instance = FindObjectOfType<BlendingVcamsHandler>();
        #endregion
    }
    /// <summary>
    /// blend vcam to new pos to track the cueball after hitting the cueball
    /// </summary>
   public void BlendingVcams(Vector3 dirToSendTheBlendingVcam)
    {
        // get the position to blend the vcam
        Vector3 positionToBlendThVcam = PostisioningBlendingVcam(dirToSendTheBlendingVcam);
        if(doNothingVcam.gameObject.activeInHierarchy)
        doNothingVcam.transform.gameObject.SetActive(false);
        if (!vcamBlending1.gameObject.activeInHierarchy)
        {
            vcamBlending1.transform.position = positionToBlendThVcam;
            vcamBlending2.transform.gameObject.SetActive(false);
            vcamBlending1.transform.gameObject.SetActive(true);
        }
        else
        {
            vcamBlending2.transform.position = positionToBlendThVcam;
            vcamBlending1.transform.gameObject.SetActive(false);
            vcamBlending2.transform.gameObject.SetActive(true);
        }

    }
    /// <returns>
    /// position to blend the vcam
    /// </returns>
    Vector3 PostisioningBlendingVcam(Vector3 dirToSendTheBlendingVcam)
    {
        dirToSendTheBlendingVcam = new Vector3(dirToSendTheBlendingVcam.x, 0, dirToSendTheBlendingVcam.z);
        Ray ray = new Ray(GameplayStatesHandler.instance.cueBall.transform.position,dirToSendTheBlendingVcam );
        if (Physics.Raycast(ray, out RaycastHit hitInfo,Mathf.Infinity,LayerMask.GetMask("WallToBlendTo")))
        {
            return new Vector3(hitInfo.point.x,Vector3.up.y*2.7f,hitInfo.point.z);
        }
        return default;
    }
}

   
   
  

        
   
            
   
   
    

