using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace InterfaceService.Example1.Server
{
    public class Impl : IInterface
    {
        public string Hello(string name)
        {
            return "Hello " + name;
        }

        public string Test(int num)
        {
            return num.ToString();
        }


        public ComplexInput Comples(string a, int num)
        {
            return new ComplexInput() { Age = num+1, Name = a+"a" };
        }


        public ComplexInput Comples2(ComplexInput b)
        {
            return new ComplexInput() { Age = b.Age+1, Name = b.Name +"a" };
        }
    }

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.Add(new Http.InterfaceRoute<IInterface>("test/{func}/", () => new Impl()));
        }
    }
}