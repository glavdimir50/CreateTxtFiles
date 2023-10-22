using System.Diagnostics;
using System.Text;

namespace CreateTxtFiles
{
    public static class FileProcess
    {
        static int filesCount = 10000;

        static string DiscPath = @"D:\test";

        public static void deleteAll()
        {
            if (Directory.EnumerateFiles(DiscPath).Count().Equals(filesCount))
            {
                Directory.Delete(DiscPath, true);

                Directory.CreateDirectory(DiscPath);
            }
        }

        public static void CreateFiles()
        {
            Stopwatch sw = Stopwatch.StartNew();

            sw.Start();
            
            for (int i = 0; i < filesCount; i++)
            {
                string filePath = Path.Combine(DiscPath, i + ".txt");

                string text = $"Hello World{i}";

                byte[] encodedText = Encoding.Unicode.GetBytes(text);

                using var sourceStream =
                    new FileStream(
                        filePath,
                        FileMode.Create, FileAccess.Write, FileShare.None,
                        bufferSize: 4096);
                sourceStream.Write(encodedText, 0, encodedText.Length);
            }

            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        #region 這樣寫會造成衝突
        //public static async Task CreateFilesAsync()
        //{
        //    List<Task> tasks = new List<Task>();

        //    for (int i = 0; i < filesCount; i++)
        //    {
        //        tasks.Add(ProcessWriteAsync(i));
        //    }

        //    await Task.WhenAll(tasks);
        //}

        //private static async Task ProcessWriteAsync(int i)
        //{
        //    string filePath = Path.Combine(DiscPath, i + ".txt");

        //    string text = $"Hello World{i}";

        //    await WriteTextAsync(filePath, text);
        //}

        //private async static Task WriteTextAsync(string filePath, string text)
        //{
        //    byte[] encodedText = Encoding.Unicode.GetBytes(text);

        //    using (var sourceStream =
        //        new FileStream(
        //            filePath,
        //            FileMode.Create, FileAccess.Write, FileShare.None,
        //            bufferSize: 4096, useAsync: true))
        //    {
        //        await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        //    }
        //}
        #endregion

        public static async Task ProcessMultipleWritesAsync()
        {
            IList<FileStream> sourceStreams = new List<FileStream>();

            IList<Task> writeTaskList = new List<Task>();

            Stopwatch sw = Stopwatch.StartNew();

            Stopwatch sw2 = Stopwatch.StartNew();

            sw2.Start();

            try
            {
                sw.Start();

                FileStream sourceStream;

                for (int i = 0; i < filesCount; ++i)
                {
                    string filePath = Path.Combine(DiscPath, i + ".txt");

                    string text = $"Hello World{i}";

                    byte[] encodedText = Encoding.Unicode.GetBytes(text);

                    sourceStream =
                        new FileStream(
                            filePath,
                            FileMode.Create, FileAccess.Write, FileShare.None,
                            bufferSize: 4096, useAsync: true);

                    Task writeTask = sourceStream.WriteAsync(encodedText, 0, encodedText.Length);

                    sourceStreams.Add(sourceStream);

                    writeTaskList.Add(writeTask);
                }

                await Task.WhenAll(writeTaskList);

                sw.Stop();
            }
            finally
            {
                foreach (FileStream sourceStream in sourceStreams)
                {
                    sourceStream.Dispose();
                }
            }

            sw2.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);

            Console.WriteLine(sw2.ElapsedMilliseconds);
        }
    }
}
