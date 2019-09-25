using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using ApiMethod;
using System.Net;
using Newtonsoft.Json;
using ResponseType;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections;

namespace MSJDParser
{

    class location
    {
        public int locationId;
    }

    class relative
    {
        public int period = 0;
        public string unit = "week";
    }

    public interface start { }

    class startWithDate : start
    {
        public string date;
    }

    class startWithRelative : start
    {
        public relative relative;
    }

    class startWithImmediate : start
    {
        public bool immediate;
    }

    class duration
    {
        public int period;
        public string unit;
    }


    public interface custom { }

    public class custom1 : custom
    {
        public int fieldId;
        public string value;
    }

    public class custom2 : custom
    {
        public int fieldId;
        public string[] value;
    }

    public class skillTags
    {
        public bool matchAll;
        public ArrayList tags;
        internal string count;
    }


    class JobOrder
    {
        public string jobTitle;
        public int companyId;
        public int contactId;
        public string jobDescription;
        public int statusId;
        public bool userFavourite;
        public location location;
        public string workplaceAddressId;
        public start start;
        public duration duration;
        public int workTypeId;
        public int numberOfJobs;
        public string source;
        public skillTags skillTags;
        public int ownerUserId;
        public ArrayList recuriterUserId;

    }

    
    class newContact
    {
        public string firstName;
        public string lastName;
        public string email;
        public int companyId = 529769;
        public int statusId = 7984;

        public newContact(string inputName, string inputEmail)
        {
            this.email = inputEmail;
            string[] seperator = { " " };
            string[] name = inputName.ToString().Split(seperator, 2, StringSplitOptions.RemoveEmptyEntries);
            firstName = name[0];
            lastName = name[1];
        }
    }


