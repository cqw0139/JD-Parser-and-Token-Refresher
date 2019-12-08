using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SkillSet;

namespace MSJD_NewParser
{
    class JDParser
    {
        public static string[] parsing(StringBuilder text, string contactToken)
        {

            //the following part is msjd parsing

            //field initialization
            string title = "- See Attachment for Full Details";
            string numOfOpen = "Number of openings:";
            string startDate = "Estimated start date:";
            string endDate = "Estimated end date:";
            string asap = "ASAP";
            string jobDescription = "Job Description";
            string confidentialYN = "Confidential Information";
            string performanceIndicator = "Performance indicators";
            string topCandidateSkill = "Top Candidate Skills";
            string top3MustHaveSkill = "Top 3 must-have hard skills";
            string additionalDetail = "Additional Detail";
            string billRateMax = "Bill rate max (if applicable):";
            string programContact = "Program office contact:";

            char[] blank = { ' ' };
           // string[] skillSet1 = { "SQL", "python", "java", "PowerShell", "javascript", "matlab", "julia", "ruby", "csharp", "http", "css", "vba" };
         //   string[] skillSet2 = { "c++", "c#" };




            //jobOrder object initialization

            dynamic jobOrder = new System.Dynamic.ExpandoObject();
            jobOrder.ownerUserId = 214994;
            ArrayList recuriter = new ArrayList();
            recuriter.Add(215227);
            jobOrder.recuriterUserId = recuriter;
            jobOrder.source = "Existing client";
            jobOrder.workTypeId = 9445;
            jobOrder.contactId = 4578452;
            jobOrder.companyId = 529769;
            jobOrder.statusId = 38753;
            jobOrder.jobTitle = "Default Title(Parsing job title failed)";

            dynamic skill = new System.Dynamic.ExpandoObject();
            skill.matchAll = true;
            skill.tags = new ArrayList();
            jobOrder.skillTags = skill;
            dynamic salary = new System.Dynamic.ExpandoObject();
            ArrayList custom = new ArrayList();

            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();
            TimeSpan diff = new TimeSpan();

            bool jobDescriptionCreated = false;

            string contact = "";

            //test flag
            bool visitHere = false;


            using (StringReader reader = new StringReader(text.ToString()))
            {
                string line = string.Empty;
                line = reader.ReadLine(); ;


                while (line != null)
                {

                    if (line.ToString().Contains(title))
                    {
                        jobOrder.jobTitle = line.Replace(title, "").Trim();
                    }
                    else if (line.ToString().Contains(numOfOpen))
                    {
                        try
                        {
                            jobOrder.numberOfJobs = Int32.Parse(line.ToString().Replace(numOfOpen, "").Trim());
                        }
                        catch (FormatException)
                        {
                            jobOrder.numberOfJobs = 1;
                        }
                    }
                    else if (line.ToString().Contains(startDate))
                    {

                        dynamic start = new System.Dynamic.ExpandoObject();
                        if (line.ToString().Contains(asap))
                        {
                            start.immediate = true;
                            jobOrder.start = start;
                            startTime = DateTime.Now;
                        }
                        else
                        {
                            string[] actualStartDate = line.ToString().Replace(startDate, "").Trim().Split(blank);
                            for (int attempt = 0; attempt < 2; attempt++)
                            {
                                try
                                {
                                    if (attempt == 0)
                                    {
                                        startTime = DateTime.ParseExact(actualStartDate[0], "MM/dd/yyyy", null);
                                    }
                                    else
                                    {
                                        startTime = DateTime.ParseExact(actualStartDate[0], "MM/d/yyyy", null);
                                    }
                                }
                                catch (Exception e)
                                {
                                    startTime = DateTime.Now;
                                }
                            }
                            start.date = startTime.ToString("yyyy-MM-dd");
                            jobOrder.start = start;
                        }
                    }
                    else if (line.ToString().Contains(endDate))
                    {

                        string[] actualEndDate = line.ToString().Replace(endDate, "").Trim().Split(blank);
                        for (int attempt = 0; attempt < 2; attempt++)
                        {
                            try
                            {
                                if (attempt == 0)
                                {
                                    endTime = DateTime.ParseExact(actualEndDate[0], "MM/dd/yyyy", null);
                                }
                                else
                                {
                                    endTime = DateTime.ParseExact(actualEndDate[0], "MM/d/yyyy", null);
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
                            catch (Exception e)
                            {

                            }
                        }
                    }
                    else if (line.ToString().Contains(jobDescription))
                    {
                        if (!jobDescriptionCreated)
                        {
                            jobOrder.jobDescription = "";
                            jobDescriptionCreated = true;
                            jobOrder.jobDescription += "Job Description:" + "<br>";

                        }
                        line = reader.ReadLine();
                        while (line != null)
                        {
                            if (line.ToString().Equals(String.Empty) == true)
                            {
                            }
                            else if (line.ToString().Contains(confidentialYN))
                            {
                                break;
                            }
                            else
                            {
                                foreach (string requiredSkill in SkillSet.SkillSet.skillSet1)
                                {
                                    if (line.ToString().IndexOf(requiredSkill, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        jobOrder.skillTags.tags.Add(requiredSkill);
                                    }
                                }
                                foreach (string requiredSkill in SkillSet.SkillSet.skillSet2)
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

                        jobOrder.jobDescription += "<br>";
                        jobOrder.jobDescription += "<br>";
                    }
                    else if (line.ToString().Contains(performanceIndicator))
                    {
                        if (!jobDescriptionCreated)
                        {
                            jobOrder.jobDescription = "";
                            jobDescriptionCreated = true;
                        }

                        while (line != null)
                        {
                            if (line.ToString().Equals(String.Empty) == true)
                            {
                            }
                            else if (line.ToString().Contains(topCandidateSkill))
                            {
                                break;
                            }
                            else
                            {
                                foreach (string requiredSkill in SkillSet.SkillSet.skillSet1)
                                {
                                    if (line.ToString().IndexOf(requiredSkill, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        jobOrder.skillTags.tags.Add(requiredSkill);
                                    }
                                }
                                foreach (string requiredSkill in SkillSet.SkillSet.skillSet2)
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

                        jobOrder.jobDescription += "<br>";
                        jobOrder.jobDescription += "<br>";
                    }
                    else if (line.ToString().Contains(top3MustHaveSkill))
                    {
                        if (!jobDescriptionCreated)
                        {
                            jobOrder.jobDescription = "";
                            jobDescriptionCreated = true;
                            jobOrder.jobDescription += line.ToString() + "<br>";
                        }

                        while (true)
                        {
                            if (line.ToString().Equals(String.Empty) == true)
                            {
                            }
                            else if (line.ToString().Contains(additionalDetail))
                            {
                                break;
                            }
                            else
                            {
                                foreach (string requiredSkill in SkillSet.SkillSet.skillSet1)
                                {
                                    if (line.ToString().IndexOf(requiredSkill, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        jobOrder.skillTags.tags.Add(requiredSkill);
                                    }
                                }
                                foreach (string requiredSkill in SkillSet.SkillSet.skillSet2)
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

                        jobOrder.jobDescription += "<br>";
                        jobOrder.jobDescription += "<br>";
                    }
                    else if (line.ToString().Contains(billRateMax))
                    {


                        string pay = line.ToString().Replace(billRateMax, "").Trim();
                        dynamic custom1 = new System.Dynamic.ExpandoObject();
                        custom1.fieldId = 2;
                        custom1.type = "Text";
                        custom1.value = pay;
                        custom.Add(custom1);
                        jobOrder.custom = custom;

                    }
                    else if (line.ToString().Contains(programContact))
                    {
                        contact = line.ToString();
                    }


                    if (line != null)
                    {
                        line = reader.ReadLine();
                    }


                }
            }


            // raw posting

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(jobOrder);

            string[] jsonAndContacts = new string[2];
            jsonAndContacts[0] = json;
            jsonAndContacts[1] = contact;

            return jsonAndContacts;
        }
    }
}
