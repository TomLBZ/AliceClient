using AliceClient;
Commands commands = new();

// output program args
if (args.Length == 0)
{
    Console.WriteLine("No program arguments provided, do you want to install (i) / uninstall (u)? (q for quit, h for help)");
    Console.WriteLine("i/u/q/h");
    string answer = Console.ReadLine() ?? "q";
    if (answer == "i")
    {
        Console.WriteLine("Enter the install directory:");
        string repo = Console.ReadLine() ?? "";
        if (repo == "")
        {
            Console.WriteLine("No install directory provided, exiting...");
            Utils.WaitForClose();
            return;
        }
        string[] installArgs = new string[] { repo };
        commands.Execute("install", installArgs);
    }
    else if (answer == "u")
    {
        string[] uninstallArgs = Array.Empty<string>();
        commands.Execute("uninstall", uninstallArgs);
    }
    else if (answer == "h")
    {
        string[] helpArgs = Array.Empty<string>();
        commands.Execute("help", helpArgs);
    }
    else
    {
        Console.WriteLine("Exiting...");
        Utils.WaitForClose();
    }
    return;
}
Console.WriteLine($"Program arguments: {string.Join(" ", args)}");
string command = args[0].ToLower();
string[] arguments = args.Skip(1).ToArray();
commands.Execute(command, arguments);