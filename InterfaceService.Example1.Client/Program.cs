using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceService.Example1.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //var proxy = new InterfaceProxy(Http)

            var test = new InterfaceService.Http.HttpInterface<IInterface>("http://localhost:10082/test/");
            var client = test.Client;

            var result = client.Hello("Mikael");
            Console.WriteLine("Result {0}", result);

            var result2 = client.Test(56);
            Console.WriteLine("Result2 {0}", result2);

            var result3 = client.Comples("Test", 56);
            Console.WriteLine("Result3 {0}, {1}", result3.Name, result3.Age);

            var result4 = client.Comples2(new ComplexInput() { Age=1, Name="Testar" });
            Console.WriteLine("Result4 {0}, {1}", result4.Name, result4.Age);

        }
    }
}
