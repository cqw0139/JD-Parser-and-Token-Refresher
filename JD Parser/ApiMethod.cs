using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using ResponseType;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ApiMethod
{
    
    class note
    {
        public string type { get; set; }
        public string text { get; set; }
    }

    class apiMethod
    {
        public string grantType = "refresh_token";

        public string clientId = "vzphsdtmvq4u3o4vs5pswnpk2a";
        public string clientSecret = "7incwfo5uoxunb27otsrfcrelux7osmmj36tguzlcmkvb3iz432m";


        public string AccessURL = "https://id.jobadder.com/connect/token";

        public string CurrentUserURL = "https://api.jobadder.com/v2/users/current";
        public string CategoryURL = "https://api.jobadder.com/v2/categories";
        public string ContactURL = "https://api.jobadder.com/v2/contacts";
        public string CountryURL = "https://api.jobadder.com/v2/countries";
        public string JobURL = "https://api.jobadder.com/v2/jobs";
        public string LocationURL = "https://api.jobadder.com/v2/locations";
        public string UserURL = "https://api.jobadder.com/v2/users";
        public string WorkTypeURL = "https://api.jobadder.com/v2/worktypes";
        public string CompanyAddressURL = "https://api.jobadder.com/v2/companies/{companyId}/addresses";
        public string NoteURL = "https://api.jobadder.com/v2/jobs/{jobId}/notes";

        public string NoteRefreshToken = string.Empty;
        public string CurrentUserRefreshToken = string.Empty;
        public string CategoryRefreshToken = string.Empty;
        public string ContactRefreshToken = string.Empty;
        public string CountryRefreshToken = string.Empty;
        public string JobRefreshToken = string.Empty;
        public string LocationRefreshToken = string.Empty;
        public string UserRefreshToken = string.Empty;
        public string WorkTypeRefreshToken = string.Empty;
        public string CompanyAddressRefreshToken = string.Empty;

        public apiMethod()
        {
            //get root path
            string startupPath = Environment.CurrentDirectory;
            string line = String.Empty;
            try
            {
                //read token file and get token information from root path

                StringBuilder text = new StringBuilder();
                object path = startupPath + @"\Token.txt";
                
                text.Append(File.ReadAllText(path.ToString()));

                using (StringReader reader = new StringReader(text.ToString()))
                {
                    line = reader.ReadLine();
                    while (line != null)
                    {
                        if (line.ToString().Contains("NoteRefreshToken:"))
                        {
                            NoteRefreshToken = line.ToString().Replace("NoteRefreshToken:", "").Trim();
                        }
                        else if (line.ToString().Contains("ContactRefreshToken:"))
                        {
                            ContactRefreshToken = line.ToString().Replace("ContactRefreshToken:", "").Trim();
                        }
                        else if (line.ToString().Contains("JobRefreshToken:"))
                        {
                            JobRefreshToken = line.ToString().Replace("JobRefreshToken:", "").Trim();
                        }
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        //use refresh token and call JobAdder api to get the access token
        public String getAccessToken(string url, string refreshToken)
        {

            var AccessRequest = (HttpWebRequest)WebRequest.Create(url);
            var postData = "client_id=" + clientId;
            postData += "&client_secret=" + clientSecret;
            postData += "&grant_type=" + grantType;
            postData += "&refresh_token=" + refreshToken;
            var data = Encoding.ASCII.GetBytes(postData);

            AccessRequest.Method = "POST";
            AccessRequest.ContentType = "application/x-www-form-urlencoded";
            AccessRequest.ContentLength = data.Length;

            using (var stream = AccessRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var AccessResponse = (HttpWebResponse)AccessRequest.GetResponse();
            var AccessResponseString = new StreamReader(AccessResponse.GetResponseStream()).ReadToEnd();

            JsonSerializer serializer = new JsonSerializer();
            AccessTokenResponse accessTokenResponse = new AccessTokenResponse(AccessResponseString);

            return accessTokenResponse.access_token;
        }

        //use email as key word to check whether exist a contact person who use the input email
        public FindContactResponse findContactByEmail(string contactEmail, string accessToken)
        {
            var getContactUri = new Uri(ContactURL + "?email=" + contactEmail);
            var contactWebRequest = WebRequest.Create(getContactUri);
            var contactHttpWebRequest = (HttpWebRequest)contactWebRequest;
            contactHttpWebRequest.PreAuthenticate = true;
            contactHttpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            contactHttpWebRequest.Accept = "application/json";

            var contactWebResponse = contactWebRequest.GetResponse();
            var responseStream = contactWebResponse.GetResponseStream();

            var myStreamReader = new StreamReader(responseStream, Encoding.Default);
            var contactResponseString = myStreamReader.ReadToEnd();

            responseStream.Close();
            contactWebResponse.Close();

            JsonSerializer serializer = new JsonSerializer();
            FindContactResponse contactResponse = new FindContactResponse(contactResponseString);
            return contactResponse;
        }

        //use name as key word to check whether exist a contact person who use the input name
        public FindContactResponse findContactByName(string contactName, string accessToken)
        {
            var getContactUri = new Uri(ContactURL + "?name=" + contactName);
            var contactWebRequest = WebRequest.Create(getContactUri);
            var contactHttpWebRequest = (HttpWebRequest)contactWebRequest;
            contactHttpWebRequest.PreAuthenticate = true;
            contactHttpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            contactHttpWebRequest.Accept = "application/json";

            var contactWebResponse = contactWebRequest.GetResponse();
            var responseStream = contactWebResponse.GetResponseStream();

            var myStreamReader = new StreamReader(responseStream, Encoding.Default);
            var contactResponseString = myStreamReader.ReadToEnd();

            responseStream.Close();
            contactWebResponse.Close();

            JsonSerializer serializer = new JsonSerializer();
            FindContactResponse contactResponse = new FindContactResponse(contactResponseString);
            return contactResponse;
        }


        //post job request with the input json string
        public int sendJob(string json)
        {
            string accessToken = getAccessToken(AccessURL, JobRefreshToken);

            HttpClient jobClient = new HttpClient();
            jobClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            jobClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var jobBody = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = jobClient.PostAsync(JobURL, jobBody).Result;
            string responseString = response.Content.ReadAsStringAsync().Result;

            JsonSerializer serializer = new JsonSerializer();
            postJobResponse postJobResponse = new postJobResponse(responseString);

            return postJobResponse.jobId;
        }

        //add note information on the job whose jobid is input jobid
        public void addNote(int jobId, string noteText)
        {
            
            note note = new note();
            note.type = "Contact Person on JD";
            note.text = noteText;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(note);

            string accessToken = getAccessToken(AccessURL, NoteRefreshToken);

            HttpClient jobClient = new HttpClient();
            jobClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            jobClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var jobBody = new StringContent(json, Encoding.UTF8, "application/json");

            string actualNoteURL = NoteURL.Replace("{jobId}", jobId.ToString());

            HttpResponseMessage response = jobClient.PostAsync(actualNoteURL, jobBody).Result;
        }

    }
}

