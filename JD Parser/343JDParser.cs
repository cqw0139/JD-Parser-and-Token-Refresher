using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections;
using ResponseType;
using ApiMethod;

namespace MS_343_JD_Parser
{

    class newContact
    {
        public string firstName;
        public string lastName;
        public int companyId = 529769;
        public int statusId = 7984;

        public newContact(string inputName)
        {
            string[] seperator = { " " };
            string[] name = inputName.ToString().Split(seperator, 2, StringSplitOptions.RemoveEmptyEntries);
            firstName = name[0];
            lastName = name[1];
        }
    }

    class JDParser
    {
        public static string parsing(StringBuilder text, string contactToken)
        {
            // initialize many fields which only be used for parsing the 343 format Job Description
            string title = "Title:";
            string hiringManager = "Hiring Manager:";
            string startDate = "Start Date:";
            string asap = "ASAP";
            string endDate = "End Date:";
            string withPossibilityBlaBla = " (w/possibility of extension)";
            string numOfPositions = "# of positions to fill:";
            string redmondB4 = "Redmond Town Center B4";
            string Location = "Location:";
            string redmond = "Redmond";
            string agencyBillRate = "Agency Bill Rate:";
            string hourDependingOnExp = "hour depending on experience";
            string billRate = "Bill rate MUST include Microsoft facility fees.";
            string[] payReplacementCheck = { billRate, hourDependingOnExp, "$", "/" };
            string pay = "";
            string[] lowAndHigh = { };
            string jobDescription = "Job Description";
            string jobId = "Job ID #";


            string[] skillSet1 = { "SQL", "python", "java", "PowerShell", "javascript", "matlab", "julia", "ruby", "cshell", "csharp", "http", "css", "vba" };
            string[] skillSet2 = { "c++", "c#" };

            string[] january = { "jan", "JAN", "janu", "JANU", "january", "JANUARY" };
            string[] february = { "feb", "FEB", "febr", "FEBR", "february", "FEBRUARY" };
            string[] march = { "mar", "MAR", "MARC", "marc", "march", "MARCH" };
            string[] April = { "apr", "APR", "apri", "APRI", "april", "APRIL" };
            string[] may = { "may", "MAY" };
            string[] june = { "jun", "JUN", "june", "JUNE" };
            string[] july = { "jul", "JUL", "july", "JULY" };
            string[] august = { "aug", "AUG", "augu", "AUGU", "august", "AUGUST" };
            string[] september = { "sep", "SEP", "sept", "SEPT", "september", "SEPTEMBER" };
            string[] october = { "oct", "OCT", "octo", "OCTO", "october", "OCTOBER" };
            string[] november = { "nov", "NOV", "nove", "NOVE", "november", "NOVEMBER" };
            string[] december = { "dec", "DEC", "dece", "DECE", "decemebr", "DECEMBER" };
            bool monthReaded = false;

            string contactName = "";

            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();
            TimeSpan diff = new TimeSpan();

            apiMethod apiMethod = new apiMethod();

            dynamic jobOrder = new System.Dynamic.ExpandoObject();

            jobOrder.ownerUserId = 214994;
            ArrayList recuriter = new ArrayList();
            recuriter.Add(215227);
            jobOrder.recuriterUserId = recuriter;
            jobOrder.source = "Existing client";
            jobOrder.workTypeId = 9445;

            jobOrder.companyId = 529769;
            jobOrder.statusId = 38753;
            dynamic skill = new System.Dynamic.ExpandoObject();
            skill.matchAll = true;
            skill.tags = new ArrayList();
            jobOrder.skillTags = skill;

            dynamic salary = new System.Dynamic.ExpandoObject();
            ArrayList custom = new ArrayList();

            //start scanning the text of 343 format job description
            using (StringReader reader = new StringReader(text.ToString()))
            {
                string line = string.Empty;
                line = reader.ReadLine();
                while (true)
                {
                    //parse job title
                    if (line.ToString().Equals(string.Empty) == false)
                    {
                        if (line.ToString().Equals(title))
                        {
                            line = reader.ReadLine();
                        }
                        else
                        {
                            string rawTitle = line.ToString().Replace(title, "").Trim();
                            rawTitle = rawTitle.ToString().Replace(jobId, "").Trim();
                            string[] numAndTitle = rawTitle.Split('-');
                            if (numAndTitle.Length > 1)
                            {
                                jobOrder.jobTitle = numAndTitle[1];
                            }
                            else
                            {
                                jobOrder.jobTitle = numAndTitle[0];
                            }
                            break;
                        }
                    }
                    line = reader.ReadLine();
                }

                line = reader.ReadLine();

                while (line != null)
                {
                    // parse the name and email of contact person
                    if (line.ToString().Contains(hiringManager))
                    {
                        if (line.ToString().Equals(hiringManager))
                        {
                            line = reader.ReadLine();
                            while(line.ToString().Equals(string.Empty))
                            {
                                line = reader.ReadLine();
                            }
                            contactName = line.ToString();
                        }
                        else
                        {
                            contactName = line.ToString().Replace(hiringManager, "").Trim();
                        }
                        line = reader.ReadLine();
                    }
                    //parse the start date of job
                    else if (line.ToString().Equals(startDate))
                    {
                        line = reader.ReadLine();
                        while (true)
                        {
                            if (line.ToString().Equals(String.Empty) == false)
                            {
                                break;
                            }
                            line = reader.ReadLine();
                        }
                        if (line.ToString().Equals(asap))
                        {
                            dynamic start = new System.Dynamic.ExpandoObject();
                            start.immediate = true;
                            jobOrder.start = start;
                            startTime = DateTime.Now;
                        }
                        else
                        {
                            startTime = DateTime.Now;
                        }
                    }
                    //parse the end date of job
                    else if (line.ToString().Equals(endDate))
                    {
                        line = reader.ReadLine();
                        while (true)
                        {
                            if (line.ToString().Equals(String.Empty) == false)
                            {
                                break;
                            }
                            line = reader.ReadLine();
                        }
                        //fuzzy matchcing for the end date
                        if (line.ToString().Contains(withPossibilityBlaBla))
                        {
                            string end = line.ToString().Replace(" (w/possibility of extension)", "").Trim();
                            string[] monthAndYear = end.Split(' ');

                            foreach (string str in january)
                            {
                                if (monthAndYear[0].Equals(str))
                                {
                                    endTime = DateTime.ParseExact(monthAndYear[1] + "Jan", "yyyyMMM", null);
                                    monthReaded = true;
                                    endTime = endTime.AddDays(29);
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in february)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Feb", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(27);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in march)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Mar", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in April)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Apr", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in may)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "May", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in june)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Jun", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in july)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Jul", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in august)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Aug", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in september)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Sep", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in october)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Oct", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in november)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Nov", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            if (monthReaded == false)
                            {
                                foreach (string str in december)
                                {
                                    if (monthAndYear[0].Equals(str))
                                    {
                                        endTime = DateTime.ParseExact(monthAndYear[1] + "Dec", "yyyyMMM", null);
                                        monthReaded = true;
                                        endTime = endTime.AddDays(29);
                                    }
                                }
                            }

                            dynamic duration = new System.Dynamic.ExpandoObject();
                            diff = endTime - startTime;
                            if (diff.Days > 100)
                            {
                                duration.unit = "Month";
                                if (diff.Days > 3000)
                                {
                                    duration.period = 100;
                                }
                                else
                                {
                                    duration.period = diff.Days / 30;
                                }
                            }
                            else
                            {
                                duration.unit = "Day";
                                duration.period = diff.Days;
                            }
                            jobOrder.duration = duration;
                        }
                    }
                    //parse the number of job
                    else if (line.ToString().Contains(numOfPositions))
                    {
                        line = reader.ReadLine();
                        while (true)
                        {
                            if (line.ToString().Equals(String.Empty) == false)
                            {
                                break;
                            }
                            line = reader.ReadLine();
                        }
                        try
                        {
                            jobOrder.numberOfJobs = Int32.Parse(line.ToString());
                        }
                        catch (FormatException)
                        {
                            jobOrder.numberOfJobs = 1;
                        }
                    }
                    //parse the work location in job description
                    else if (line.ToString().Equals(Location))
                    {
                        line = reader.ReadLine();
                        while (true)
                        {
                            if (line.ToString().Equals(String.Empty) == false)
                            {
                                break;
                            }
                            line = reader.ReadLine();
                        }
                        if (line.ToString().Contains(redmondB4))
                        {
                            jobOrder.workplaceAddressId = "3e97c233-2bc0-4e78-8399-b5e006d911fd";
                        }
                        else if (line.ToString().Contains(redmond))
                        {
                            jobOrder.workplaceAddressId = "923d5855-48af-4445-b2d4-406a4756ca52";
                        }
                    }
                    //parse the bill rate
                    else if (line.ToString().Contains(agencyBillRate))
                    {
                        line = reader.ReadLine();
                        while (true)
                        {
                            if (line.ToString().Contains(hourDependingOnExp))
                            {
                                pay = line.ToString();
                                foreach (string str in payReplacementCheck)
                                {
                                    pay = pay.Replace(str, "").Trim();
                                }
                                dynamic custom1 = new System.Dynamic.ExpandoObject();
                                custom1.fieldId = 2;
                                custom1.type = "Text";
                                custom1.value = pay;
                                custom.Add(custom1);
                                jobOrder.custom = custom;
                                break;
                            }
                            line = reader.ReadLine();
                        }
                    }
                    //parse the job description
                    else if (line.ToString().Contains(jobDescription))
                    {
                        jobOrder.jobDescription = "";
                        line = reader.ReadLine();
                        while (line != null)
                        {
                            if (line.ToString().Equals(String.Empty) == true)
                            {
                            }
                            else
                            {
                                foreach (string requiredSkill in skillSet1)
                                {
                                    if (line.ToString().IndexOf(requiredSkill, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        jobOrder.skillTags.tags.Add(requiredSkill);
                                    }
                                }
                                foreach (string requiredSkill in skillSet2)
                                {
                                    if (line.ToString().IndexOf(requiredSkill, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        jobOrder.skillTags.tags.Add("\"" + requiredSkill + "\"");
                                    }
                                }
                                jobOrder.jobDescription += line.ToString() + "<br>";
                            }
                            line = reader.ReadLine();
                        }

                    }

                    if (line != null)
                    {
                        line = reader.ReadLine();
                    }

                }

                if (string.Equals(contactName, ""))
                    {
                    }
                    else
                    {

                        FindContactResponse contactResponse = apiMethod.findContactByName(contactName, contactToken);

                        if (contactResponse.totalCount == 0)
                        {
                            // POST a new contact

                            string contactJson = Newtonsoft.Json.JsonConvert.SerializeObject(new newContact(contactName)); ;

                            HttpClient contactClient = new HttpClient();
                            contactClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            contactClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", contactToken);

                            var contactBody = new StringContent(contactJson, Encoding.UTF8, "application/json");
                            var contactResult = contactClient.PostAsync(apiMethod.ContactURL, contactBody).Result;

                            contactResponse = apiMethod.findContactByName(contactName, contactToken);
                        }
                        else
                        {
                        }
                        contactItem contactItem = new contactItem(contactResponse.items.GetValue(0).ToString());
                        jobOrder.contactId = contactItem.contactId;
                    }

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(jobOrder);

                Console.WriteLine(json);

                return json;

            }

        }
    }
}
