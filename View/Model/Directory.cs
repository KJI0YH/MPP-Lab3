using System.Collections.ObjectModel;

namespace View.Model
{
    public class Directory : Node
    {
        public ObservableCollection<Node> Childs { get; } = new ObservableCollection<Node>();

        public Directory(string name, long size, double sizeInPercent)
        {
            Name = name;
            Size = size;
            SizeInPercent = sizeInPercent;
        }
    }
}
