namespace View.Model
{
    public abstract class Node
    {
        public string Name { get; protected set; }
        public long Size { get; protected set; }
        public double SizeInPercent { get; protected set; }
    }
}
