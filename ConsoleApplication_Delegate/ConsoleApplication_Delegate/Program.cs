using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication_Delegate
{

  
    

    class Program
    {
        public delegate void FirstDelegate(params object[] args);
        static void Main(string[] args)
        {
            FirstDelegate delTest = x=> Fun2(200,"test");
            
            delTest();
        }

        public static void Fun1(int a)
        {
            Console.WriteLine("Fun 1 {0}",a);
        }
       

        public static void Fun2(int a , string b)
        {
            Console.WriteLine("Fun 2 {0} , {1}" , a, b);
        }

        public static void Fun3(int a, int b , int c)
        {
            Console.WriteLine("Fun 3 {0} , {1} , {2}" , a , b ,c);
        }



    }

    
    
}
