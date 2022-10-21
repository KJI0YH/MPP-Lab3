using Core.Model;
using Core.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using View.Commands;
using View.Model;

namespace View.ViewModel
{
    public class ApplicatioinViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly Scanner _scanner = new Scanner();

        public RelayCommand SetDirectoryPathCommand { get; }
        public RelayCommand StartScanningCommand { get; }
        public RelayCommand CancelScanningCommand { get; }

        public ApplicatioinViewModel()
        {
            SetDirectoryPathCommand = new RelayCommand(_ =>
            {
                using var openFileDialog = new FolderBrowserDialog();
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                DirectoryPath = openFileDialog.SelectedPath;
            }, _ => true);

            StartScanningCommand = new RelayCommand(_ =>
            {
                Task.Run(() =>
                {
                    IsScanning = true;
                    FileTree result = _scanner.Start(DirectoryPath, MaxThreadCount);
                    IsScanning = false;
                    Tree = new NodeTree(result);
                });
            }, _ => _directoryPath != null && !IsScanning);

            CancelScanningCommand = new RelayCommand(_ =>
            {
                _scanner.Stop();
                IsScanning = false;
            }, _ => IsScanning);
        }

        private volatile bool _isScanning = false;
        public bool IsScanning
        {
            get { return _isScanning; }
            set
            {
                _isScanning = value;
                OnPropertyChanged("IsScanning");
            }
        }

        private string _directoryPath;
        public string DirectoryPath
        {
            get { return _directoryPath; }
            set
            {
                _directoryPath = value;
                OnPropertyChanged("DirectoryPath");
            }
        }

        private int _maxThreadCount = 100;
        public int MaxThreadCount
        {
            get { return _maxThreadCount; }
            set
            {
                _maxThreadCount = value;
                OnPropertyChanged("MaxThreadCount");
            }
        }

        private NodeTree _tree;
        public NodeTree Tree
        {
            get { return _tree; }
            private set
            {
                _tree = value;
                OnPropertyChanged("Tree");
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
