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
                        clientContext.ExecuteQuery();

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
            eventReceiver.ReceiverAssembly = "RERWithoutApp";
            eventReceiver.ReceiverClass = "RERWithoutApp.Services.RERSynchronous";
            eventReceiver.ReceiverName = "RERSynchronous";
            eventReceiver.ReceiverUrl = "http://localhost:19393/Services/RERSynchronous.svc";
            eventReceiver.SequenceNumber = 1000;
            eventReceiver.Synchronization = synchronization;
            targetList.EventReceivers.Add(eventReceiver);
            clientContext.Web.Context.ExecuteQuery();
        }
    }
}
