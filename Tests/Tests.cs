using Core.Model;
using Core.Services;
using System.Diagnostics;

namespace Tests
{
    public class Tests
    {
        private Scanner _scanner;

        [SetUp]
        public void Setup()
        {
            _scanner = new Scanner();
        }

        [Test]
        public void InvalidThreadCountTest()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                _scanner.Start(".", 0);
            });
        }

        [Test]
        public void InvalidDirectoryTest()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                _scanner.Start(".noexist", 5);
            });
        }

        [Test]
        public void DirectoryScanTest()
        {
            string rootDirPath = "../../../../";
            string rootDirName = "testdir";
            string dirName = "dir";
            string fileName = "file";

            if (!Directory.Exists(rootDirPath + rootDirName))
            {
                Directory.CreateDirectory(rootDirPath + rootDirName);
            }

            for (byte i = 0; i < 3; i++)
            {
                string subdirName = rootDirPath + rootDirName + "/" + dirName + i.ToString() + "/";

                if (!Directory.Exists(subdirName))
                {
                    Directory.CreateDirectory(subdirName);
                    FileStream fs = File.Create(subdirName + fileName + i.ToString());
                    fs.WriteByte(i);
                    fs.Close();
                }
            }

            FileTree result = _scanner.Start(rootDirPath + rootDirName, 5);

            Assert.Multiple(() =>
            {
                Assert.That(rootDirName, Is.EqualTo(result.Root.Name));

                Node[] childs = result.Root.Childs.ToArray();

                for (int i = 0; i < childs.Length; i++)
                {
                    switch (childs[i].Name)
                    {
                        case "dir0":
                            {
                                Assert.NotZero(childs[i].Childs.Select(f => f.Name == (fileName + "0") && f.Size == 1).ToArray().Length);
                            }
                            break;
                        case "dir1":
                            {
                                Assert.NotZero(childs[i].Childs.Select(f => f.Name == (fileName + "1") && f.Size == 1).ToArray().Length);
                            }
                            break;
                        case "dir2":
                            {
                                Assert.NotZero(childs[i].Childs.Select(f => f.Name == (fileName + "2") && f.Size == 1).ToArray().Length);
                            }
                            break;
                        default:
                            //Assert.Fail();
                            break;
                    }
                }
            });

        }

        [Test]
        public void CancelTest()
        {
            const string dirPath = "C:\\";
            const int maxThreadCount = 500;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            FileTree resultFull = _scanner.Start(dirPath, maxThreadCount);
            stopwatch.Stop();
            var timeFull = stopwatch.ElapsedMilliseconds;

            FileTree resultCancel = null;
            Task task = Task.Run(() =>
            {
                stopwatch.Restart();
                resultCancel = _scanner.Start(dirPath, maxThreadCount);
                stopwatch.Stop();
            });
            Thread.Sleep(100);
            _scanner.Stop();
            task.Wait();
            var timeCancel = stopwatch.ElapsedMilliseconds;

            Assert.Multiple(() =>
            {
                Assert.That(timeCancel, Is.LessThanOrEqualTo(timeFull));
                Assert.That(resultCancel.Root.Size, Is.LessThanOrEqualTo(resultFull.Root.Size));
            });
        }
    }
}