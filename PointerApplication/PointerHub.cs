using System.Windows.Forms;
using Microsoft.AspNet.SignalR;

namespace PointerApplication
{
    public class PointerHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        public int GetMonitorCount()
        {
            return Screen.AllScreens.Length;
        }
    }
}