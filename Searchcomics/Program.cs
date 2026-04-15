

using FuzzierSharp;
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
            GetAuthorPath(autor, ComicsDirectory) : ComicsDirectory;

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
            BrowseFolder(authorPath);
        }


        Console.WriteLine("Do you want to search another comic? (y/n)");

        key = Console.ReadKey(true).Key;
        isContinue = key == ConsoleKey.Y;
    } while (isContinue);
}
else
{
    string author = FindArg("--author");
    string path = GetAuthorPath(author, ComicsDirectory);

    string comic = FindArg("--comic");

    Start(path, comic, ComicsDirectory);
    string toCompare = FindArg("--compare");

    if (!string.IsNullOrEmpty(toCompare))
    {
        path = GetAuthorPath(author, toCompare);
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

static string GetAuthorPath(string author, string path)
{
    var result = Process.ExtractOne(author,
        Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly),
        s => Path.GetFileNameWithoutExtension(s).ToLower());
    return result != null ? result.Value : ComicsDirectory;
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

static int CountPage(string path)
{
    int pages = 0;
    var dirs = new Stack<string>();
    dirs.Push(path);

    while (dirs.Count > 0)
    {
        var current = dirs.Pop();
        try
        {
            foreach (var _ in Directory.EnumerateFiles(current))
                pages++;

            foreach (var dir in Directory.GetDirectories(current))
                dirs.Push(dir);
        }
        catch { /* Ignore inaccessible folders */ }
    }
    return pages;
}

static void BrowseFolder(string path)
{
    foreach (var folder in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly))
    {
        string relative = folder.StartsWith(ComicsDirectory) && folder.Length > ComicsDirectory.Length
            ? folder[ComicsDirectory.Length..].TrimStart(Path.DirectorySeparatorChar)
            : folder;
        bool hasSubDirs = false;
        try
        {
            using var enumerator = Directory.EnumerateDirectories(folder).GetEnumerator();
            hasSubDirs = enumerator.MoveNext();
        }
        catch { /* Ignore inaccessible folders */ }

        if (hasSubDirs)
        {
            BrowseFolder(folder);
        }
        else
        {
            Console.WriteLine($"{relative}: {CountPage(folder)} pages");
        }


    }
}

void Start(string authorPath, string comic, string path)
{
    IEnumerable<string> dirs = Directory.EnumerateDirectories(authorPath, "*", SearchOption.AllDirectories);

    var result = Process.ExtractSorted(comic, dirs, s => Path.GetFileNameWithoutExtension(s).ToLower()
    .Replace("\\W", ""), cutoff: 100);

    if (result.Any())
    {
        foreach (var res in result)
        {
            string fromPath = res.Value.Replace(path, string.Empty)[1..];

            if (path.StartsWith(ComicsDirectory))
                Console.WriteLine($"From Manga: {CountPage(res.Value)} pages - {fromPath}");
            else
                Console.WriteLine($"From Backup: {CountPage(res.Value)} pages - {fromPath}");
        }
    }
    else
        Console.WriteLine($"No comic found in {path}");
}