namespace AliceClient
{
    internal class Commands
    {
        internal static CommandInfo[] AllInfo = new CommandInfo[]
        {
            new()
            {
            Name = "help",
            Arguments = Array.Empty<string>(),
            Description = "Displays all available commands",
            },
            new()
            {
            Name = "install",
            Arguments = new string[] { "installPath" },
            Description = "Installs the Alice Client into the installPath directory (MUST be empty) and add it to the PATH variable",
            },
            new()
            {
            Name = "uninstall",
            Arguments = Array.Empty<string>(),
            Description = "Uninstalls the Alice Client from the installPath and delete it from the PATH variable ",
            },
            new()
            {
            Name = "create",
            Arguments = new string[] { "lang", "outPath" },
            Description = $"Creates a new Doll project of the specified lang (cs or ts) into the outPath (MUST be empty)",
            },
            new()
            {
                Name = "build",
                Arguments = new string[] { "projectPath", "user" },
                Description = "Builds the project at the projectPath and upload to DockerHub under the user specified. You must \"docker login\" first!",
            }
        };
        internal Command[] All { get; init; }
        internal Dictionary<string, Action<string[]>> Actions = new();
        internal Commands()
        {
            All = new Command[]
            {
                Help,
                Install,
                Uninstall,
                Create,
                Build
            };
            foreach (Command cmd in All)
            {
                Actions.Add(cmd.Name, cmd.Handler);
            }
        }
        internal static Command Help = new()
        {
            Name = "help",
            Handler = arguments =>
            {
                Console.WriteLine("Available commands:");
                foreach (CommandInfo cmd in AllInfo)
                {
                    Console.WriteLine($"  {cmd.Name} {string.Join(" ", cmd.Arguments)}");
                    Console.WriteLine($"    {cmd.Description}");
                }
            }
        };
        internal static Command Install = new()
        {
            Name = "install",
            Handler = arguments =>
            {
                // checks if the process has admin rights
                if (!Utils.IsAdministrator())
                {
                    Console.WriteLine("You need to run this program as administrator (root) to uninstall.");
                    return;
                }
                // checks if the argument count is valid
                if (arguments.Length != 1)
                {
                    Console.WriteLine("Invalid argument count. Try \"alice help install\" first.");
                    return;
                }
                // Checks if the installPath is valid
                string installPath = arguments[0];
                if (!Directory.Exists(installPath))
                {
                    Console.WriteLine($"The installPath {installPath} does not exist, creating...");
                    try
                    {
                        Directory.CreateDirectory(installPath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occured while creating the installPath: {e.Message}");
                        return;
                    }
                }
                // Checks if the installPath is empty
                if (Directory.EnumerateFileSystemEntries(installPath).Any())
                {
                    Console.WriteLine($"The installPath {installPath} is not empty. Installation canceled.");
                    return;
                }
                // copy this program to the installPath
                try
                {
                    string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "";
                    string programName = $"{assemblyName}.exe";
                    string dllName = $"{assemblyName}.dll";
                    string runtimeconfigName = $"{assemblyName}.runtimeconfig.json";
                    // if exe exists, copy it to the installPath
                    if (File.Exists(programName))
                        File.Copy(programName, Path.Combine(installPath, programName));
                    // if dll exists, copy it to the installPath
                    if (File.Exists(dllName))
                        File.Copy(dllName, Path.Combine(installPath, dllName));
                    // if plane assembly file exists, copy it to the installPath
                    if (File.Exists(assemblyName))
                        File.Copy(assemblyName, Path.Combine(installPath, assemblyName));
                    // if runtimeconfig exists, copy it to the installPath
                    if (File.Exists(runtimeconfigName))
                        File.Copy(runtimeconfigName, Path.Combine(installPath, runtimeconfigName));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occured while copying the program to the installPath: {e.Message}");
                    return;
                }
                // add the installPath to the PATH variable
                string path = Environment.GetEnvironmentVariable("PATH") ?? "";
                if (path.Split(';').Contains(installPath))
                {
                    Console.WriteLine($"The installPath {installPath} is already in the PATH variable.");
                }
                else
                {
                    try
                    {
                        Environment.SetEnvironmentVariable("PATH", $"{path};{installPath}", EnvironmentVariableTarget.Machine);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occured while adding the installPath to the PATH variable: {e.Message}");
                        return;
                    }
                }
                Console.WriteLine($"The Alice Client has been installed in the installPath {installPath}. Try \"alice help\" in a new console window!");
            }
        };
        internal static Command Uninstall = new()
        {
            Name = "uninstall",
            Handler = arguments =>
            {
                // checks if the process has admin rights
                if (!Utils.IsAdministrator())
                {
                    Console.WriteLine("You need to run this program as administrator (root) to uninstall.");
                    return;
                }
                // gets the executable path
                string executablePath = System.Reflection.Assembly.GetEntryAssembly()?.Location ?? "";
                // gets the installPath
                string installPath = Path.GetDirectoryName(executablePath) ?? "";
                // checks if the installPath is in the PATH variable
                string path = Environment.GetEnvironmentVariable("PATH") ?? "";
                if (!path.Split(';').Contains(installPath))
                {
                    Console.WriteLine($"The installPath {installPath} is not in the PATH variable. The program has not been installed correctly.");
                }
                else
                {
                    // removes the installPath from the PATH variable
                    try
                    {
                        Environment.SetEnvironmentVariable("PATH", path.Replace(installPath, ""), EnvironmentVariableTarget.Machine);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occured while removing the installPath from the PATH variable: {e.Message}");
                        return;
                    }
                }
                // deletes the installPath after this program is closed
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        Arguments = $"/C choice /C Y /N /D Y /T 5 & rmdir /S /Q \"{installPath}\"",
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        FileName = "cmd.exe"
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occured while deleting the installPath: {e.Message}");
                    return;
                }
                Console.WriteLine($"The Alice Client has been uninstalled from the installPath {installPath}.");
            }
        };
        internal static Command Create = new()
        {
            Name = "create",
            Handler = arguments =>
            {
                // checks if the argument count is valid
                if (arguments.Length != 2)
                {
                    Console.WriteLine("Invalid argument count. Try \"alice help create\" first.");
                    return;
                }
                // Checks if the outpath is valid
                string outpath = arguments[1];
                if (!Directory.Exists(outpath))
                {
                    Console.WriteLine($"The outpath {outpath} does not exist, creating...");
                    try
                    {
                        Directory.CreateDirectory(outpath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occured while creating the outpath: {e.Message}");
                        return;
                    }
                }
                // Checks if the outpath is empty
                if (Directory.EnumerateFileSystemEntries(outpath).Any())
                {
                    Console.WriteLine($"The outpath {outpath} is not empty. Skipped.");
                    return;
                }
                // checks if git is available
                if (!Utils.IsGitAvailable())
                {
                    Console.WriteLine("Git is not available. Please install it and try again.");
                    return;
                }
                // Clones the repository
                string repo = arguments[0].ToLower() switch
                {
                    "cs" => "https://github.com/TomLBZ/Doll.git",
                    "ts" => "",
                    _ => ""
                };
                if (repo == "")
                {
                    Console.WriteLine("Invalid language. Try \"alice help create\" first.");
                    return;
                }
                System.Diagnostics.ProcessStartInfo startInfo = new()
                {
                    FileName = "git",
                    Arguments = $"clone {repo} {outpath}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                System.Diagnostics.Process? process = System.Diagnostics.Process.Start(startInfo);
                if (process == null)
                {
                    Console.WriteLine("An error occured while cloning the repository.");
                    return;
                }
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    Console.WriteLine("An error occured while cloning the repository.");
                    return;
                }
                Console.WriteLine($"The project has been created under {outpath}, please follow the README.md to develop the dll.");
            }
        };
        internal static Command Build = new()
        {
            Name = "build",
            Handler = arguments =>
            {
                Console.WriteLine("Validating information...");
                // checks if the argument count is valid
                if (arguments.Length != 2)
                {
                    Console.WriteLine("Invalid argument count. Try \"alice help build\" first.");
                    return;
                }
                // Checks if the project path is valid
                string projectPath = arguments[0];
                if (!Directory.Exists(projectPath))
                {
                    Console.WriteLine($"The outpath {projectPath} does not exist, build canceled.");
                    return;
                }
                // Checks if the project path is empty
                if (!Directory.EnumerateFileSystemEntries(projectPath).Any())
                {
                    Console.WriteLine($"The outpath {projectPath} is empty. Skipped.");
                    return;
                }
                // goto the projectPath\Doll folder
                string dollPath = Path.Combine(projectPath, "Doll");
                if (!Directory.Exists(dollPath))
                {
                    Console.WriteLine($"The project {projectPath} is not a valid project. Skipped.");
                    return;
                }
                // checks if the Dockerfile is available
                string dockerfilePath = Path.Combine(dollPath, "Dockerfile");
                if (!File.Exists(dockerfilePath))
                {
                    Console.WriteLine($"The project {projectPath} is not a valid project. Skipped.");
                    return;
                }
                // checks if the docker command is available
                if (!Utils.IsDockerAvailable())
                {
                    Console.WriteLine("Docker is not available. Please install it and try again.");
                    return;
                }
                string dockerUser = arguments[1];
                // generates a random string for the docker image name
                string dockerImageName = Utils.GenerateRandomString();
                string buildCommand = $"build -t {dockerUser}/{dockerImageName}:latest -f {dockerfilePath} {projectPath}";
                // builds the docker image
                Console.WriteLine("Building...");
                System.Diagnostics.ProcessStartInfo startInfo = new()
                {
                    FileName = "docker",
                    Arguments = buildCommand,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                System.Diagnostics.Process? process = System.Diagnostics.Process.Start(startInfo);
                if (process == null)
                {
                    Console.WriteLine($"An error occured while building the docker image. Try building manually using this command:\ndocker {buildCommand}");
                    return;
                }
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    string err = process.StandardOutput.ReadToEnd();
                    Console.WriteLine($"The following error occured while building the docker image:\n{err}\nTry building manually using this command:\ndocker {buildCommand}");
                    return;
                }
                // pushes the docker image
                Console.WriteLine("Pushing...");
                startInfo = new()
                {
                    FileName = "docker",
                    Arguments = $"push {dockerUser}/{dockerImageName}:latest",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                process = System.Diagnostics.Process.Start(startInfo);
                if (process == null)
                {
                    Console.WriteLine("An error occured while pushing the docker image. Have you logged in?");
                    return;
                }
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    string err = process.StandardOutput.ReadToEnd();
                    Console.WriteLine($"The following error occured while pushing the docker image:\n{err}\nHave you logged in?");
                    return;
                }
                // save a log file
                string logPath = Path.Combine(projectPath, "buildlog.alice");
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                File.WriteAllText(logPath, $"{time}: Pushed to [{dockerUser}/{dockerImageName}:latest]");
                Console.WriteLine($"The docker image has been built and pushed to {dockerUser}/{dockerImageName}:latest.");
            }
        };
        internal void Execute(string command, string[] arguments)
        {
            if (Actions.ContainsKey(command))
            {
                Actions[command](arguments);
            }
            else
            {
                Console.WriteLine($"The command {command} does not exist. Try \"alice help\" first.");
            }
            Utils.WaitForClose();
        }
    }

}
