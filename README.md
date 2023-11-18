# BlueConsole
A versatile in-game debug console with built-in commands, logging messages, and a regex-based hint system. It was developed for use in the Unity game engine, but it's suitable for any kind of application and engine that runs on C#. This is because of the separation of console logic, visuals, commands, and type parameters into individual classes.

I initiated this project because some time ago, I needed a console to debug my game and couldn't find anything online that met all my requirements. I then decided to create one myself.

Please let me know what you think about the console and its presentation. I hope that my explanations are somehow C# newcomer-friendly. It's my first game tool, and every piece of feedback is valuable to me. Thanks!

Main features of the console:
    
    Very modular and an easy way to add custom console commands.  
    [ConsoleCommand("clear", "clears the console content")]
    public void Clear()
    {
        _attachedConsole.ClearContent();
    }
    [ConsoleCommand("quit", "closes the application")]
    public void Quit()
    {
        Application.Quit();
    }
    [ConsoleCommand("reset", "resets current scene")]
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }


Every command is essentially a method with a custom "ConsoleCommandAttribute." Adding your own commands is a straightforward process – just add the attribute, customize the command's ID and description, and write the code for your function. The console reads all methods from a specified class and adds them as executables.
    
    Commands use custom parameter types.
    [TypeParameter("\".*?\"", true)]     
    public string StringParameter(string inputParam)     
    {         
        return inputParam.Replace("\"", string.Empty);    
    }
    [TypeParameter(@"[-+]?([0-9]*[.])?[0-9]+([eE][-+]?\d+)?", false)]
    public float FloatParameter(string inputParam)
    {
        return float.Parse(inputParam, NumberStyles.Any, CultureInfo.InvariantCulture);
    }


Every type is parsed and checked differently. BlueConsole, by default, supports almost every C# type and some Unity types. If you want, you can easily allow your own types to be parsed in arguments – it could be a struct, scriptable object, or anything you prefer!

    Regex-based hint system.


The console tells you the closest command to what you are typing in the input field using a simple regex matching function. This feature makes it intuitive to find what you want and ensures that you didn't misspell anything.

    Logic and visuals are separated.
Everything that happens in the console should appear on the screen and be customizable. BlueConsole not only allows you to change every aspect of its looks but can also be completely overhauled with some tweaks in a single class. This is because the logic triggers specific events, and visuals receive them to display content accordingly.

    Logging Unity messages


By default BlueConsole subscribes to all Unity logs and shows them in-game. This is particularly useful for debugging built projects that don't have the default console provided by Unity editor. Colored messaged also make it effortless to spot what needs extra attention in code.

    QnA
What's coming in the next updates?
I will probably need to add comments in code to explain everything along the way, but I hope that my code even without them is clear and readable. I will also tinker around with making the console a static object with a single instance, but I'm still not sure about it yet.
I also want to provide a tutorial for setting up the console in Unity, preferably in a video form. 

Can I suggest something?
Definitely! If you have an idea for a new feature or feel like something need to be changed please let me know in the comments section. I want this project to be polished and complete so every feedback is very welcomed.

How it works?
It uses reflection for the commands methods and type parameters. The rest is pretty basic with a little big of regex validation.

    Resources
Here are some sites I used for learning during the development of BlueConsole. If your interested in these topics I highly recommend them!

For regex:

https://regex101.com

https://gist.github.com/vitorbritto/9ff58ef998100b8f19a0
