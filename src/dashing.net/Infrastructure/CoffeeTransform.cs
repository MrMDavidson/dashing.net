using System;
using System.IO;
using System.Web.Optimization;

namespace dashing.net.Infrastructure
{
    public class CoffeeTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            var coffee = new CoffeeSharp.CoffeeScriptEngine();

            response.ContentType = "text/javascript";
            response.Content = string.Empty;

            foreach (var fileInfo in response.Files)
            {
                if (fileInfo.VirtualFile.Name.EndsWith(".coffee", StringComparison.Ordinal))
                {
                    using (Stream stream = fileInfo.VirtualFile.Open())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var result = coffee.Compile(reader.ReadToEnd());
                        response.Content += result;
                    }
                }
                else if (fileInfo.VirtualFile.Name.EndsWith(".js", StringComparison.Ordinal))
                {
                    using (Stream stream = fileInfo.VirtualFile.Open())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        response.Content += reader.ReadToEnd();
                    }
                }
            }
        }
    }
}