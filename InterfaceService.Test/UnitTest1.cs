using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace InterfaceService.Test
{
    public interface IInterface
    {
        void Hej();
        int Hejsan(int a, int b, string what);
        string Knuylla();
    }

    public class Complex
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class NoopMethodCaller : IMethodCaller
    {
        public object Call(string methodName, string[] parameterNames, object[] parameterValues, Type[] parameterTypes, Type expectedReturn)
        {
            if (expectedReturn == typeof(void)) return null;
            if (!expectedReturn.IsValueType) return null;

            return Activator.CreateInstance(expectedReturn);
        }
    }

    [TestClass]
    public class HttpInterfaceTest
    {
        [TestMethod]
        public void TestTest()
        {

            var test = new ProxyInterface<IInterface>(new NoopMethodCaller());

            Assert.AreEqual(null, test.Client.Knuylla());

            var value = test.Client.Hejsan(1, 2, "Mikael");
            Assert.AreEqual(0, value);
            test.Client.Hej();
        }

        [TestMethod]
        public void asd()
        {

            var proxy = new ProxyInterface<IInterface>(new NoopMethodCaller());
            proxy.Client.Hej();
            proxy.Client.Hejsan(1, 2, "");
        }

        

        [TestMethod]
        public void TestTest2()
        {
            JsonSerializer s = new JsonSerializer();
            var response = s.SerializeParameter(new string[] { "test" }, new object[] { new Complex() { Name="Micke", Age=27 } } );

            var parameters = s.DeserializeParameters(response);
            Assert.AreEqual("test", parameters.First().Name);
            var value = (Complex)parameters.First().GetValue(typeof(Complex));

            Assert.AreEqual("Micke", value.Name);
            Assert.AreEqual(27, value.Age);
            

            Console.WriteLine(response);
        }

        [TestMethod]
        public void Test3()
        {
            var caller = new Http.HttpCaller(new JsonSerializer(), "http://localhost:10082/test/");
            var response = caller.Call("Hello", new string[] { "name" }, new object[] { "Micke" }, new Type[] { typeof(string) }, typeof(string));

            Console.WriteLine("Method responsed with: {0}", response);
        }
    }
}
