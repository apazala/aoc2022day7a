internal class Program
{
    const int LIMIT = 100000;
    class DirTree
    {
        public long DirectSize { get; set; }
        public string Name { get; }
        Dictionary<string, DirTree> childDirectories;

        public DirTree(string name)
        {
            Name = name;
            DirectSize = 0;
            childDirectories = new Dictionary<string, DirTree>();
        }

        public void AddChild(string name)
        {
            childDirectories[name] = new DirTree(name);
        }

        public DirTree GetChild(string name)
        {
            return childDirectories[name];
        }

        public long TotalSize(ref long sumValidSizes)
        {
            long totalS = DirectSize;
            foreach(var entry in childDirectories)
            {
                totalS+= entry.Value.TotalSize(ref sumValidSizes);
            }

            if(totalS <= LIMIT)
            {
                sumValidSizes += totalS;
            }

            return totalS;
        }
    }

    private static void Main(string[] args)
    {
        List<DirTree> path = new List<DirTree>();
        DirTree currDir = new DirTree("/");
        path.Add(currDir);

        string[] readText = File.ReadAllLines(@"input.txt");
        for (int lineNumber = 0; lineNumber < readText.Length; lineNumber++)
        {
            string[] tokens = readText[lineNumber].Split(' ');

            //Assume no errors: first token always a "$" at this point
            if (tokens[1][0] == 'c') //cd
            {
                ChangeDir(path, tokens[2]);
                
            }
            else  //ls
            {
                currDir = path[path.Count-1];
                long directSize = 0;
                while (lineNumber + 1 < readText.Length && readText[lineNumber + 1][0] != '$')
                {
                    lineNumber++;
                    tokens = readText[lineNumber].Split(' ');
                    if(tokens[0][0] == 'd')//dir
                    {
                        currDir.AddChild(tokens[1]);
                    }else{
                        directSize += long.Parse(tokens[0]);
                    }
                }
                currDir.DirectSize = directSize;
            }

        }

        long sumValidSizes = 0;
        path[0].TotalSize(ref sumValidSizes);

        Console.WriteLine(sumValidSizes);

    }

    private static int LeadingInt(string s)
    {
        int n = 0;
        for (int i = 0; s[i] >= '0'; i++)
        {
            n = 10 * n + (s[i] - '0');
        }
        return n;
    }



    private static void ChangeDir(List<DirTree> path, string dirName)
    {
        switch (dirName)
        {
            case "..":
                path.RemoveAt(path.Count - 1);
                break;
            case "/":
                path.RemoveRange(1, path.Count - 1);
                break;
            default:
                DirTree child = path[path.Count - 1].GetChild(dirName);
                path.Add(child);
                break;
        }
    }
}