using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
namespace RERWithoutPHADeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            string spUrl = "your SP url";
            string webUrl = "your web site url";
            string listName = "the Name of your list";
            ClientContext cc = new ClientContext(spUrl);
            RegisterReceiver(cc, EventReceiverType.ItemAdding, EventReceiverSynchronization.Synchronous, listName, webUrl);
            RegisterReceiver(cc, EventReceiverType.ItemUpdating, EventReceiverSynchronization.Synchronous, listName, webUrl);
            RegisterReceiver(cc, EventReceiverType.ItemDeleting, EventReceiverSynchronization.Synchronous, listName, webUrl);
        }

        private static void RegisterReceiver(ClientContext clientContext, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, string listName, string webUrl)
        {
            List targetList = clientContext.Web.Lists.GetByTitle(listName);
            clientContext.Load(targetList);
           
            EventReceiverDefinitionCollection ec = targetList.EventReceivers;
            clientContext.Load(ec);
            clientContext.ExecuteQuery();
            
            //Get rid of old rer registration in the case that we are re-deploying
            for (int i = 0; i < ec.Count; i++)
            {
                if (ec[i].ReceiverName == "RERSynchronous" && !ec[i].ServerObjectIsNull.Value && ec[i].EventType == eventReceiverType)
                {
                    ec[i].DeleteObject();
                    clientContext.ExecuteQuery();
                    break;
                }
            }

            EventReceiverDefinitionCreationInformation eventReceiver = new EventReceiverDefinitionCreationInformation();
            eventReceiver.EventType = eventReceiverType;
            //eventReceiver.ReceiverAssembly = "RERWithoutPHA";
            //eventReceiver.ReceiverClass = "RERWithoutPHA.Services.RERSynchronous";
            eventReceiver.ReceiverName = "RERSynchronous";
            eventReceiver.ReceiverUrl = webUrl + "/Services/RERSynchronous.svc";
            eventReceiver.SequenceNumber = 1000;
            eventReceiver.Synchronization = synchronization;
            targetList.EventReceivers.Add(eventReceiver);
            clientContext.Web.Context.ExecuteQuery();
        }
    }
}
