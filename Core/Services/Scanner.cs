using Core.Model;
using System.Collections.Concurrent;

namespace Core.Services
{
    public class Scanner
    {
        private CancellationTokenSource _tokenSource;
        private TaskQueue _taskQueue;

        private struct ScanInfo
        {
            public Node Node;
            public CancellationToken Token;

            public ScanInfo(Node node, CancellationToken token)
            {
                Node = node;
                Token = token;
            }
        }

        public Scanner()
        {

        }

        public FileTree Start(string path, int maxThreadCount)
        {
            // Single file
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                return new FileTree(new Node(fileInfo.FullName, fileInfo.Name, fileInfo.Length));
            }

            // Directory does not exist
            if (!Directory.Exists(path))
            {
                throw new ArgumentException($"Directory {path} does not exist");
            }

            // Incorrent max thread count
            if (maxThreadCount <= 0)
            {
                throw new ArgumentException($"Thread count counld not be less or equal than 0");
            }

            // Prepare for scanning
            _tokenSource = new CancellationTokenSource();
            _taskQueue = new TaskQueue(maxThreadCount, _tokenSource);

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            Node root = new Node(dirInfo.FullName, dirInfo.Name);
            Task scanTask = new Task(Scan, new ScanInfo(root, _tokenSource.Token), _tokenSource.Token);

            _taskQueue.Add(scanTask);

            _taskQueue.WaitTask.Start();
            _taskQueue.WorkTask.Start();
            _taskQueue.WaitTask.Wait();
            _taskQueue.WorkTask.Wait();

            return new FileTree(root);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private void Scan(object? obj)
        {
            Node parent = ((ScanInfo)obj).Node;
            CancellationToken token = ((ScanInfo)obj).Token;

            parent.Childs = new ConcurrentBag<Node>();
            DirectoryInfo dirInfo = new DirectoryInfo(parent.Path);

            // Process all files in parent directory
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                if (token.IsCancellationRequested)
                    return;
                if (file.LinkTarget == null)
                    parent.Childs.Add(new Node(file.FullName, file.Name, file.Length));
            }

            // Process all dirs in parent directory
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                if (token.IsCancellationRequested)
                    return;
                Node childDir = new Node(dir.FullName, dir.Name);
                parent.Childs.Add(childDir);
                Task scanTask = new Task(Scan, new ScanInfo(childDir, token), token);
                _taskQueue.Add(scanTask);
            }
        }
    }


}
