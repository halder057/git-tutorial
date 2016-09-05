using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
         

        static void Main(string[] args)
        {

            string str = "1234aa";

            int num =  55;

            bool  res =  Int32.TryParse(str ,out num);

            if (res)
                Console.WriteLine("Done");

            Console.WriteLine(num);
            Console.ReadLine();
        }
    }
}
