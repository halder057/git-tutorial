using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using System.IO;
using System.Xml;
namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {

                string fileName = "" , line ;

                fileName = Console.ReadLine();
                Console.WriteLine(fileName);
                //Console.WriteLine(Directory.GetCurrentDirectory() + "testing ");


                XmlReader xmlreader = XmlReader.Create(fileName+ ".xml");
                List<string> student = new List<string>();
                List<string> test = new List<string>();

                while (xmlreader.Read())
                {

                  
                        if (xmlreader.Name.ToString() == "Student")
                        {
                            Console.Write(xmlreader.Value);
                           
                        }

                        else if (xmlreader.Name.ToString() == "Test")
                        {
                            //Console.Write(xmlreader.ReadContentAsString());
                        }
                    
                   
                }

                Console.ReadLine();

            }
            catch (Exception e)
            {

                Console.WriteLine("File couldn't be read");
                Console.ReadLine();
            }

        }
    }
}
