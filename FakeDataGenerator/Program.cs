using System.IO.Compression;
using System.Text;

namespace FakeDataGenerator
{

    internal class Program
    {

        static void Main(string[] args)
        {
            var rnd = new Random();

            string csvHeaderLine = "ID,DT,Item,Price";
            int idCounter = 0;

            for (int i = 0; i < 12; i++)
            {

                var csvBody = new StringBuilder();
                csvBody.AppendLine(csvHeaderLine);

                for (int j = 0; j < 10 * 1000; j++)
                {
                    csvBody.AppendLine(
                        String.Join(",",
                            idCounter.ToString(),
                            new DateTime(2024, i + 1, rnd.Next(1, 28)).ToString("O"),
                            "item_" + rnd.Next(0, 10),
                            (i + rnd.Next(0, 10)).ToString()));
                }


                using (var ms = new MemoryStream())
                {
                    using (var gzip = new GZipStream(ms, CompressionMode.Compress, false))
                    {
                        gzip.Write(Encoding.UTF8.GetBytes(csvBody.ToString()));
                    }

                    string outFileName = $"fakedata_{i}.csv.gz";
                    File.WriteAllBytes($"../../../../Data/{outFileName}", ms.ToArray());
                }

            }
        }

    }

}
