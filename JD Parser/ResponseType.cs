using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Json.Net;

namespace ResponseType
{
    // Response for api access request
    public class AccessTokenResponse
    {

        public string id_token { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string api { get; set; }
        public string instance { get; set; }
        public int account { get; set; }


        public AccessTokenResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            id_token = (string)jObject["id_token"];
            access_token = (string)jObject["access_token"];
            expires_in = (int)jObject["expires_in"];
            token_type = (string)jObject["token_type"];
            refresh_token = (string)jObject["refresh_token"];
            api = (string)jObject["api"];
            instance = (string)jObject["instance"];
            account = (int)jObject["account"];
        }
    }

    // Response for contact person check request(the item in list)
    public class contactItem
    {
        public int contactId { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }

        public contactItem(string json)
        {
            JObject jObject = JObject.Parse(json);
            contactId = (int)jObject["contactId"];
            firstName = (string)jObject["firstName"];
            lastName = (string)jObject["lastName"];
            email = (string)jObject["email"];
        }
    }

    // Response of contact person check request(list)
    public class FindContactResponse
    {

        public FindContactResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            totalCount = (int)jObject["totalCount"];
            items = jObject["items"].ToArray();
        }

        public Array items { get; set; }

        public int totalCount { get; set; }

    }

    // Response for adding new contact request
    public class AddContactResponse
    {
        public int contactId { get; set; }

        public AddContactResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            contactId = (int)jObject["contactId"];
        }
    }

    // The Response for posting new job request
    public class postJobResponse
    {
        public int jobId { get; set; }

        public postJobResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            jobId = (int)jObject["jobId"];
        }
    }

    // The response for refreshing the token
    public class refreshTokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string api { get; set; }
        public string instance { get; set; }
        public int account { get; set; }

        public refreshTokenResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            access_token = (string)jObject["access_token"];
            expires_in = (int)jObject["expires_in"];
            token_type = (string)jObject["token_type"];
            refresh_token = (string)jObject["refresh_token"];
            api = (string)jObject["api"];
            instance = (string)jObject["instance"];
            account = (int)jObject["account"];
        }
    }

}