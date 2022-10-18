namespace Core.Model
{
    public class FileTree
    {
        public Node Root { get; private set; }

        public FileTree(Node root)
        {
            Root = root;
            Root.Size = GetSize(root);
        }

        // Get full size of the node
        private long GetSize(Node node)
        {
            if (node.Childs == null)
            {
                return node.Size;
            }

            foreach (Node child in node.Childs)
            {
                node.Size += GetSize(child);
            }
            return node.Size;
        }
    }
}
