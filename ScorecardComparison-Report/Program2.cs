using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using System.DirectoryServices;
using System.Linq;

namespace ScorecardComparison_Report
{
    //Author : Chidvilash Vakada
    //Date Written : 9th April 2019

    /// <summary>
    /// Generate the Email Reprots based on App.Config File queries
    /// </summary>
    /// 

    class Program2
    {
        public static double xCount = 0;

        public static Dictionary<string, int> diffSheerCount = new Dictionary<string, int>();
        public static List<string> datesInFilePathList = new List<string>();
        public static string fridayComparedReport = null;
        public static string fridayComparedReportDate = null;
        public static string fridayComparedReportWeekName = null;
        public static string weeklyReportComapredReport = null;
        public static DataSet ds = new DataSet("Results");
        public static string EstimatedStartDateTracking = "EstimatedStartDateTracking";
        public static string InProcessAppsTracking = "InProcessAppsTracking";
        public static string MasBugsData = "MasBugsData";
        public static string TotalBugsSummary = "TotalBugsSummary";
        public static string TotalAppNames = "TotalAppNames";
        public static string SeverityWiseBugsSummary = "SeverityWiseBugsSummary";
        public static string ApplicationWiseDependencyBugs = "ApplicationWiseDependencyBugs";
        public static string ApplicationWiseSeverityBugs = "ApplicationWiseSeverityBugs";
        public static string ApplicationWiseAgingData = "ApplicationWiseAgingData";
        public static string SeverityWiseAgingData = "SeverityWiseAgingData";
        public static string ResolvedBugsData = "ResolvedBugsData";
        public static string Sev1Sev2ActiveNewBugs = "Sev1Sev2ActiveNewBugs";
        public static string ApplicationNameWiseSev1andSev2Bugs = "ApplicationNameWiseSev1andSev2Bugs";
        public static string ApplicationNameWiseData = "ApplicationNameWiseData";
        public static string ApplicationList = "ApplicationList";

        static void Main(string[] args)
        {
            GetUserEmail("Cory Joseph");

            /**************************
             * Upload Data from local path to local Database Starting
             *********************************/
            // uploadBugsToDb();
            /**************************
             * Upload Data from local path to local Database Ending
             *********************************/
            /**************************
            * Connect Local DB and Get Data From Local DB Starting
            *********************************
           Console.WriteLine("Connecting Local Or Live Db Based On Type and get Data from Required Query.....!");
           //System.Data.DataTable dtInventoryTbRaw = connectAndGetDataFromDB("MasBugsData", ConfigurationManager.AppSettings["MasBugsData"], 2);
           ds.Tables.Add(dtInventoryTbRaw);*/

            string applicationType = ConfigurationManager.AppSettings["ApplicationType"];

            if ("1".Equals(applicationType))
            {
                /**************************
                 * Upload Data from local path to local Database Starting
                 *********************************/
                uploadResolvedBugsDataToDb();
                /**************************
                 * Upload Data from local path to local Database Ending
                 *********************************/
                System.Data.DataTable dtTotalAppNames = connectAndGetDataFromDB("TotalAppNames", ConfigurationManager.AppSettings["TotalAppNames"], ConfigurationManager.AppSettings["DataBaseType"]);

                foreach (DataRow dr in dtTotalAppNames.Rows)
                {
                    foreach (DataColumn dc in dtTotalAppNames.Columns)
                    {
                        DataSet ds1 = new DataSet("Results");
                        string currentAppName = dr[dc].ToString();
                        System.Data.DataTable dtInventoryTbSummary = connectAndGetDataFromDB("TotalBugsSummary", ConfigurationManager.AppSettings["TotalBugsSummary"].Replace("ApplicationName", currentAppName), ConfigurationManager.AppSettings["DataBaseType"]);
                        System.Data.DataTable dtInventoryTbSeverityBugs = connectAndGetDataFromDB("SeverityWiseBugsSummary", ConfigurationManager.AppSettings["SeverityWiseBugsSummary"].Replace("ApplicationName", currentAppName), ConfigurationManager.AppSettings["DataBaseType"]);
                        // System.Data.DataTable dtInventoryTbApplicationWiseDependencyBugs = connectAndGetDataFromDB("ApplicationWiseDependencyBugs", ConfigurationManager.AppSettings["ApplicationWiseDependencyBugs"], ConfigurationManager.AppSettings["DataBaseType"]);
                        //  System.Data.DataTable dtInventoryTbApplicationWiseSeverityBugs = connectAndGetDataFromDB("ApplicationWiseSeverityBugs", ConfigurationManager.AppSettings["ApplicationWiseSeverityBugs"], ConfigurationManager.AppSettings["DataBaseType"]);
                        //  System.Data.DataTable dtInventoryTbApplicationWiseAgingData = connectAndGetDataFromDB("ApplicationWiseAgingData", ConfigurationManager.AppSettings["ApplicationWiseAgingData"], ConfigurationManager.AppSettings["DataBaseType"]);
                        //  System.Data.DataTable dtInventoryTbSeverityWiseAgingData = connectAndGetDataFromDB("SeverityWiseAgingData", ConfigurationManager.AppSettings["SeverityWiseAgingData"], ConfigurationManager.AppSettings["DataBaseType"]);
                        //   System.Data.DataTable dtInventoryTbResolvedBugsData = connectAndGetDataFromDB("ResolvedBugsData", ConfigurationManager.AppSettings["ResolvedBugsData"], ConfigurationManager.AppSettings["DataBaseType"]);
                        ds1.Tables.Add(dtInventoryTbSummary);
                        ds1.Tables.Add(dtInventoryTbSeverityBugs);
                        //  ds.Tables.Add(dtInventoryTbApplicationWiseDependencyBugs);
                        //   ds.Tables.Add(dtInventoryTbApplicationWiseSeverityBugs);
                        //   ds.Tables.Add(dtInventoryTbApplicationWiseAgingData);
                        //   ds.Tables.Add(dtInventoryTbSeverityWiseAgingData);
                        //  ds.Tables.Add(dtInventoryTbResolvedBugsData);
                        Console.WriteLine(" Results Data table added to Data Table .....!");
                        SendMasBugsDataByTitle("AllResolvedBugs", applicationType, ds1);
                    }
                }
                /**************************
                * Connect Local DB and Get Data From Local DB Ending
                *********************************/
            }
            else if ("2".Equals(applicationType))
            {
                Console.WriteLine(" Results Data table added to Data Table .....!");
                uploadActiveAndNewBugsDataToDb();
                System.Data.DataTable dtSev1Sev2ActiveNewBugs = connectAndGetDataFromDB("Sev1Sev2ActiveNewBugs", ConfigurationManager.AppSettings["Sev1Sev2ActiveNewBugs"], ConfigurationManager.AppSettings["DataBaseType"]);
                System.Data.DataTable dtApplicationNameWiseSev1andSev2Bugs = connectAndGetDataFromDB("ApplicationNameWiseSev1andSev2Bugs", ConfigurationManager.AppSettings["ApplicationNameWiseSev1andSev2Bugs_All"], ConfigurationManager.AppSettings["DataBaseType"]);
                System.Data.DataTable dtApplicationList = connectAndGetDataFromDB("ApplicationList", ConfigurationManager.AppSettings["ApplicationList_All"], ConfigurationManager.AppSettings["DataBaseType"]);
                ds.Tables.Add(dtSev1Sev2ActiveNewBugs);
                ds.Tables.Add(dtApplicationNameWiseSev1andSev2Bugs);
                ds.Tables.Add(dtApplicationList);
                SendMasBugsDataByTitle("AllActiveNewBugs_All", applicationType, ds);
            }




            /*
             * Est Date Tracker Details
             */
            //Console.WriteLine("Connecting the AIRT Live DB.....!");
            // System.Data.DataTable dtInventoryTbRaw = GetDataFromDB("Est Date Tracker", ConfigurationManager.AppSettings["AIRTEstTrackQuery"]);
            //dtInventoryTbRaw.TableName = EstimatedStartDateTracking;
            //ds.Tables.Add(dtInventoryTbRaw);
            //Console.WriteLine("Est Date Tracker Data Updated In DataTable");

            /*
             * In Process App Details
             */
            //System.Data.DataTable dtInPorcessTbRaw = GetDataFromDB("InProcess Tracker", ConfigurationManager.AppSettings["AIRTSqlQueryInProgress"]);
            //dtInPorcessTbRaw.TableName = InProcessAppsTracking;
            //ds.Tables.Add(dtInPorcessTbRaw);
            //SendMail();

        }
        

public string GetUserEmail(string UserId)
    {

        var searcher = new DirectorySearcher("LDAP://" + UserId.Split('\\').First().ToLower())
        {
            Filter = "(&(ObjectClass=person)(sAMAccountName=" + UserId.Split('\\').Last().ToLower() + "))"
        };

        var result = searcher.FindOne();
        if (result == null)
            return string.Empty;

        return result.Properties["mail"][0].ToString();

    }

