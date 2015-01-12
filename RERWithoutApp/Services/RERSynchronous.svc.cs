using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
namespace RERWithoutApp
{
    public class RERSynchronous : IRemoteEventService
    {
        #region IRemoteEventService implements
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();
            if (properties.ItemEventProperties.AfterProperties["TestChoice"].ToString() == "C1")
            {
                result.ChangedItemProperties.Add("Test1", "Changed because TestChoice was c1");
            }
            else if (properties.ItemEventProperties.AfterProperties["TestChoice"].ToString() == "C2")
            {
                result.ErrorMessage = "No can do because TestChoice was C2";
                result.Status = SPRemoteEventServiceStatus.CancelWithError;
            }
            return result;
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            //this event will not fire but must be here to satisfy IRemoteEventService
        }
        #endregion
    }
}
