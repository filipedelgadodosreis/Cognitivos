using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ImageFaces
{
    class Program
    {
        static void Main(string[] args)
        {
            const string subscriptionKey = "<Subscription Key>";

            const string uriBase =
                "https://<myresourcename>.cognitive.microsoft.com/face/v1.0/detect";

            Console.WriteLine("Hello World!");
        }
    }
}
