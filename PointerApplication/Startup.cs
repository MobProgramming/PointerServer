using System.Linq;
using Microsoft.Owin.Cors;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace PointerApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = Enumerable.Repeat("index.html", 1).ToList()
            });
            app.UseStaticFiles();
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}