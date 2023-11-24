using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonoClass : MonoBehaviour
{
    [Command]
    public void Test()
    {
        Debug.Log("Hi! My name is: " + this.name);
    }
}
