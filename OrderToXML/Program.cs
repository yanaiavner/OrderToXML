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
                StreamWriter toFile = null;
                if (File.Exists(fileName)) File.Delete(fileName);
                toFile = new StreamWriter(fileName);

                try
                {
                    
                    var orderLimit = 0;
                    var cts = new CancellationTokenSource();

                    
                    


                    WriteXMLLine(GetXMLLine(0, "Root"), toFile);
                    var result = SetOrdersXMLAsync(db, orderLimit, toFile, cts.Token);

                    while (!result.IsCompleted)
                    {
                        if (Console.KeyAvailable)
                        {
                            Console.ReadKey();
                            if (!cts.IsCancellationRequested)
                            {
                                cts.Cancel();
                            }
                        }
                    }

                    WriteXMLLine(GetXMLLine(0, "Root", true), toFile);
                    
                    
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    if (toFile == null) Console.ReadKey();
                    if (toFile != null) toFile.Close();
                }
            }
        }

        private static async Task SetOrdersXMLAsync(ProductionDBEntities db, int orderLimit, StreamWriter toFile, CancellationToken token)
        {
            var query = from o in db.Orders
                        orderby o.Name
                        select o;

            var orderCounter = 0;

            foreach (var order in query)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                orderCounter++;
                if (orderLimit > 0 && orderCounter > orderLimit) break;
                await Task.Run(() => SetOrderTagXML(db, toFile, order));

                if(toFile != null)
                {
                    Console.Write(orderCounter);
                    Console.CursorLeft = 0;
                }
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
                var pakcage = (from pk in db.Packages where pk.PackageID == orderPackage.PackageID select pk).FirstOrDefault();
                if (pakcage == null) continue;

                WriteXMLLine(GetXMLLine(2, "Packages", false, orderPackage.PackageID.ToString(), true, "ID"), toFile);
                WriteXMLLine(GetXMLLine(3, "PackageID", false, pakcage.PackageID.ToString()), toFile);
                WriteXMLLine(GetXMLLine(3, "Name", false, pakcage.Name), toFile);
                WriteXMLLine(GetXMLLine(3, "PackageStateID", false, pakcage.PackageStateID.ToString()), toFile);
                WriteXMLLine(GetXMLLine(3, "PackageTypeID", false, pakcage.PackageTypeID.ToString()), toFile);
                WriteXMLLine(GetXMLLine(3, "PDF", false, pakcage.PDF), toFile);

                var packageParts = from pp in db.PackageParts where pp.PackageID == pakcage.PackageID select pp;

                foreach (var pPart in packageParts)
                {
                    var part = (from p in db.Parts where p.PartID == pPart.PartID select p).FirstOrDefault();
                    if (part == null) continue;

                    WriteXMLLine(GetXMLLine(3, "Part", false, part.PartID.ToString(), true, "ID"), toFile);
                    WriteXMLLine(GetXMLLine(4, "PartID", false, part.PartID.ToString()), toFile);
                    WriteXMLLine(GetXMLLine(4, "Number", false, part.PartNo), toFile);
                    WriteXMLLine(GetXMLLine(4, "QuantityOnHand", false, part.QuantityOnHand.ToString()), toFile);
                    WriteXMLLine(GetXMLLine(4, "Description", false, part.Description.Trim()), toFile);
                    WriteXMLLine(GetXMLLine(4, "ProceesTypeID", false, part.ProceesTypeID.ToString()), toFile);

                    WriteXMLLine(GetXMLLine(4, "Vendor", false, part.VendorID.ToString(), true, "ID"), toFile);
                    WriteXMLLine(GetXMLLine(5, "VendorID", false, part.Vendor.VendorID.ToString()), toFile);
                    WriteXMLLine(GetXMLLine(5, "Name", false, part.Vendor.VendorName), toFile);
                    WriteXMLLine(GetXMLLine(4, "Vendor", true), toFile);

                    WriteXMLLine(GetXMLLine(3, "Part", true), toFile);
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
