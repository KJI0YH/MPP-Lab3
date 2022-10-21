using Core.Model;

namespace View.Model
{
    public class NodeTree
    {
        public Directory Root { get; }

        public NodeTree(FileTree tree)
        {
            Root = new Directory(tree.Root.Name, tree.Root.Size, 100.00);
            if (tree.Root.Childs != null)
            {
                CreateChilds(tree.Root, Root);
            }
        }

        private void CreateChilds(Core.Model.Node parent, Directory dir)
        {
            foreach (Core.Model.Node child in parent.Childs)
            {
                double sizeInPercent = (double)child.Size / (double)parent.Size * 100;

                // Add file
                if (child.Childs == null)
                {
                    dir.Childs.Add(new File(child.Name, child.Size, sizeInPercent));
                }

                // Add directory
                else
                {
                    Directory nodeDir = new Directory(child.Name, child.Size, sizeInPercent);
                    CreateChilds(child, nodeDir);
                    dir.Childs.Add(nodeDir);
                }
            }
        }
    }
}
