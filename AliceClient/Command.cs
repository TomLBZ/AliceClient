public readonly record struct Command
{
    public string Name { get; init; }
    public Action<string[]> Handler { get; init; }
}
public readonly record struct CommandInfo
{
    public string Name { get; init; }
    public string[] Arguments { get; init; }
    public string Description { get; init; }
}