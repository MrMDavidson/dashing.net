using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Optimization;

namespace dashing.net.Infrastructure
{
    public class ScssTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            var compiler = new SassAndCoffee.Ruby.Sass.SassCompiler();

            response.ContentType = "text/css";
            response.Content = string.Empty;

            foreach (var fileInfo in response.Files)
            {
                if (fileInfo.VirtualFile.Name.EndsWith(".sass", StringComparison.Ordinal) || fileInfo.VirtualFile.Name.EndsWith(".scss", StringComparison.Ordinal))
                {
                    string path = Path.Combine(context.HttpContext.Server.MapPath("~/Temp"), Path.GetTempFileName());
                    try
                    {

                        using (Stream dest = File.OpenWrite(path))
                        {
                            using (Stream source = fileInfo.VirtualFile.Open())
                            {
                                source.CopyTo(dest);
                            }
                        }

                        response.Content += compiler.Compile(path, false, new List<string>());
                    }
                    finally
                    {
                        if (File.Exists(path) == true)
                        {
                            File.Delete(path);
                        }
                    }
                }
                else if (fileInfo.VirtualFile.Name.EndsWith(".css", StringComparison.Ordinal))
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