    class JDParser
    {
        public static string[] parsing(StringBuilder text, string contactToken){

            // initialize many fields which only be used for parsing the MSJD format Job Description
            string jobTitle = "Job Title";
            string estimateStartDate = "Estimated Start Date";
            string confidentialInfo = "Confidential Information (Y/N)";
            string jobDescription = "Job Description  Ensure Alignment to Selected Title";
            string estimatedEndDate = "Estimated End Date";
            string numOfOpening = "Number of Openings";
            string performanceIndicator = "Performance indicators";
            string top3MustHaveSkill = "Top 3 must-have hard skills";
            string stackRankedByImportance = "Stack-ranked by importance";
            string additionalDetailLogic = "Additional Details  Logistics";
            string programOfficeContact = "Program Office Contact";

            string[] skillSet1 = { "SQL", "python", "java", "PowerShell", "javascript", "matlab", "julia", "ruby", "csharp", "http", "css", "vba" };
            string[] skillSet2 = { "c++", "c#" };

            string contactName = "";
            string contactEmail = "";


            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            System.TimeSpan diff = new TimeSpan();


            JobOrder jobOrder = new JobOrder();

            jobOrder.contactId = 4578452;
            jobOrder.ownerUserId = 214994;
            ArrayList recuriter = new ArrayList();
            recuriter.Add(215227);
            jobOrder.recuriterUserId = recuriter;
            jobOrder.source = "Existing client";
            jobOrder.workTypeId = 9445;
            location location = new location();
            location.locationId = 153223;
            jobOrder.location = location;
            jobOrder.workplaceAddressId = "a8657bb0-a683-4291-ab3f-7c9a1d5d806a";
            jobOrder.companyId = 529769;
            jobOrder.statusId = 38753;
            skillTags skill = new skillTags();
            skill.matchAll = true;
            skill.tags = new ArrayList();
            jobOrder.skillTags = skill;

            bool visitHere = false;
            bool tryflag = true;

            //start scanning the input text
            using (StringReader reader = new StringReader(text.ToString()))
            {
                string line = string.Empty;
                line = reader.ReadLine();
                while (line != null)
                {
                    // parse the job title
                    if (string.Equals(jobTitle, line.ToString()))
                    {

                        line = reader.ReadLine();
                        jobOrder.jobTitle = line.ToString();

                    }
                    //parse the start date
                    else if (String.Equals(estimateStartDate, line.ToString()))
                    {

                        startWithDate start = new startWithDate();
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        line = reader.ReadLine();
                        if (line.ToString().Length > 1)
                        {
                            if (line.ToString().Contains("ASAP"))
                            {
                                startDate = DateTime.Now;
                            }
                            else
                            {
                                tryflag = true;
                                try {
                                    startDate = DateTime.Parse(line.ToString());
                                    tryflag = false;
                                }
                                catch (Exception e)
                                {

                                }
                                if (tryflag)
                                {
                                    try
                                    {
                                        startDate = DateTime.ParseExact(line.ToString(), "dd MMMM yyyy", null);
                                    }
                                    catch (FormatException)
                                    {
                                        startDate = DateTime.ParseExact(line.ToString(), "d MMMM yyyy", null);
                                    }
                                }
                            }
                            start.date = startDate.ToString("yyyy-MM-dd");
                        }
                        jobOrder.start = start;
                    }
                    //parse the end date
                    else if (string.Equals(estimatedEndDate, line.ToString()))
                    {
                        line = reader.ReadLine();
                        if (line.ToString().Length > 1)
                        {

                            tryflag = true;
                            try
                            {
                                endDate = DateTime.Parse(line.ToString());
                                tryflag = false;
                            }
                            catch (Exception e)
                            {

                            }
                            if (tryflag)
                            {
                                try
                                {
                                    endDate = DateTime.ParseExact(line.ToString(), "dd MMMM yyyy", null);
                                }
                                catch (FormatException)
                                {
                                    endDate = DateTime.ParseExact(line.ToString(), "d MMMM yyyy", null);
                                }
                            }

                            duration duration = new duration();
                            diff = endDate - startDate;
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
                    //parse the text in job description
                    else if (string.Equals(jobDescription, line.ToString()))
                    {
                        line = reader.ReadLine();
                        while (line != null)
                        {

                            if (string.Equals(line, confidentialInfo))
                            {
                                break;
                            }
                            else
                            {
                                jobOrder.jobDescription += line.ToString() + "<br>";
                            }

                            line = reader.ReadLine();
                        }
                    }
                    //parse the number of job opening
                    else if (string.Equals(numOfOpening, line.ToString()))
                    {
                        line = reader.ReadLine();
                        try
                        {
                            jobOrder.numberOfJobs = Int32.Parse(line.ToString());
                        }
                        catch (FormatException)
                        {
                            jobOrder.numberOfJobs = 1;
                        }
                    }
                    //parse the text in performance indicator and add them into job description field
                    else if (string.Equals(performanceIndicator, line.ToString()))
                    {
                        jobOrder.jobDescription += "<br>";
                        line = reader.ReadLine();
                        while (line != null)
                        {

                            if (string.Equals(line, top3MustHaveSkill))
                            {
                                break;
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
                    //parse the text in top 3 must have skill and add into job description field
                    else if (string.Equals(stackRankedByImportance, line.ToString()))
                    {
                        jobOrder.jobDescription += "<br>";
                        jobOrder.jobDescription += top3MustHaveSkill + "<br>";
                        line = reader.ReadLine();
                        while (line != null)
                        {

                            if (string.Equals(line, additionalDetailLogic))
                            {
                                break;
                            }
                            else
                            {
                                foreach (string requiredSkill in skillSet1)
                                {
                                    if (line.ToString().IndexOf(requiredSkill, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        jobOrder.skillTags.tags.Add(requiredSkill);
                                        visitHere = true;
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
                    //parse the name and email of contact person
                    else if (string.Equals(programOfficeContact, line.ToString()))
                    {
                        line = reader.ReadLine();
                        while (line == null)
                        {

                            line = reader.ReadLine();
                        }

                            if (line.ToString().Contains(", "))
                            {
                                string[] seperator = { ", " };
                                string[] splitLine = line.ToString().Split(seperator, 2, StringSplitOptions.RemoveEmptyEntries);
                                contactName = splitLine[0];
                                contactEmail = splitLine[1];
                            }
                            else if (line.ToString().Contains("@"))
                            {
                                contactEmail = line.ToString();
                                contactName = "";
                            }
                            else
                            {
                                contactName = line.ToString();
                                line = reader.ReadLine();
                                if (line.ToString().Contains("@"))
                                {
                                    contactEmail = line.ToString();
                                }
                                else
                                {
                                    contactName = "";
                                }
                            }

                        
                    }

                    if (line != null)
                    {
                        line = reader.ReadLine();

                    }

                }
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(jobOrder);

            string[] output = new string[2];
            output[0] = json;
            output[1] = "Name:" + contactName + ",  Email:" + contactEmail;

            return output;



        }


    }
}
