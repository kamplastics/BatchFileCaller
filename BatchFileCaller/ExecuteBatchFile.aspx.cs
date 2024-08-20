﻿using System;
using System.Diagnostics;

namespace YourNamespace
{
    public partial class ExecuteBatchFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Output the current user running the ASP.NET application
                string currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                Response.Write("<p>Current User: " + currentUser + "</p>");
                Response.Flush(); // Immediately flush the response to the client

                // Render the page structure with the output container
                RenderPageStructure();

                // Start streaming the batch file output
                RunBatchFile();
            }
        }

        private void RenderPageStructure()
        {
            // Render the basic HTML structure including the output container
            Response.Write("<!DOCTYPE html>");
            Response.Write("<html>");
            Response.Write("<head>");
            Response.Write("<title>Execute Batch File</title>");
            Response.Write("<style>#outputContainer { list-style-type: none; padding: 0; } #outputContainer li { margin-bottom: 5px; }</style>");
            Response.Write("</head>");
            Response.Write("<body>");
            Response.Write("<form id=\"form1\" runat=\"server\">");
            Response.Write("<ul id=\"outputContainer\"></ul>");
            Response.Write("</form>");
            Response.Write("</body>");
            Response.Write("</html>");

            // Flush the response to send the structure to the client immediately
            Response.Flush();
        }

        private void RunBatchFile()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = @"C:\scripts\Time_Series_Data_Transfer_Toolkit\hourly\0_6-23_s_s_s.bat",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = psi;

                    process.OutputDataReceived += new DataReceivedEventHandler((s, e) => SendOutputToClient(e.Data));
                    process.ErrorDataReceived  += new DataReceivedEventHandler((s, e) => SendOutputToClient(e.Data));

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                SendOutputToClient("Error: " + ex.Message);
            }
        }

        private void SendOutputToClient(string output)
        {
            if (!string.IsNullOrEmpty(output))
            {
                // Use JavaScript to prepend output as a list item to the output container
                string script = $"<script>document.getElementById('outputContainer').insertAdjacentHTML('afterbegin', '<li>{Server.HtmlEncode(output)}</li>');</script>";
                Response.Write(script);
                Response.Flush(); // Immediately flush the response to the client
            }
        }
    }
}
