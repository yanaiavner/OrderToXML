using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;


namespace OrderToXML
{
    class Program
    {
        static void Main(string[] args)
        {
            
            using (var db = new ProductionDBEntities())
            {
                var fileName = "Order.xml";
                var orderLimit = 3;
                var cts = new CancellationTokenSource();

                if (File.Exists(fileName)) File.Delete(fileName);
                //StreamWriter toFile = new StreamWriter(fileName);
                StreamWriter toFile = null;

                WriteXMLLine(GetXMLLine(0, "Root"), toFile);
                var result = SetOrdersXMLAsync(db, orderLimit, toFile);
                result.Wait();

                WriteXMLLine(GetXMLLine(0, "Root", true), toFile);


                if (toFile == null) Console.ReadKey();

                if (toFile != null) toFile.Close();
            }
        }

        private static async Task SetOrdersXMLAsync(ProductionDBEntities db, int orderLimit, StreamWriter toFile)
        {
            var query = from o in db.Orders
                        orderby o.Name
                        select o;

            var orderCounter = 0;

            foreach (var order in query)
            {
                orderCounter++;
                if (orderLimit > 0 && orderCounter > orderLimit) break;
                await Task.Run(() => SetOrderTagXML(db, toFile, order));
            }
        }

        private static void SetOrderTagXML(ProductionDBEntities db, StreamWriter toFile, Order order)
        {
            WriteXMLLine(GetXMLLine(1, "Order", false, order.OrderID.ToString(), true, "ID"), toFile);
            WriteXMLLine(GetXMLLine(2, "Name", false, order.Name), toFile);
            WriteXMLLine(GetXMLLine(2, "System", false, order.System), toFile);
            WriteXMLLine(GetXMLLine(2, "PO", false, order.PO), toFile);
            WriteXMLLine(GetXMLLine(2, "PODate", false, order.PODate), toFile);
            if (!string.IsNullOrWhiteSpace(order.SN)) WriteXMLLine(GetXMLLine(2, "SN", false, order.SN), toFile);
            WriteXMLLine(GetXMLLine(2, "StateID", false, order.StateID.ToString()), toFile);

            var orderPakcages = from op in db.OrderPackages where op.OrderID == order.OrderID select op;

            foreach (var orderPackage in orderPakcages)
            {
                var pakcage = from pk in db.Packages where pk.PackageID == orderPackage.PackageID select pk;

                WriteXMLLine(GetXMLLine(2, "Packages", false, orderPackage.PackageID.ToString(), true, "ID"), toFile);
                if (pakcage.Count() > 0)
                {
                    WriteXMLLine(GetXMLLine(3, "Name", false, pakcage.FirstOrDefault().Name), toFile);
                    WriteXMLLine(GetXMLLine(3, "PackageStateID", false, pakcage.FirstOrDefault().PackageStateID.ToString()), toFile);
                    WriteXMLLine(GetXMLLine(3, "PackageTypeID", false, pakcage.FirstOrDefault().PackageTypeID.ToString()), toFile);
                    WriteXMLLine(GetXMLLine(3, "PDF", false, pakcage.FirstOrDefault().PDF), toFile);
                }
                WriteXMLLine(GetXMLLine(2, "Packages", true), toFile);
            }

            WriteXMLLine(GetXMLLine(1, "Order", true), toFile);
        }

        static void WriteXMLLine(string line, StreamWriter toFile)
        {
            if (toFile != null)
            {
                toFile.WriteLine(line);
            }
            else
            {
                Console.WriteLine(line);
            }
            
        }

        static string GetXMLLine(int level, string tag, bool isCloseTag = false, string value = null, bool isValueAtrebute = false, string atrebute = null)
        {
            string line = string.Empty;

            for (int i = 0; i < level; i++)
            {
                line = line + "\t";
            }

            if (isCloseTag)
            {
                line += $"</{tag}>";
                return line;
            }

            line += $"<{tag}";

            if(value == null)
            {
                line += ">";
                return line;
            }

            if(isValueAtrebute)
            {
                line += $" {atrebute}={value}>";
                return line;
            }

            line += $">{value}</{tag}>";

            return line;
        }
    }
}