        //Get Logged in user email address


        public static void SendMasBugsDataByTitle(string title, String type, DataSet ds)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 3.2//EN\">");
            builder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            builder.Append("<head>");
            builder.Append("<title>");
            builder.Append(ConfigurationManager.AppSettings[title].ToString() + new DateTime());
            builder.Append("</title>");
            builder.Append("<style type=\"text/css\">");
            builder.Append("table.MsoNormalTable{font-size:10.0pt;font-family:\"Calibri\",serif;}" +
                "p.MsoNormal{margin-bottom:.0001pt;font-size:12.0pt;font-family:\"Calibri\",serif;margin-left: 0in;margin-right: 0in;margin-top: 0in;}" +
                "h1{margin-right:0in;margin-left:0in;font-size:24.0pt;font-family:\"Calibri\",serif;font-weight:bold;}" +
                "a:link{color:#0563C1;text-decoration:underline;text-underline:single;}p{margin-right:0in;margin-left:0in;font-size:12.0pt;font-family:\"Calibri\",serif;}");
            builder.Append("table.imagetable{font-family: verdana,Calibri,sans-serif;font-size:11px;" +
                "color:#333333;border-width: 1px;border-color: #999999;border-collapse: collapse;}");
            builder.Append("table.imagetable th {background:#0070C0 url('cell-blue.jpg');border-width: 1px;padding: 8px;border-style: solid;border-color: #999999;}");
            builder.Append("table.imagetable td {background:#FFFFFF url('cell-grey.jpg');border-width: 1px;" +
                "padding: 8px;border-style: solid;border-color: #999999;width:1px;white-space:nowrap;}</style>");
            builder.Append("</head>");
            builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\">Hi All,</span><br>");
            if ("1".Equals(type))
            {
                builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\">Below are Resolved, bugs in all CSEO group applications.</span><br><br>");
                builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\">Please take these bugs with the respective lead, get the application access and start working on them.</span><br><br>");
                builder.Append("<span style=\"font - size:11pt; \"><a href=\"https://apc01.safelinks.protection.outlook.com/?url=https%3A%2F%2Fmsit.powerbi.com%2Fgroups%2F3bb1acb0-9ece-4448-bcf1-98eb7bb5c561%2Freports%2F7f380259-78d1-4c4b-ab8d-9702f1058dfc%2FReportSection41bd3940e89d4e584371&amp;data=02%7C01%7Cvenkatakrishn.sabbe%40hcl.com%7C095f5faf4ff749e72eda08d73dace238%7C189de737c93a4f5a8b686f4ca9941912%7C0%7C0%7C637045685278829929&amp;sdata=bouSYeGeaoDtEgD%2F6LCJMMKyANGNGZX7bSFnzhrwDL0%3D&amp;reserved=0\" target=\"_blank\" rel=\"noopener noreferrer\" data-auth=\"Verified\" originalsrc=\"https://msit.powerbi.com/groups/3bb1acb0-9ece-4448-bcf1-98eb7bb5c561/reports/7f380259-78d1-4c4b-ab8d-9702f1058dfc/ReportSection41bd3940e89d4e584371\" shash=\"qazPglr/V4TMHTLHuas3WEojPw+lJs1/Mr5XK9eXCkC3+qAB+ZWUueb5ZekeFUxqixWpPNpYQbhvqUfxnzyt4vf0kX530RXATRVL8jEqW3zJbF7KTRNCjIKa/gN2E7CXV6UbAONb8C2h+ikO4wl1xv27xV1/KdzjdwA+REnULp8=\" title=\"Original URL: https://msit.powerbi.com/groups/3bb1acb0-9ece-4448-bcf1-98eb7bb5c561/reports/7f380259-78d1-4c4b-ab8d-9702f1058dfc/ReportSection41bd3940e89d4e584371" +
                "Click or tap if you trust this link.\">Click here </a> for Resolved bugs PowerBI Report. </span> ");
            }
            else if ("2".Equals(type))
            {
                builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\">Below are Severity 1 bugs ageing more than 30 days and Severity 2 aging more than 60 days.</span><br><br>");
                builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\">Please take necessary action for compliance grading of the application.</span>");
            }
            builder.Append("<span style=\"mso-fareast-font-family:&quot;Times New Roman&quot;\"><u5:p></u5:p><o:p></o:p></span><br>");
            builder.Append("<u5:p></u5:p>");
            builder.Append("<body>");
            builder.Append("<br/>");
            //if (type == 3) { 
            //    builder.Append("<table class=\"imagetable\">");
            //    builder.Append("<tr bgcolor=\"#0F2BDE\">");
            //    builder.Append("<th><font color=\"#FFFFFF\">RecId</font></th>" +
            //        "<th><font color=\"#FFFFFF\">AppName</font></th>" +
            //        "<th><font color=\"#FFFFFF\">BugId</font></th>" +
            //        "<th><font color=\"#FFFFFF\">State</font></th>" +
            //        "<th><font color=\"#FFFFFF\">Severity</font></th>" +
            //        "<th><font color=\"#FFFFFF\">MasRule</font></th>" +
            //        "<th><font color=\"#FFFFFF\">Created By</font></th>" +
            //        "<th><font color=\"#FFFFFF\">Created Date</font></th>" +
            //        "<th><font color=\"#FFFFFF\">Assigned To</font></th>");
            //    builder.Append("</tr>");

            //    foreach (System.Data.DataTable table in ds.Tables)
            //    {
            //        if (table.TableName == MasBugsData)
            //        {
            //            foreach (DataRow dr in table.Rows)
            //            {
            //                builder.Append("<tr>");
            //                //mailTo += dr["CreatedBy"] +";";
            //                foreach (DataColumn dc in table.Columns)
            //                {       
            //                    builder.Append("<td>" + dr[dc].ToString() + "</td>");
            //                    Console.WriteLine(dr[dc].ToString());
            //                }
            //                builder.Append("</tr>");
            //            }
            //        }
            //        builder.Append("</table>");
            //    }
            //}else 
            if ("1".Equals(type))
            {
                foreach (System.Data.DataTable table in ds.Tables)
                {
                    if (table.TableName == TotalBugsSummary)
                    {
                        builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Total Number of Bugs</b></span>");
                        builder.Append("<table class=\"imagetable\">");
                        builder.Append("<tr bgcolor=\"#0F2BDE\">");
                        builder.Append("<th><font color=\"#FFFFFF\">Total Number of Bugs</font></th>" +
                            "<th><font color=\"#FFFFFF\">Total Number of Applications</font></th>"
                            );
                        builder.Append("</tr>");
                        foreach (DataRow dr in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn dc in table.Columns)
                            {
                                builder.Append("<td>" + dr[dc].ToString() + "</td>");
                                Console.WriteLine(dr[dc].ToString());
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</table>");
                    }

                    if (table.TableName == SeverityWiseBugsSummary)
                    {
                        builder.Append("<br>");
                        builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Severity Wise All Resolved Bugs Summary – </b></span>");
                        builder.Append("<table class=\"imagetable\">");
                        builder.Append("<tr bgcolor=\"#0F2BDE\">");
                        builder.Append("<th><font color=\"#FFFFFF\">Severity</font></th>" +
                            "<th><font color=\"#FFFFFF\">Count</font></th>"
                            );
                        builder.Append("</tr>");
                        foreach (DataRow dr in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn dc in table.Columns)
                            {
                                builder.Append("<td>" + dr[dc].ToString() + "</td>");
                                Console.WriteLine(dr[dc].ToString());
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</table>");
                    }

                    if (table.TableName == ApplicationWiseDependencyBugs)
                    {
                        builder.Append("<br>");
                        builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Application Wise Dependency Resolved Bugs –</b></span>");
                        builder.Append("<table class=\"imagetable\">");
                        builder.Append("<tr bgcolor=\"#0F2BDE\">");
                        builder.Append("<th><font color=\"#FFFFFF\">Application Name</font></th>" +
                            "<th><font color=\"#FFFFFF\">1st Party</font></th>" +
                            "<th><font color=\"#FFFFFF\">3rd Party</font></th>" +
                            "<th><font color=\"#FFFFFF\">Core Dev</font></th>" +
                            "<th><font color=\"#FFFFFF\">Untagged</font></th>" +
                            "<th><font color=\"#FFFFFF\">Total</font></th>"
                            );
                        builder.Append("</tr>");
                        foreach (DataRow dr in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn dc in table.Columns)
                            {
                                builder.Append("<td>" + dr[dc].ToString() + "</td>");
                                Console.WriteLine(dr[dc].ToString());
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</table>");
                    }

                    if (table.TableName == ApplicationWiseSeverityBugs)
                    {
                        builder.Append("<br>");
                        builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Application Wise Severity Resolved Bugs – </b></span>");
                        builder.Append("<table class=\"imagetable\">");
                        builder.Append("<tr bgcolor=\"#0F2BDE\">");
                        builder.Append("<th><font color=\"#FFFFFF\">Application Name</font></th>" +
                            "<th><font color=\"#FFFFFF\">1- Critical</font></th>" +
                            "<th><font color=\"#FFFFFF\">2- High </font></th>" +
                            "<th><font color=\"#FFFFFF\">3- Medium</font></th>" +
                            "<th><font color=\"#FFFFFF\">4- Low</font></th>" +
                            "<th><font color=\"#FFFFFF\">Total</font></th>"
                            );
                        builder.Append("</tr>");
                        foreach (DataRow dr in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn dc in table.Columns)
                            {
                                builder.Append("<td>" + dr[dc].ToString() + "</td>");
                                Console.WriteLine(dr[dc].ToString());
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</table>");
                    }

                    if (table.TableName == ApplicationWiseAgingData)
                    {
                        builder.Append("<br>");
                        builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Application Wise Resolved Bugs Ageing – </b></span>");
                        builder.Append("<table class=\"imagetable\">");
                        builder.Append("<tr bgcolor=\"#0F2BDE\">");
                        builder.Append("<th><font color=\"#FFFFFF\">Application Name</font></th>" +
                            "<th><font color=\"#FFFFFF\"> <30 </ font></th>" +
                            "<th><font color=\"#FFFFFF\">30 To 60</font></th>" +
                            "<th><font color=\"#FFFFFF\">60 To 180</font></th>" +
                            "<th><font color=\"#FFFFFF\">>=180</font></th>" +
                            "<th><font color=\"#FFFFFF\">Total</font></th>"
                            );
                        builder.Append("</tr>");
                        foreach (DataRow dr in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn dc in table.Columns)
                            {
                                builder.Append("<td>" + dr[dc].ToString() + "</td>");
                                Console.WriteLine(dr[dc].ToString());
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</table>");
                    }

                    if (table.TableName == SeverityWiseAgingData)
                    {
                        builder.Append("<br>");
                        builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Severity Wise Resolved Bugs Ageing –</b></span>");
                        builder.Append("<table class=\"imagetable\">");
                        builder.Append("<tr bgcolor=\"#0F2BDE\">");
                        builder.Append("<th><font color=\"#FFFFFF\">Severity</font></th>" +
                            "<th><font color=\"#FFFFFF\"> <30 </ font></th>" +
                            "<th><font color=\"#FFFFFF\">30 To 60</font></th>" +
                            "<th><font color=\"#FFFFFF\">60 To 180</font></th>" +
                            "<th><font color=\"#FFFFFF\">>=180</font></th>" +
                            "<th><font color=\"#FFFFFF\">Total</font></th>"
                            );
                        builder.Append("</tr>");
                        foreach (DataRow dr in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn dc in table.Columns)
                            {
                                builder.Append("<td>" + dr[dc].ToString() + "</td>");
                                Console.WriteLine(dr[dc].ToString());
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</table>");
                    }

                    if (table.TableName == ResolvedBugsData)
                    {
                        builder.Append("<br>");
                        builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>All Resolved Bugs -</b></span>");
                        builder.Append("<table class=\"imagetable\">");
                        builder.Append("<tr bgcolor=\"#0F2BDE\">");
                        builder.Append("<th><font color=\"#FFFFFF\">ID</font></th>" +
                            "<th><font color=\"#FFFFFF\">RecID</font></th>" +
                            "<th><font color=\"#FFFFFF\">AppName</font></th>" +
                            "<th><font color=\"#FFFFFF\">DependencyType</font></th>" +
                            "<th><font color=\"#FFFFFF\">First Party Dependency</font></th>" +
                            "<th><font color=\"#FFFFFF\">Third Party Dependency</font></th>" +
                            "<th><font color=\"#FFFFFF\">Created By</font></th>" +
                            "<th><font color=\"#FFFFFF\">Assigned To</font></th>" +
                            "<th><font color=\"#FFFFFF\">Severity</font></th>" +
                            "<th><font color=\"#FFFFFF\">State</font></th>" +
                            "<th><font color=\"#FFFFFF\">Resolved_Date</font></th>" +
                            "<th><font color=\"#FFFFFF\">Link</font></th>" +
                            "<th><font color=\"#FFFFFF\">Area_Path</font></th>" +
                            "<th><font color=\"#FFFFFF\">Bug Ageing Based on Resolved Date</font></th>"
                            );
                        builder.Append("</tr>");
                        foreach (DataRow dr in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn dc in table.Columns)
                            {
                                builder.Append("<td>" + dr[dc].ToString() + "</td>");
                                Console.WriteLine(dr[dc].ToString());
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</table>");
                    }
                }
            }
            else if ("2".Equals(type))
            {
                builder.Append("<br>");
                builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Severity 1 and 2 Actvie and New Bugs Ageing -</b></span>");
                builder.Append("<table class=\"imagetable\">");
                builder.Append("<tr bgcolor=\"#0F2BDE\">");
                builder.Append("<th><font color=\"#FFFFFF\">Severity 1 bugs >30 Days</font></th>" +
                "<th><font color=\"#FFFFFF\">Severity 2 bugs >60 Days</font></th>"
                );
                builder.Append("</tr>");
                foreach (System.Data.DataTable table in ds.Tables)
                {
                    if (table.TableName == Sev1Sev2ActiveNewBugs)
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            builder.Append("<tr>");
                            foreach (DataColumn dc in table.Columns)
                            {
                                builder.Append("<td><center>" + dr[dc].ToString() + "</center></td>");
                                Console.WriteLine(dr[dc].ToString());
                            }
                            builder.Append("</tr>");
                        }
                        builder.Append("</table>");
                    }
                    List<String> applicationList = new List<String>();
                    if (table.TableName == ApplicationNameWiseSev1andSev2Bugs)
                    {
                        builder.Append("<br>");
                        builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Application Name Wise Severity1 and Severity2 Bugs Ageing -</b></span>");
                        builder.Append("<table class=\"imagetable\">");
                        builder.Append("<tr bgcolor=\"#0F2BDE\">");
                        builder.Append("<th><font color=\"#FFFFFF\">Application Name</font></th>" +
                        "<th><font color=\"#FFFFFF\">Severity 1(More than 30days) </font></th>" +
                        "<th><font color=\"#FFFFFF\">Severity 2(More than 60days) </font></th>"
                        );
                        builder.Append("</tr>");
                        foreach (DataRow dr in table.Rows)
                        {

                            if ((!"0".Equals(dr["Sev1"].ToString())) || (!"0".Equals(dr["Sev2"].ToString())))
                            {
                                builder.Append("<tr>");
                                applicationList.Add(dr["AppName"].ToString());
                                foreach (DataColumn dc in table.Columns)
                                {

                                    builder.Append("<td><center>" + dr[dc].ToString() + "</center></td>");
                                    Console.WriteLine(dr[dc].ToString());
                                }
                                builder.Append("</tr>");
                            }
                        }
                        builder.Append("</table>");
                    }


                    foreach (String applicationName in applicationList)
                    {
                        String dynamicQuery = "select ID,AppName,RecID,Severity,DependencyType,MAS_Rules,Created_Date,Created_By,DurationCreatedDate,Assigned_To,Area_Path,Link from dbo.NewAndActiveBugsData where((Severity = '1 - Critical' and DurationCreatedDate > 30) or(Severity = '2 - High' and DurationCreatedDate > 60)) and AppName = '" + applicationName + "' order by Severity asc";
                        System.Data.DataTable applicationNameWiseData = connectAndGetDataFromDB("ApplicationNameWiseData", dynamicQuery, ConfigurationManager.AppSettings["DataBaseType"]);
                        if (applicationNameWiseData != null && applicationNameWiseData.Rows.Count > 0)
                        {
                            builder.Append("<br>");
                            builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\"><b>Application Name: " + applicationName + "</b></span>");
                            builder.Append("<table class=\"imagetable\">");
                            builder.Append("<tr bgcolor=\"#0F2BDE\">");
                            builder.Append("<th><font color=\"#FFFFFF\">Bug Id</font></th>" +
                            "<th><font color=\"#FFFFFF\">Application Name</font></th>" +
                            "<th><font color=\"#FFFFFF\">RecID</font></th>" +
                            "<th><font color=\"#FFFFFF\">Severity</font></th>" +
                            "<th><font color=\"#FFFFFF\">Dependency Type</font></th>" +
                            "<th><font color=\"#FFFFFF\">MAS Rules</font></th>" +
                            "<th><font color=\"#FFFFFF\">Created_Date</font></th>" +
                            "<th><font color=\"#FFFFFF\">Created By</font></th>" +
                            "<th><font color=\"#FFFFFF\">Bug Ageing</font></th>" +
                            "<th><font color=\"#FFFFFF\">Assign to</font></th>" +
                            "<th><font color=\"#FFFFFF\">Area Path</font></th>" +
                            "<th><font color=\"#FFFFFF\">Bug Link</font></th>"
                            );
                            builder.Append("</tr>");
                            foreach (DataRow dr in applicationNameWiseData.Rows)
                            {
                                builder.Append("<tr>");
                                foreach (DataColumn dc in applicationNameWiseData.Columns)
                                {
                                    builder.Append("<td><center>" + dr[dc].ToString() + "</center></td>");
                                }
                                builder.Append("</tr>");
                            }
                            builder.Append("</table>");
                        }
                    }
                }
            }
            builder.Append("<p style=\"color:blue;\">*** <i>This is an automatically generated email</i> ***</p>");
            builder.Append("</body>");
            string HtmlFile = builder.ToString();
            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem oMsg = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            oMsg.To = "venkatakrishn.sabbe@hcl.com";//ConfigurationManager.AppSettings["SendMailTo"];
            oMsg.Subject = ConfigurationManager.AppSettings[title].ToString() + " : " + System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString();
            oMsg.HTMLBody = HtmlFile;
           // oMsg.CC = ConfigurationManager.AppSettings["SendMailCC"];
            oMsg.Send();
        }

        private static System.Data.DataTable connectAndGetDataFromDB(String tableName, string QueryName, String type)
        {
            Console.WriteLine("Enter to Get Data From Local Or Live Db");
            string dbConn = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            if ("1".Equals(type))
                dbConn = @"Data Source = airtproddbserver.database.windows.net; user id=; password=; Initial Catalog = AIRTProd;";
            else if ("2".Equals(type))
                dbConn = @"Data Source = localhost;Initial Catalog =OneITVSO Active MAS Bugs; Integrated Security=True";
            cmd.CommandText = QueryName;
            Console.WriteLine("Executes query Based on Query Type");
            SqlConnection sqlConnection1 = new SqlConnection(dbConn);
            cmd.Connection = sqlConnection1;
            sqlConnection1.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            sda.Fill(dt);
            dt.TableName = tableName;
            sqlConnection1.Close();
            Console.WriteLine("Return the all records in Data table formate");
            return dt;
        }

        private static void uploadBugsToDb()
        {
            Console.WriteLine("Store Records from Excel");
            SqlConnection SQLConnection = new SqlConnection();
            // SQLConnection.ConnectionString = "Data Source = airtproddbserver.database.windows.net; user id=; password=;Initial Catalog = AIRTProd; ";
            SQLConnection.ConnectionString = " Data Source = (local) ;Initial Catalog =OneITVSO Active MAS Bugs; "
               + "Integrated Security=true;";
            SqlCommand SqlCmd = new SqlCommand();
            SqlCmd.Connection = SQLConnection;
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", "E:\\Chinna\\Bugs_Data\\BulkData.xlsx");
            OleDbConnection Econ = new OleDbConnection(constr);
            string Query = string.Format("Select [RecID],[AppName],[Bug_Id],[State],[Severity],[Bug_Identified],[Test_Environment],[MAS_Rule]," +
                "[Duration_Created_Date],[CreatedBy],[Created_Date],[Assigned_To],[Area_Path],[Iteration_Path],[Title],[Link] FROM [{0}]", "data$");
            OleDbCommand Ecom = new OleDbCommand(Query, Econ);
            Econ.Open();
            DataSet ds = new DataSet();
            OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
            Econ.Close();
            oda.Fill(ds);
            DataTable Exceldt = ds.Tables[0];
            SqlBulkCopy objbulk = new SqlBulkCopy(SQLConnection);
            objbulk.DestinationTableName = "[dbo].[OneITVso_Active_New]";
            objbulk.ColumnMappings.Add("RecID", "RecID");
            objbulk.ColumnMappings.Add("AppName", "AppName");
            objbulk.ColumnMappings.Add("Bug_Id", "Bug_Id");
            objbulk.ColumnMappings.Add("State", "State");
            objbulk.ColumnMappings.Add("Severity", "Severity");
            objbulk.ColumnMappings.Add("Bug_Identified", "Bug_Identified");
            objbulk.ColumnMappings.Add("Test_Environment", "Test_Environment");
            objbulk.ColumnMappings.Add("MAS_Rule", "MAS_Rule");
            objbulk.ColumnMappings.Add("Duration_Created_Date", "Duration_Created_Date");
            objbulk.ColumnMappings.Add("CreatedBy", "CreatedBy");
            objbulk.ColumnMappings.Add("Created_Date", "Created_Date");
            objbulk.ColumnMappings.Add("Assigned_To", "Assigned_To");
            objbulk.ColumnMappings.Add("Area_Path", "Area_Path");
            objbulk.ColumnMappings.Add("Iteration_Path", "Iteration_Path");
            objbulk.ColumnMappings.Add("Title", "Title");
            objbulk.ColumnMappings.Add("Link", "Link");
            SQLConnection.Open();
            objbulk.WriteToServer(Exceldt);
            SQLConnection.Close();
        }

        private static void uploadResolvedBugsDataToDb()
        {
            //Truncate previous table data while inserting new records...
            truncateTable("[dbo].[resolvedBugsData]", 2);
            Console.WriteLine("Store Resolved Records from Excel");
            SqlConnection SQLConnection = new SqlConnection();
            // SQLConnection.ConnectionString = "Data Source = airtproddbserver.database.windows.net; user id=; password=;Initial Catalog = AIRTProd; ";
            SQLConnection.ConnectionString = " Data Source = (local) ;Initial Catalog =OneITVSO Active MAS Bugs; "
               + "Integrated Security=true;";
            SqlCommand SqlCmd = new SqlCommand();
            SqlCmd.Connection = SQLConnection;
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", "E:\\Chinna\\Bugs_Data\\All_Resolved_Bugs.xlsx");
            OleDbConnection Econ = new OleDbConnection(constr);
            string Query = string.Format("Select [ID],[RecID],[AppName],[DependencyType],[First_Party_Dependency],[Third_Party_Dependency],[Created_By],[Assigned_To]," +
                "[Severity],[State],[Resolved_Date],[Link],[Area_Path],[Bug_Ageing_Based_on_Resolved_Date] FROM [{0}]", "Sheet1$");
            OleDbCommand Ecom = new OleDbCommand(Query, Econ);
            Econ.Open();
            DataSet ds = new DataSet();
            OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
            Econ.Close();
            oda.Fill(ds);
            DataTable Exceldt = ds.Tables[0];
            SqlBulkCopy objbulk = new SqlBulkCopy(SQLConnection);
            objbulk.DestinationTableName = "[dbo].[resolvedBugsData]";
            objbulk.ColumnMappings.Add("ID", "ID");
            objbulk.ColumnMappings.Add("RecID", "RecID");
            objbulk.ColumnMappings.Add("AppName", "AppName");
            objbulk.ColumnMappings.Add("DependencyType", "DependencyType");
            objbulk.ColumnMappings.Add("First_Party_Dependency", "First_Party_Dependency");
            objbulk.ColumnMappings.Add("Third_Party_Dependency", "Third_Party_Dependency");
            objbulk.ColumnMappings.Add("Created_By", "Created_By");
            objbulk.ColumnMappings.Add("Assigned_To", "Assigned_To");
            objbulk.ColumnMappings.Add("Severity", "Severity");
            objbulk.ColumnMappings.Add("State", "State");
            objbulk.ColumnMappings.Add("Resolved_Date", "Resolved_Date");
            objbulk.ColumnMappings.Add("Link", "Link");
            objbulk.ColumnMappings.Add("Area_Path", "Area_Path");
            objbulk.ColumnMappings.Add("Bug_Ageing_Based_on_Resolved_Date", "Bug_Ageing_Based_on_Resolved_Date");
            SQLConnection.Open();
            objbulk.WriteToServer(Exceldt);
            SQLConnection.Close();
        }

        private static void uploadActiveAndNewBugsDataToDb()
        {
            //Truncate previous table data while inserting new records...
            truncateTable("[dbo].[NewAndActiveBugsData]", 2);
            Console.WriteLine("Table Truncated Successfully");
            Console.WriteLine("Store Active and New Records from Excel");
            SqlConnection SQLConnection = new SqlConnection();
            // SQLConnection.ConnectionString = "Data Source = airtproddbserver.database.windows.net; user id=; password=;Initial Catalog = AIRTProd; ";
            SQLConnection.ConnectionString = " Data Source = (local) ;Initial Catalog =OneITVSO Active MAS Bugs; "
               + "Integrated Security=true;";
            SqlCommand SqlCmd = new SqlCommand();
            SqlCmd.Connection = SQLConnection;
            string constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", "E:\\Chinna\\Bugs_Data\\NewAndActiveBugsData.xlsx");
            OleDbConnection Econ = new OleDbConnection(constr);
            string Query = string.Format("Select [ID],[RecID],[AppName],[State],[Severity],[Bug_Identified]" +
               ",[Test_Environment],[DependencyType],[First_Party_Dependency],[Third_Party_Dependency]" +
               ",[MAS_Rules],[DurationCreatedDate],[Created_By],[Created_Date],[Assigned_To]," +
                "[Area_Path],[Iteration_Path],[Title],[Link],[TodayDate] " +
                "FROM [{0}]", "Sheet1$");
            OleDbCommand Ecom = new OleDbCommand(Query, Econ);
            Econ.Open();
            DataSet ds = new DataSet();
            OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
            Econ.Close();
            oda.Fill(ds);
            DataTable Exceldt = ds.Tables[0];
            SqlBulkCopy objbulk = new SqlBulkCopy(SQLConnection);
            objbulk.DestinationTableName = "[dbo].[NewAndActiveBugsData]";
            objbulk.ColumnMappings.Add("ID", "ID");
            objbulk.ColumnMappings.Add("RecID", "RecID");
            objbulk.ColumnMappings.Add("AppName", "AppName");
            objbulk.ColumnMappings.Add("State", "State");
            objbulk.ColumnMappings.Add("Severity", "Severity");
            objbulk.ColumnMappings.Add("Bug_Identified", "Bug_Identified");
            objbulk.ColumnMappings.Add("Test_Environment", "Test_Environment");
            objbulk.ColumnMappings.Add("DependencyType", "DependencyType");
            objbulk.ColumnMappings.Add("First_Party_Dependency", "First_Party_Dependency");
            objbulk.ColumnMappings.Add("Third_Party_Dependency", "Third_Party_Dependency");
            objbulk.ColumnMappings.Add("MAS_Rules", "MAS_Rules");
            objbulk.ColumnMappings.Add("DurationCreatedDate", "DurationCreatedDate");
            objbulk.ColumnMappings.Add("Created_By", "Created_By");
            objbulk.ColumnMappings.Add("Created_Date", "Created_Date");
            objbulk.ColumnMappings.Add("Assigned_To", "Assigned_To");
            objbulk.ColumnMappings.Add("Area_Path", "Area_Path");
            objbulk.ColumnMappings.Add("Iteration_Path", "Iteration_Path");
            objbulk.ColumnMappings.Add("Title", "Title");
            objbulk.ColumnMappings.Add("Link", "Link");
            objbulk.ColumnMappings.Add("TodayDate", "TodayDate");
            SQLConnection.Open();
            objbulk.WriteToServer(Exceldt);
            SQLConnection.Close();
        }

        #region Connect AIRT DB
        private static System.Data.DataTable GetDataFromDB(String tableName, string QueryName)
        {
            Console.WriteLine("Enter to GetDataFormDB method and calling the Micrsoft AIRT DB");
            string dbConn = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            dbConn = @"Data Source = airtproddbserver.database.windows.net; user id=; password=; Initial Catalog = AIRTProd;";
            //cmd.CommandText = "Select * from Inventory";
            cmd.CommandText = QueryName;
            Console.WriteLine("Executed Inventory table query");
            SqlConnection sqlConnection1 = new SqlConnection(dbConn);
            cmd.Connection = sqlConnection1;
            sqlConnection1.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            System.Data.DataTable dt = new System.Data.DataTable();
            sda.Fill(dt);
            dt.TableName = tableName;
            sqlConnection1.Close();
            Console.WriteLine("Return the all inventory reconds in Data table formate");
            return dt;
        }
        #endregion

        private static void truncateTable(String tableName, int type)
        {

            Console.WriteLine("Truncate Table data from table :: " + tableName);
            string dbConn = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            if (type == 1)
                dbConn = @"Data Source = airtproddbserver.database.windows.net; user id=; password=; Initial Catalog = AIRTProd;";
            else if (type == 2)
                dbConn = @"Data Source = localhost;Initial Catalog =OneITVSO Active MAS Bugs; Integrated Security=True";
            //cmd.CommandText = "Select * from Inventory";
            cmd.CommandText = "TRUNCATE TABLE " + tableName;
            Console.WriteLine("Executed Inventory table query");
            SqlConnection sqlConnection1 = new SqlConnection(dbConn);
            cmd.Connection = sqlConnection1;
            sqlConnection1.Open();
            cmd.ExecuteNonQuery();
            sqlConnection1.Close();
        }
        public static void SendMail()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 3.2//EN\">");
            builder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
            builder.Append("<head>");
            builder.Append("<title>");
            builder.Append(ConfigurationManager.AppSettings["ReportTitle"].ToString());
            builder.Append("</title>");
            builder.Append("<style type=\"text/css\">");
            builder.Append("table.MsoNormalTable{font-size:10.0pt;font-family:\"Calibri\",serif;}" +
                "p.MsoNormal{margin-bottom:.0001pt;font-size:12.0pt;font-family:\"Calibri\",serif;margin-left: 0in;margin-right: 0in;margin-top: 0in;}" +
                "h1{margin-right:0in;margin-left:0in;font-size:24.0pt;font-family:\"Calibri\",serif;font-weight:bold;}" +
                "a:link{color:#0563C1;text-decoration:underline;text-underline:single;}p{margin-right:0in;margin-left:0in;font-size:12.0pt;font-family:\"Calibri\",serif;}");
            builder.Append("table.imagetable{font-family: verdana,Calibri,sans-serif;font-size:11px;" +
                "color:#333333;border-width: 1px;border-color: #999999;border-collapse: collapse;}");
            builder.Append("table.imagetable th {background:#0070C0 url('cell-blue.jpg');border-width: 1px;padding: 8px;border-style: solid;border-color: #999999;}");
            builder.Append("table.imagetable td {background:#FFFFFF url('cell-grey.jpg');border-width: 1px;" +
                "padding: 8px;border-style: solid;border-color: #999999;width:1px;white-space:nowrap;}</style>");
            builder.Append("</head>");
            builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\">Hi All,</span><br>");
            builder.Append("<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\">Below are Resolved, bugs.</span>" +
                "<span style=\"mso-fareast-font-family:&quot;Times New Roman&quot;\"><u5:p></u5:p><o:p></o:p></span><br><br>"
                );
            builder.Append("<u5:p></u5:p>");
            builder.Append("<body>");
            //builder.Append("<span style=\"margin:0in;margin-bottom:.0001pt\">" +
            //    "<span style=\"font-size:11.0pt;mso-fareast-font-family:&quot;Times New Roman&quot;\">" +
            //    "<br/>We found "+ "<b>" + ds.Tables[0].Rows.Count+ "</b>" + " records. <br/></span></span>");
            //Display the AIRT tech reports row count this is useful to identified the which sheet get the changes
            builder.Append("<br/>");
            //builder.Append("<br/><h2 style=\"margin:0in;margin-bottom:.0001pt\"><span style=\"font-size:12.0pt;mso-fareast-font-family:&quot;Calibri&quot;\">Identified fields count -</span></h2>");
            builder.Append("<table class=\"imagetable\">");
            builder.Append("<tr>");
            builder.Append("<th><font color=\"#FFFFFF\">Orignal Group Name</font></th>" +
                "<th><font color=\"#FFFFFF\">Group</font></th>" +
                "<th><font color=\"#FFFFFF\">Full Sub Group Name</font></th>" +
                "<th><font color=\"#FFFFFF\">Sub Group</font></th>" +
                "<th><font color=\"#FFFFFF\">RecID</font></th>" +
                "<th><font color=\"#FFFFFF\">Digital Property</font></th>" +
                "<th><font color=\"#FFFFFF\">Link to AIRT</font></th>" +
                "<th><font color=\"#FFFFFF\">Link to Assessment Records</font></th>" +
                "<th><font color=\"#FFFFFF\">Link to ST</font></th>" +
                "<th><font color=\"#FFFFFF\">Component Or URL</font></th>" +
                "<th><font color=\"#FFFFFF\">Priority</font></th>" +
                "<th><font color=\"#FFFFFF\">Assessment Grade</font></th>" +
                "<th><font color=\"#FFFFFF\">Assessment Type</font></th>" +
                "<th><font color=\"#FFFFFF\">Assessment Status</font></th>" +
                "<th><font color=\"#FFFFFF\">Assessed By</font></th>" +
                "<th><font color=\"#FFFFFF\">EstStDate</font></th>" +
                "<th><font color=\"#FFFFFF\">EstEndDate</font></th>" +
                "<th><font color=\"#FFFFFF\">DateStarted</font></th>" +
                "<th><font color=\"#FFFFFF\">DateCompleted</font></th>" +
                "<th><font color=\"#FFFFFF\">Assessment Notes</font></th>" +
                "<th><font color=\"#FFFFFF\">Documents</font></th>"
                );
            builder.Append("</tr>");

            //foreach (System.Data.DataTable table in ds.Tables)
            //{
            /*if(table.TableName == EstimatedStartDateTracking)
            {
                foreach (DataRow dr in table.Rows)
                {
                    builder.Append("<tr>");
                    foreach (DataColumn dc in table.Columns)
                    {

                        builder.Append("<td>" + dr[dc].ToString() + "</td>");
                        Console.WriteLine(dr[dc].ToString());

                    }
                    builder.Append("</tr>");
                }
            }*/
            builder.Append("</table>");
            /*    
            if (table.TableName == InProcessAppsTracking)
            {
                builder.Append("<br/>");
                builder.Append("<br/><h2 style=\"margin:0in;margin-bottom:.0001pt\"><span style=\"font-size:12.0pt;mso-fareast-font-family:&quot;Calibri&quot;\">In Process Assessments</span></h2>");
                builder.Append("<table class=\"imagetable\">");
                builder.Append("<tr>");
                builder.Append("<th><font color=\"#FFFFFF\">RecID</font></th>" +
                    "<th><font color=\"#FFFFFF\">Assessment Method</font></th>" +
                    "<th><font color=\"#FFFFFF\">Group</font></th>" +
                    "<th><font color=\"#FFFFFF\">Sub Group</font></th>" +
                    "<th><font color=\"#FFFFFF\">Application</font></th>" +
                    "<th><font color=\"#FFFFFF\">Priority</font></th>" +
                    "<th><font color=\"#FFFFFF\">Assessment Activity</font></th>" +
                    "<th><font color=\"#FFFFFF\">Assessment Status</font></th>" +
                    "<th><font color=\"#FFFFFF\">Assessment Start Date</font></th>" +
                    "<th><font color=\"#FFFFFF\">Assessment End Date</font></th>" +
                    "<th><font color=\"#FFFFFF\">Assessment End Year</font></th>" +
                    //"<th><font color=\"#FFFFFF\">Assessment End Month</font></th>" +
                    "<th><font color=\"#FFFFFF\">Assessment End Month Name</font></th>"
                    );
                foreach (DataRow drInProcess in table.Rows)
                {
                    builder.Append("<tr>");
                    foreach (DataColumn dcInProcess in table.Columns)
                    {
                        builder.Append("<td>" + drInProcess[dcInProcess].ToString() + "</td>");
                        Console.WriteLine(drInProcess[dcInProcess].ToString());
                    }
                    builder.Append("</tr>");
                }
            }
            */
            //builder.Append("</table>");
            //}


            builder.Append("<p style=\"color:blue;\">*** <i>This is an automatically generated email</i> ***</p>");
            builder.Append("</body>");
            string HtmlFile = builder.ToString();
            Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem oMsg = (Microsoft.Office.Interop.Outlook.MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            oMsg.To = ConfigurationManager.AppSettings["SendMailTo"];
            oMsg.Subject = ConfigurationManager.AppSettings["ReportTitle"].ToString() + " : " + System.DateTime.Now.ToShortDateString() + " " + System.DateTime.Now.ToShortTimeString();
            oMsg.HTMLBody = HtmlFile;
            oMsg.CC = ConfigurationManager.AppSettings["SendMailCC"];
            oMsg.Send();
        }


    }

}