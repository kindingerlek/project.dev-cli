namespace Tools.DevConsole.Interfaces
{
    public interface ISuggestible
    {
        string[] GetSuggestions(string[] commandParts);
    }
}
