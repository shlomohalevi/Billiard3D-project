using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSinngelton : MonoBehaviour
{
    // Start is called before the first frame update

    private void Start()
    {
        Test.testInstance.transform.position = Vector3.one;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
