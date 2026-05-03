

using FuzzierSharp;
using FuzzierSharp.Extractor;
using SearchComicsLib;
using System.IO.Pipes;

const string ComicsDirectory = @"E:\Manga Scan\Manga\hentai";
const string pipeName = "SearchComicsPipe";
const int timeoutMs = 5000;

if (args.Length == 0)
{
    bool isContinue;
    do
    {
        Console.Clear();
        Console.WriteLine("Enter the name of the autor (enter to blank): ");
        string? autor = Console.ReadLine();

        string authorPath = !string.IsNullOrEmpty(autor) ?
          SearchUtility.GetAuthorPath(autor, ComicsDirectory) : ComicsDirectory;

        Console.WriteLine("Search (S) or List (L)");
        ConsoleKey key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.S)
        {
            Console.WriteLine("Enter the name of the comic: ");
            string? comic = Console.ReadLine();

            if(!string.IsNullOrEmpty(comic))
            {
                Start(authorPath, comic, ComicsDirectory);
            }

            
        }
        else if (key == ConsoleKey.L)
        {
            SearchUtility.BrowseFolder(authorPath, ComicsDirectory);
        }


        Console.WriteLine("Do you want to search another comic? (y/n)");

        key = Console.ReadKey(true).Key;
        isContinue = key == ConsoleKey.Y;
    } while (isContinue);
}
else
{
    string author = FindArg("--author");
    string path = SearchUtility.GetAuthorPath(author, ComicsDirectory);

    string comic = FindArg("--comic");

    Start(path, comic, ComicsDirectory);
    string toCompare = FindArg("--compare");

    if (!string.IsNullOrEmpty(toCompare))
    {
        path = SearchUtility.GetAuthorPath(author, toCompare);
        Start(path, comic, toCompare);
    }

    string jd = FindArg("--jd");
    if (!string.IsNullOrEmpty(jd))
    {
        Console.WriteLine($"From Jdownloader: {FindArg("--jd")}");
    }

   

    string delete = FindArg("--delete");
    if (!string.IsNullOrEmpty(delete))
    {
        try
        {
            using NamedPipeClientStream pipeClient = new(".", pipeName, PipeDirection.InOut);
            pipeClient.Connect(timeoutMs);

            using StreamReader reader = new(pipeClient);
            using StreamWriter writer = new(pipeClient) { AutoFlush = true };

            Console.WriteLine("Connected to pipe server...");
            Console.WriteLine("Confirm Delete (Y/N)?");

            writer.WriteLine(Console.ReadKey().KeyChar);
        }
        catch (Exception)
        {
        }
    }
    else
    {
        Console.WriteLine("Press Enter to end the program");
        Console.ReadLine();
    }
}

string FindArg(string arg)
{
    var result = Process.ExtractOne(arg, args, cutoff: 100);
    if (result != null)
    {
        return args[Array.IndexOf(args, result.Value) + 1];
    }


    return string.Empty;
}

void Start(string authorPath, string comic, string path)
{
    IEnumerable<ExtractedResult<string>> result = SearchUtility.GetComics(authorPath, comic);

    if (result.Any())
    {
        foreach (var res in result)
        {
            string fromPath = res.Value.Replace(path, string.Empty)[1..];

            if (path.StartsWith(ComicsDirectory))
                Console.WriteLine($"From Manga: {SearchUtility.CountPage(res.Value)} pages - {fromPath}");
            else
                Console.WriteLine($"From Backup: {SearchUtility.CountPage(res.Value)} pages - {fromPath}");
        }
    }
    else
        Console.WriteLine($"No comic found in {path}");
}