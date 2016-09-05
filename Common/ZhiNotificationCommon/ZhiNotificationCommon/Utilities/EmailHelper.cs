using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZhiNotificationCommon.Models;
namespace ZhiNotificationCommon.Utilities
{
    public class EmailHelper
    {
        public static writeResult SendEmailWithBronto(string to, BrontoMessage brontoMessageName, List<BrontoContent> brontoContents, BrontoEmailType brontoEmailType)
        {
            BrontoSoapApiImplService brontosoapAPI = new BrontoSoapApiImplService();

            BrontoSettings settings = BrontoSettings.GetBrontoSettings();
            string brontoApiKey = settings.ApiKey;
            string fromEmail = settings.FromEmail;
            string fromName = settings.FromName;
            string sessionId = brontosoapAPI.login(brontoApiKey);

            sessionHeader sessionHeader = new sessionHeader();
            sessionHeader.sessionId = sessionId;
            brontosoapAPI.sessionHeaderValue = sessionHeader;

            //Adding Contact
            contactObject[] contacts = new contactObject[1];
            contacts[0] = new contactObject() { email = to, status = brontoEmailType.ToString().ToLower() };
            writeResult addContactResult;
            addContactResult = brontosoapAPI.addOrUpdateContacts(contacts);
            if (addContactResult.errors != null)
            {
                return addContactResult;
            }

            //Get Messages 
            stringValue[] strValues = new stringValue[1];
            strValues[0] = new stringValue() { value = brontoMessageName.ToString(), @operator = filterOperator.EqualTo, operatorSpecified = true };
            messageFilter msgFilter = new messageFilter() { name = strValues };
            messageObject[] msgObjects;
            msgObjects = brontosoapAPI.readMessages(msgFilter, false, 1);
            if (msgObjects == null)
            {
                return null;
            }

            //Add Deliveries
            deliveryRecipientObject[] deliveryRecipients = new deliveryRecipientObject[1];
            deliveryRecipients[0] = new deliveryRecipientObject() { id = addContactResult.results[0].id, type = Constants.DeliveryRecipientType_Contact };
            deliveryObject[] deliveries = new deliveryObject[1];

            messageFieldObject[] messageFieldObjects = new messageFieldObject[brontoContents.Count];

            for (int i = 0; i < brontoContents.Count; i++)
            {
                messageFieldObjects[i] = new messageFieldObject
                {
                    name = brontoContents[i].Name,
                    type = brontoContents[i].Type,
                    content = brontoContents[i].Content
                };
            }

            deliveries[0] = new deliveryObject()
            {
                start = DateTime.UtcNow,
                startSpecified = true,
                messageId = msgObjects[0].id,
                fromName = fromName,
                fromEmail = fromEmail,
                recipients = deliveryRecipients,
                fields = messageFieldObjects,
                type = brontoEmailType.ToString().ToLower() // "triggered", "test"
            };

            writeResult addDeliveriesResult = brontosoapAPI.addDeliveries(deliveries);
            return addDeliveriesResult;
        }
    }
}