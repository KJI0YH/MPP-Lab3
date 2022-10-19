using System.Collections.Concurrent;

namespace Core.Model
{
    public class Node
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public long Size { get; set; }
        public ConcurrentBag<Node>? Childs { get; set; } = null;

        public Node(string path, string name)
        {
            Path = path;
            Name = name;
        }

        public Node(string path, string name, long size) : this(path, name)
        {
            Size = size;
        }
    }
}
