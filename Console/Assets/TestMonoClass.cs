using UnityEngine;

public class TestMonoClass : MonoBehaviour
{
    [Command("test", "tests the console", InstanceTargetType.All)]
    public void Test()
    {
        Debug.Log("Hi! My name is: " + this.name);
    }
}
