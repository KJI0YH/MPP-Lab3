namespace View.Model
{
    public class File : Node
    {
        public File(string name, long size, double sizeInPercent)
        {
            Name = name;
            Size = size;
            SizeInPercent = sizeInPercent;
        }

        public string IcoPath
        {
            get
            {
                return "Resources/file.png";
            }
        }
    }
}
