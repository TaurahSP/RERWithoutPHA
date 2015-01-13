using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
namespace RERWithoutPHA
{
    public class RERSynchronous : IRemoteEventService
    {
        #region IRemoteEventService implements
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            // if this is a document library and this item is being updated, have a look at the value of the field before it was changed.
            // the BeforeProperties collection is not available for lists, it will be null
            string myFieldBefore = properties.ItemEventProperties.BeforeProperties["myField"].ToString();
            // Have a look at the changed fields.  Of Course you'll have to change "myField" to the name of a field in your list or document library
            string myFieldAfter = properties.ItemEventProperties.AfterProperties["myField"].ToString();
            // Maybe you'd like to change the value of a field in your list item
            result.ChangedItemProperties.Add("fieldIWantToChange", "My New value");

            //  maybe you don't like what's been done and want to tell SharePoint to cancel the save
            // comment in the lines below to see the save cancel
            //result.ErrorMessage = "No can do!";
            //result.Status = SPRemoteEventServiceStatus.CancelWithError;
            
            //YOU GET THE PICTURE

            return result;
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            //this event will not fire but must be here to satisfy IRemoteEventService
        }
        #endregion


    }
}
