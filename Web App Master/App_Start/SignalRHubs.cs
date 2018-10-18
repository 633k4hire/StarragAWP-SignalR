
using Helpers;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Web_App_Master.Models;
using static Web_App_Master.Models.Data_Models;

namespace Web_App_Master.App_Start
{
    public class SignalRHubs
    {
        public static class UserHandler //this static class is to store the number of  users conected at the same time
        {
            public static HashSet<string> ConnectedIds = new HashSet<string>();
        }
        public static class ClientHandler //this static class is to store the number of  users conected at the same time
        {
            public static HashSet<string> ConnectedIds = new HashSet<string>();
            public static HashSet<ClientData> ClientDatas = new HashSet<ClientData>();
            public static HashSet<ClientData> AdminClientDatas = new HashSet<ClientData>();
        }
        [HubName("hsacHub")]
        public class HsacHub : Hub
        {

            public override Task OnConnected() //override OnConnect, OnReconnected and OnDisconnected  to know if a user is connected or disconnected 
            {

                UserHandler.ConnectedIds.Add(Context.ConnectionId); //add a connection id to the list 
                Clients.All.usersConnected(UserHandler.ConnectedIds.Count()); //this will send to ALL the clients  the number of users connected 


                return base.OnConnected();
            }

            public override Task OnReconnected()
            {
                UserHandler.ConnectedIds.Add(Context.ConnectionId);
                Clients.All.usersConnected(UserHandler.ConnectedIds.Count());
                return base.OnConnected();
            }

            public override Task OnDisconnected(bool stopCalled)
            {
                UserHandler.ConnectedIds.Remove(Context.ConnectionId);
                Clients.All.usersConnected(UserHandler.ConnectedIds.Count());
                return base.OnDisconnected(stopCalled);
            }

            public void sendMsg(string name, string message)
            {
                // Call the broadcastMessage method to update clients.
                Clients.Others.incomingMsg(name, message);
            }
            public void sendData(Data_Base_Class data, string message)
            {
                // Call the broadcastMessage method to update clients.
                Clients.All.broadcastMessage(data, message);
            }

            public void pushToOthers(Data_Base_Class data)
            {

                //Create a function here that can be called in the javascript of your page (dataPushed)
                this.Clients.Others.dataPushed(data);

            }

            public void pushToAll(Data_Base_Class data)
            {

                //Create a function here that can be called in the javascript of your page (dataPushedAll)
                this.Clients.All.dataPushed(data);

            }


        }

        [HubName("clientHub")]
        public class ClientHub : HsacHub
        {
            public override Task OnConnected() //override OnConnect, OnReconnected and OnDisconnected  to know if a user is connected or disconnected 
            {
                ClientData cd = new ClientData();
                cd.Id = Context.ConnectionId;
                try
                {
                    //HttpContext.Current.Session["SignalRid"] = cd.Id;

                    cd.Data = null;
                    var user = Context.QueryString["user"];
                    if (user != "admin")
                    {
                        ClientHandler.ClientDatas.Add(cd);
                        this.Groups.Add(cd.Id, "clients");
                    }
                    else
                    {
                        ClientHandler.AdminClientDatas.Add(cd);
                        this.Groups.Add(cd.Id, "admins");
                    }
                    ClientHandler.ConnectedIds.Add(Context.ConnectionId); //add a connection id to the list 
                    Clients.Group("admins").usersConnected(ClientHandler.ConnectedIds.Count()); //this will send to ALL the clients  the number of users connected 
                }
                catch
                {
                }
                return base.OnConnected();
            }

            public override Task OnReconnected()
            {
                ClientData cd = new ClientData();
                cd.Id = Context.ConnectionId;
                cd.Data = null;
                ClientHandler.ClientDatas.Add(cd);
                ClientHandler.ConnectedIds.Add(Context.ConnectionId);
                Clients.Group("admins").usersConnected(ClientHandler.ConnectedIds.Count());
                return base.OnConnected();
            }

            public override Task OnDisconnected(bool stopCalled)
            {
                var client = (from c in ClientHandler.ClientDatas where c.Id == Context.ConnectionId select c).FirstOrDefault();
                ClientHandler.ClientDatas.Remove(client);
                ClientHandler.ConnectedIds.Remove(Context.ConnectionId);
                Clients.Group("admins").usersConnected(ClientHandler.ConnectedIds.Count());
                super("end connection", "noarg1", "noarg2");
                return base.OnDisconnected(stopCalled);
            }

            /// <summary>
            /// Receives Images From Clients
            /// </summary>
            /// <param name="input">Base64 input string for Image</param>
            public void ClientImage(string input)

            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(input)))
                    {
                        Image returnImage = Image.FromStream(ms);

                        var clientId = Context.ConnectionId;

                        var client = ClientHandler.ClientDatas.FindById(Context.ConnectionId);

                        client.Data = Convert.ToBase64String(returnImage.ToArray());

                        returnImage.Dispose();

                        this.Clients.Others.imgReady(client);
                    }

                    this.Clients.Caller.imgRecved("ok");
                }
                catch
                {
                    this.Clients.Caller.imgRecved("error");
                }
            }

            public void pollMenuItems(string input)
            {               
                var pi = Account.AssetController.PollNavItems();
                try
                {
                    

                    this.Clients.Caller.pollItemsReady(pi);
                }
                catch
                {
                    this.Clients.Caller.pollItemsReady(new PollItems());
                }
            }

            public void changeAssetCache(object input)
            {
                this.Clients.All.assetCacheChanged();
            }

            /// <summary>
            /// Passes Super Command
            /// </summary>
            /// <param name="command"></param>
            /// <param name="arg1"></param>
            /// <param name="arg2"></param>
            public void super(string command, string arg1, string arg2)
            {
                this.Clients.Group("admins").onSuper(command, arg1, arg2);
                // this.Clients.All.onSuper(command, arg1, arg2);
            }

        }

        [HubName("nodeHub")]
        public class NodeHub : HsacHub
        {

        }
        [HubName("serverHub")]
        public class ServerHub : HsacHub
        {

        }
    }

}