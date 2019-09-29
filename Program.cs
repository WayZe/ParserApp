using System;
using System.IO;
using System.Net;
using System.Xml;

namespace ParserApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var xDoc = new XmlDocument();
                xDoc.LoadXml(GetXmlString());
                XmlElement xRoot = xDoc.DocumentElement;
                using (DatabaseDataContext db = new DatabaseDataContext())
                {
                    Курс rate;

                    foreach (XmlNode xnode in xRoot)
                    {
                        rate = new Курс();

                        foreach (XmlNode childnode in xnode.ChildNodes)
                        {
                            if (childnode.Name == "CharCode")
                            {
                                rate.валюта = childnode.InnerText;
                            }
                            if (childnode.Name == "Value")
                            {
                                rate.курс = (float)Convert.ToDouble(childnode.InnerText);
                            }
                        }

                        rate.дата = DateTime.Now;

                        db.Курс.InsertOnSubmit(rate);
                        db.SubmitChanges();
                    }
                }

                Console.WriteLine("Данные были получены успешно");
            }
            catch
            {
                Console.WriteLine("Данные не были получены");
            }

            Console.ReadLine();
        }

        private static string GetXmlString()
        {
            string line = "";

            var client = new WebClient();

            using (Stream stream = client.OpenRead("http://www.cbr.ru/scripts/XML_daily.asp?date_req=21.08.2019"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    line = reader.ReadLine();
                }
            }

            return line;
        }
    }
}
