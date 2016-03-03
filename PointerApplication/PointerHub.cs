using Microsoft.AspNet.SignalR;

namespace PointerApplication
{
    public class PointerHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
    }
}