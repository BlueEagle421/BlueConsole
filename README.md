<h1 align="center">  
  Blue-Console
</h1>

<h4 align="center">A lightweight, versatile in-game debug console with built-in commands, logging messages, and a regex-based hint system.</h4>

<p align="center">
 <a href="https://unity3d.com/get-unity/download">
 <img src="https://img.shields.io/badge/unity-2022.3%2B-blue.svg" alt="Unity Download Link">
 <a href="https://github.com/BlueEagle421/BlueConsole/blob/main/LICENSE">
 <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" alt="License MIT">
</p>

 <p align="center">
  <a href="#about">About</a> •
  <a href="#installation">Installation</a> •
  <a href="#features">Features</a> •
  <a href="#feedback">Feedback</a> •
  <a href="#resources">Resources</a> •
  <a href="#license">License</a>
</p>

## About
A versatile in-game debug console with built-in commands, logging messages, and a regex-based hint system.
The strongest aspect of this console is how easy and fast it is to add new commands to suit any game.

I initiated this project because some time ago, I needed a console to debug my game and couldn't find anything online that met all my requirements.
I then decided to create one myself.

## Installation
To add BlueConsole to your project simply copy the HTTPS link from this repository and download it from the Unity Package Manager using the "Add package from git URL..." option.

```
https://github.com/BlueEagle421/BlueConsole.git
```

<p align="center">
 <img src="https://i.imgur.com/uf6pXfu.png">
</p>

You can also download it directly from [Releases](https://github.com/BlueEagle421/BlueConsole/releases)

## Features

Main features of the console:

### Very modular and an easy way to add custom console commands.
Every command is essentially a method with a custom "ConsoleCommandAttribute." Adding your own commands is a straightforward process – just add the attribute, customize the command's ID and description, and write the code for your function. The console reads all methods from specified assemblies and adds them as executables.

```c#
[Command("quit", "closes the application")]
public static void Quit()
{
    Application.Quit();
}
```



### Commands use custom parameter types.
Every type is parsed and checked differently. BlueConsole, by default, supports almost every C# type and a bunch of Unity types. If you want, you can easily allow your own types to be parsed in arguments – it could be a struct, scriptable object, or anything you prefer!

```c#
[TypeParameter("\".*?\"", true)]     
public static string StringParameter(string inputParam)     
{         
    return inputParam.Replace("\"", string.Empty);    
}
```



### Regex-based hint system.
The console tells you the closest command to what you are typing in the input field using a simple regex matching function. This feature makes it intuitive to find what you want and ensures that you didn't misspell anything.

### Logic and visuals are separated.
Everything that happens in the console should appear on the screen and be customizable. BlueConsole not only allows you to change every aspect of its looks but can also be completely overhauled with some tweaks in a single class. This is because the logic triggers specific events, and visuals receive them to display content accordingly.

### Logging Unity messages
By default BlueConsole subscribes to all Unity logs and shows them in-game. This is particularly useful for debugging built projects that don't have the default console provided by Unity editor. Colored messaged also make it effortless to spot what needs extra attention in code.

## Feedback

Please let me know what you think about the console and its presentation. I hope that my explanations are somewhat C# newcomer-friendly. It's my first game tool, and every piece of feedback is valuable to me.
Every issue with the package can be reported here:
[Issues](https://github.com/BlueEagle421/BlueConsole/issues)

## Resources
Here are some sites I used for learning during the development of BlueConsole. If your interested in these topics I highly recommend them!

For regex:

https://regex101.com

https://gist.github.com/vitorbritto/9ff58ef998100b8f19a0

## License

MIT License

https://github.com/BlueEagle421/BlueConsole/blob/main/LICENSE
