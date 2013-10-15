InterfaceService
================

An example of how you could host an interface as an service as well as access the host from a client.

Example of hosting an interface in a .net web project:

In Global.asax:

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.Add(new Http.InterfaceRoute<IInterface>("test/{func}/", () => new Impl()));
        }
    }
* IInterface is the interface to host
* "test/{func}/" is the url where it is hosted
* new Impl() is a instance implementing the interface


Example of consuming the service on a client:

var client = new InterfaceService.Http.HttpInterface<IInterface>("http://localhost:10082/test/").Client;
var result = client.MethodName("parameters");

