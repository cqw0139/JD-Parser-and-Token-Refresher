using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Token_Refresher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string contactTokenInput = string.Empty;

        public string jobTokenInput = string.Empty;

        public string noteTokenInput = string.Empty;

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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void contactChanged(object sender, TextChangedEventArgs e)
        {
            contactTokenInput = contactToken.Text;
            
        }

        private void jobChanged(object sender, TextChangedEventArgs e)
        {
            jobTokenInput = jobToken.Text;
        }

        private void noteChanged(object sender, TextChangedEventArgs e)
        {
            noteTokenInput = noteToken.Text;
        }

        private void refresh(object sender, RoutedEventArgs e)
        {
            //initilization part
            int err = 0;

            string startupPath = Environment.CurrentDirectory;
            StringBuilder text = new StringBuilder(), tokenContent = new StringBuilder();
            string line = String.Empty;

            string grantType = "authorization_code";
            string clientId = "vzphsdtmvq4u3o4vs5pswnpk2a";
            string clientSecret = "7incwfo5uoxunb27otsrfcrelux7osmmj36tguzlcmkvb3iz432m";

            string url = "https://id.jobadder.com/connect/token";

            string ContactURL = "https://api.jobadder.com/v2/contacts";
            string JobURL = "https://api.jobadder.com/v2/jobs";
            string NoteURL = "https://api.jobadder.com/v2/jobs/{jobId}/notes";

            //scan the token file, keep all the token whcih do note need to be refreshed
            try
            {
                object path = startupPath + @"\Token.txt";

                text.Append(File.ReadAllText(path.ToString()));

                using (StringReader reader = new StringReader(text.ToString()))
                {
                    line = reader.ReadLine();
                    while (line != null)
                    {
                        if (line.ToString().Contains("NoteRefreshToken:"))
                        {
                            if (noteTokenInput.Equals(string.Empty))
                            {
                                tokenContent.Append(line.ToString() + Environment.NewLine);
                            }
                        }
                        else if (line.ToString().Contains("ContactRefreshToken:"))
                        {
                            if (contactTokenInput.Equals(string.Empty))
                            {
                                tokenContent.Append(line.ToString() + Environment.NewLine);
                            }
                        }
                        else if (line.ToString().Contains("JobRefreshToken:"))
                        {
                            if (jobTokenInput.Equals(string.Empty))
                            {
                                tokenContent.Append(line.ToString()+ Environment.NewLine);
                            }
                        }
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception excep)
            {
                err = 1;
            }

            //refresh note token
            if (noteTokenInput.Equals(string.Empty) == false && err != 1)
            {
                try
                {
                    var AccessRequest1 = (HttpWebRequest)WebRequest.Create(url);
                    var noteData = "client_id=" + clientId;
                    noteData += "&client_secret=" + clientSecret;
                    noteData += "&grant_type=" + grantType;
                    noteData += "&code=" + noteTokenInput;
                    noteData += "&redirect_uri=" + NoteURL;
                    var data1 = Encoding.ASCII.GetBytes(noteData);

                    AccessRequest1.Method = "POST";
                    AccessRequest1.ContentType = "application/x-www-form-urlencoded";
                    AccessRequest1.ContentLength = data1.Length;

                    using (var stream = AccessRequest1.GetRequestStream())
                    {
                        stream.Write(data1, 0, data1.Length);
                    }

                    var Response1 = (HttpWebResponse)AccessRequest1.GetResponse();
                    var ResponseString1 = new StreamReader(Response1.GetResponseStream()).ReadToEnd();

                    JsonSerializer serializer = new JsonSerializer();
                    refreshTokenResponse noteTokenResponse = new refreshTokenResponse(ResponseString1);

                    tokenContent.Append("NoteRefreshToken:" + noteTokenResponse.refresh_token + Environment.NewLine);
                }
                catch (Exception e2)
                {
                    err = 2;
                }
            }

            //refresh job token
            if (jobTokenInput.Equals(string.Empty) == false && err != 1 && err != 2)
            {
                try
                {
                    var AccessRequest2 = (HttpWebRequest)WebRequest.Create(url);
                    var jobData = "client_id=" + clientId;
                    jobData += "&client_secret=" + clientSecret;
                    jobData += "&grant_type=" + grantType;
                    jobData += "&code=" + jobTokenInput;
                    jobData += "&redirect_uri=" + JobURL;
                    var data2 = Encoding.ASCII.GetBytes(jobData);

                    AccessRequest2.Method = "POST";
                    AccessRequest2.ContentType = "application/x-www-form-urlencoded";
                    AccessRequest2.ContentLength = data2.Length;

                    using (var stream = AccessRequest2.GetRequestStream())
                    {
                        stream.Write(data2, 0, data2.Length);
                    }

                    var Response2 = (HttpWebResponse)AccessRequest2.GetResponse();
                    var ResponseString2 = new StreamReader(Response2.GetResponseStream()).ReadToEnd();

                    JsonSerializer serializer = new JsonSerializer();
                    refreshTokenResponse jobTokenResponse = new refreshTokenResponse(ResponseString2);

                    tokenContent.Append("JobRefreshToken:" + jobTokenResponse.refresh_token + Environment.NewLine);
                }
                catch (Exception e3)
                {
                    err = 3;
                }
            }
            
            //refresh contact token
            if (contactTokenInput.Equals(string.Empty) == false && err != 1 && err != 2 && err !=3)
            {
                try
                {
                    var AccessRequest3 = (HttpWebRequest)WebRequest.Create(url);
                    var contactData = "client_id=" + clientId;
                    contactData += "&client_secret=" + clientSecret;
                    contactData += "&grant_type=" + grantType;
                    contactData += "&code=" + contactTokenInput;
                    contactData += "&redirect_uri=" + ContactURL;
                    var data3 = Encoding.ASCII.GetBytes(contactData);

                    AccessRequest3.Method = "POST";
                    AccessRequest3.ContentType = "application/x-www-form-urlencoded";
                    AccessRequest3.ContentLength = data3.Length;

                    using (var stream = AccessRequest3.GetRequestStream())
                    {
                        stream.Write(data3, 0, data3.Length);
                    }

                    var Response3 = (HttpWebResponse)AccessRequest3.GetResponse();
                    var ResponseString3 = new StreamReader(Response3.GetResponseStream()).ReadToEnd();

                    JsonSerializer serializer = new JsonSerializer();
                    refreshTokenResponse contactTokenResponse = new refreshTokenResponse(ResponseString3);

                    tokenContent.Append("ContactRefreshToken:" + contactTokenResponse.refresh_token + Environment.NewLine);
                }
                catch (Exception e4)
                {
                    err = 4;
                }
            }

            //post result in message box
            if (err == 0)
            {
                File.WriteAllText(startupPath + "/Token.txt", tokenContent.ToString());
                MessageBox.Show("successfully refreshed!");
            }
            else if(err == 1)
            {
                MessageBox.Show("Unexpected Token File Reading Error");
            }
            else if(err == 2)
            {
                MessageBox.Show("authorized code for note token is expired, please enter a new code to refresh the note token");
            }
            else if(err == 3)
            {
                MessageBox.Show("authorized code for job token is expired, please enter a new code to refresh the job token");
            }
            else if(err == 4)
            {
                MessageBox.Show("authorized code for contact token is expired, please enter a new code to refresh the contact token");
            }
        }
    }
}
