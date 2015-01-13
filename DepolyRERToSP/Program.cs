using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
namespace DepolyRERToSP
{
    class Program
    {
        static void Main(string[] args)
        {
            RemoveAll();
            RegisterAll();
            
            ClientContext cc =  new ClientContext("http://win-ija7pajsek8:38370");
            RegisterReceiver(cc, EventReceiverType.ItemAdding, EventReceiverSynchronization.Synchronous);
            RegisterReceiver(cc, EventReceiverType.ItemUpdating, EventReceiverSynchronization.Synchronous);
            RegisterReceiver(cc, EventReceiverType.ItemDeleting, EventReceiverSynchronization.Synchronous);


        }

        private static void RegisterReceiver(ClientContext clientContext, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization)
        {
            List targetList = clientContext.Web.Lists.GetByTitle("Test");
            clientContext.Load(targetList);
           
            EventReceiverDefinitionCollection ec = targetList.EventReceivers;
            clientContext.Load(ec);
            clientContext.ExecuteQuery();
            
            for (int i = 0; i < ec.Count; i++)
            {
                if (ec[i].ReceiverName == "RERSynchronous" && !ec[i].ServerObjectIsNull.Value && ec[i].EventType == eventReceiverType)
                {

                    try
                    {
                        ec[i].DeleteObject();

                    }
                    catch (Exception ex)
                    {
                        //log the error but keep going
                    }
                }
            }
            clientContext.ExecuteQuery();

            EventReceiverDefinitionCreationInformation eventReceiver = new EventReceiverDefinitionCreationInformation();
            eventReceiver.EventType = eventReceiverType;
            //eventReceiver.ReceiverAssembly = "RERWithoutApp";
            //eventReceiver.ReceiverClass = "RERWithoutApp.Services.RERSynchronous";
            eventReceiver.ReceiverName = "RERSynchronous";
            eventReceiver.ReceiverUrl = "http://localhost:19393/Services/RERSynchronous.svc";
            eventReceiver.SequenceNumber = 1000;
            eventReceiver.Synchronization = synchronization;
            targetList.EventReceivers.Add(eventReceiver);
            clientContext.Web.Context.ExecuteQuery();
        }
        private static void RegisterAll()
        {
            string sharePointUrl = "http://win-ija7pajsek8:38370";
            string remoteWebUrl = "http://localhost:19393";
            string listName = "Test";

            ClientContext clientContext = new ClientContext(sharePointUrl);
            List targetList = clientContext.Web.Lists.GetByTitle(listName);
            clientContext.Load(targetList);
            EventReceiverDefinitionCollection ec = targetList.EventReceivers;
            clientContext.Load(ec);

            EventReceiverDefinitionCreationInformation eventReceiver = new EventReceiverDefinitionCreationInformation();
            eventReceiver.EventType = EventReceiverType.ItemUpdating;
            //The ReceiverAssembly and ReceiverClass properties, as far as I can tell, are not needed
            // the RER gets deployed and functions properly with or without them
            //eventReceiver.ReceiverAssembly = "RERWithoutApp";
            //eventReceiver.ReceiverClass = "RERWithoutApp.Services.RERSynchronous";
            eventReceiver.ReceiverName = "RERSynchronous";
            eventReceiver.ReceiverUrl = remoteWebUrl + "/Services/RERSynchronous.svc";
            eventReceiver.SequenceNumber = 1000;
            eventReceiver.Synchronization = EventReceiverSynchronization.Synchronous;
            targetList.EventReceivers.Add(eventReceiver);

            eventReceiver = new EventReceiverDefinitionCreationInformation();
            eventReceiver.EventType = EventReceiverType.ItemAdding;
            eventReceiver.ReceiverName = "RERSynchronous";
            eventReceiver.ReceiverUrl = remoteWebUrl + "/Services/RERSynchronous.svc";
            eventReceiver.SequenceNumber = 1000;
            eventReceiver.Synchronization = EventReceiverSynchronization.Synchronous;
            targetList.EventReceivers.Add(eventReceiver);

            eventReceiver = new EventReceiverDefinitionCreationInformation();
            eventReceiver.EventType = EventReceiverType.ItemDeleting;
            eventReceiver.ReceiverName = "RERSynchronous";
            eventReceiver.ReceiverUrl = remoteWebUrl + "/Services/RERSynchronous.svc";
            eventReceiver.SequenceNumber = 1000;
            eventReceiver.Synchronization = EventReceiverSynchronization.Synchronous;
            targetList.EventReceivers.Add(eventReceiver);
            
            clientContext.Web.Context.ExecuteQuery();
        }
        private static void RemoveAll()
        {
            string sharePointUrl = "http://win-ija7pajsek8:38370";
            string listName = "Test";

            ClientContext clientContext = new ClientContext(sharePointUrl);
            List targetList = clientContext.Web.Lists.GetByTitle(listName);
            clientContext.Load(targetList);
            EventReceiverDefinitionCollection ec = targetList.EventReceivers;
            clientContext.Load(ec);
            clientContext.ExecuteQuery();

            bool found = true;
            while (found)
            {
                found = false;
                for (int i = 0; i < ec.Count; i++)
                {
                    if (ec[i].ReceiverName == "RERSynchronous" && !ec[i].ServerObjectIsNull.Value)
                    {

                        ec[i].DeleteObject();
                        clientContext.ExecuteQuery();
                        found = true;
                        break;
                    }
                }
            }
        }
    }
}
