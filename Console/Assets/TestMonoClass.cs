using UnityEngine;
using BlueConsole;
public class TestMonoClass : MonoBehaviour
{
    [Command("test", "tests the console", InstanceTargetType.All)]
    public void Test()
    {
        Debug.Log("Hi! My name is: " + this.name);
    }

    private void Start()
    {
        //InvokeRepeating(nameof(Overload), 5f, 0.2f);
    }

    void Overload()
    {
        Debug.Log("Overload");
    }
}
