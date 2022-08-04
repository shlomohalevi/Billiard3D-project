using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class Test : MonoBehaviour
{
    public static Test testInstance;
    public Iinterface iinterface;
    [SerializeField]
    Text text;
    private void Awake()
    {
        
        
        if (testInstance == null)
            testInstance = FindObjectOfType<Test>();
    }
    void Start()
    {
        Debug.Log(this);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
    public class Student {public int ma = 9; }
