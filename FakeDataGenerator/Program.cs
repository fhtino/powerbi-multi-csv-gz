using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FakeDataGenerator
{

    internal class Program
    {

        static void Main(string[] args)
        {
            string outputFolder = "../../../../Data/";
            Directory.CreateDirectory(Path.Combine(outputFolder, "Orders"));
            Directory.CreateDirectory(Path.Combine(outputFolder, "OrdersDetails"));

            int maxOrdersPerFiles = 20000;
            DateTime baseDT = new DateTime(2024, 1, 1);

            var rnd = new Random(1);

            string OrderCsvHeaderLine = "OrderID,DT,CustomerID";
            string OrderDetailsCsvHeaderLine = "OrderID,RowID,Item,Price";

            int fileCounter = 0;
            int orderPerFileCounter = 0;

            var orderCsvBody = new StringBuilder();
            var orderDetailsCsvBody = new StringBuilder();

            for (int orderID = 0; orderID < 200 * 1000; orderID++)
            {
                if (orderCsvBody.Length == 0) orderCsvBody.AppendLine(OrderCsvHeaderLine);
                if (orderDetailsCsvBody.Length == 0) orderDetailsCsvBody.AppendLine(OrderDetailsCsvHeaderLine);

                int dayNumber = rnd.Next(0, 365);

                orderCsvBody.AppendLine(
                    String.Join(",",
                        orderID.ToString(),
                        baseDT.AddDays(dayNumber).ToString("O"),
                        rnd.Next(0, 100)
                        ));

                for (int orderRowID = 0; orderRowID < rnd.Next(0, 10); orderRowID++)
                {
                    double price = (rnd.Next(0, 5) + 5 + 3 * Math.Sin((double)dayNumber / 20.0));

                    orderDetailsCsvBody.AppendLine(
                        String.Join(",",
                            orderID.ToString(),
                            orderRowID.ToString(),
                            "item_" + rnd.Next(0, 100),
                            price.ToString(CultureInfo.InvariantCulture)
                        ));
                }

                orderPerFileCounter++;

                if (orderPerFileCounter >= maxOrdersPerFiles)
                {
                    WriteCsvGz(orderCsvBody.ToString(), $"../../../../Data/Orders/orders_{fileCounter}.csv.gz");
                    WriteCsvGz(orderDetailsCsvBody.ToString(), $"../../../../Data/OrdersDetails/orders_details_{fileCounter}.csv.gz");

                    orderPerFileCounter = 0;
                    fileCounter++;
                    orderCsvBody.Clear();
                    orderDetailsCsvBody.Clear();
                }
            }           
        }


        private static void WriteCsvGz(string body, string fileName)
        {
            Console.WriteLine($"{fileName}  [{body.Length}]");

            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, false))
                {
                    gzip.Write(Encoding.UTF8.GetBytes(body));
                }
                File.WriteAllBytes(fileName, ms.ToArray());
            }
        }

    }

}
