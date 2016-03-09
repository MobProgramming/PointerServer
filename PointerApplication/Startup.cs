using System.Configuration;
using System.Linq;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Web.Http;

namespace PointerApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var rootDir = ConfigurationManager.AppSettings["rootFolder"];
            var physicalFileSystem = new PhysicalFileSystem(rootDir);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "index.html"
            };

            app.UseFileServer(options);
            app.UseCors(CorsOptions.AllowAll);
            app.UseFileServer(options);
        }
    }
}