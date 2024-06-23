namespace Dispctrl;

internal static class TextUi
{
    public static int ChoicePrompt(string prompt, string[] choices)
    {
        Console.WriteLine(prompt);
        Console.WriteLine();
        for (var i = 0; i < choices.Length; i++)
        {
            Console.WriteLine($"\t{i + 1}) {choices[i]}");
        }
        Console.WriteLine();
        requestInput:
        Console.Write("> ");
        var line = Console.ReadLine();
        if (string.IsNullOrEmpty(line)) goto badSelection;
        if (int.TryParse(line, out var selection))
        {
            if (selection <= choices.Length) return selection;
        }
        badSelection:
        Console.WriteLine("Please enter a selection");
        goto requestInput;
    }
}