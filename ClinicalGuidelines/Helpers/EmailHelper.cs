//Beacon Clinical Guidelines
//Copyright (C) 2019  University Hospitals Plymouth NHS Trust 
//
//You should have received a copy of the GNU Affero General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>. 
// 
// See LICENSE in the project root for license information.
﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using ClinicalGuidelines.Models;

namespace ClinicalGuidelines.Helpers
{
    public static class EmailHelper
    {
        public static void SendApprovalEmail(ArticleRevision articleRevision)
        {
            var _db = new ClinicalGuidelinesAppContext();
            var author = _db.Users.FirstOrDefault(a => a.Id == articleRevision.UserId);
            var approver = _db.Users.FirstOrDefault(ap => ap.UserName == articleRevision.ApprovalBy);
            // Create email message using stringbuilder
            var sb = new StringBuilder();
            sb.AppendLine($"<p>Dear {author.Forename} {author.Surname}, </p>");
            sb.AppendLine("<br>");
            sb.AppendLine($"<p>I have approved article - {articleRevision.Title}, please see my comments below.</p>");
            sb.AppendLine("<br>");
            sb.AppendLine($"<p>{articleRevision.ApprovalComments}</p>");
            sb.AppendLine("<br>");
            sb.AppendLine($"<p>Kind Regards, <br> {approver.Forename} {approver.Surname}</p>");

            var approvalMessage = sb.ToString();
            var approvalSubject = $"{articleRevision.Title} has been approved.";
            var mailFromAddress = ConfigurationManager.AppSettings["mailFromAddress"];

            SendMail(author.EmailAddress, mailFromAddress, approvalSubject, approvalMessage);
        }

        public static void SendRejectedEmail(ArticleRevision articleRevision)
        {
            var _db = new ClinicalGuidelinesAppContext();
            var author = _db.Users.FirstOrDefault(a => a.Id == articleRevision.UserId);
            var approver = _db.Users.FirstOrDefault(ap => ap.UserName == articleRevision.ApprovalBy);
            // Create email message using stringbuilder
            var sb = new StringBuilder();
            sb.AppendLine($"<p>Dear {author.Forename} {author.Surname}, </p>");
            sb.AppendLine("<br>");
            sb.AppendLine($"<p>I have rejected the article - {articleRevision.Title}, please see my comments below for rejection reason(s).</p>");
            sb.AppendLine("<br>");
            sb.AppendLine($"<p>{articleRevision.ApprovalComments}</p>");
            sb.AppendLine("<br>");
            sb.AppendLine($"<p>Kind Regards, <br> {approver.Forename} {approver.Surname}</p>");

            var approvalMessage = sb.ToString();
            var approvalSubject = $"{articleRevision.Title} has been rejected.";
            var mailFromAddress = ConfigurationManager.AppSettings["mailFromAddress"];

            SendMail(author.EmailAddress, mailFromAddress, approvalSubject, approvalMessage);
        }

        public static void SendMail(string mailToAddress, string mailFromAddress, string subject, string message)
        {
            var mailUsername = ConfigurationManager.AppSettings["mailUsername"];
            var mailPassword = ConfigurationManager.AppSettings["mailPassword"];
            var mailSMTPServer = ConfigurationManager.AppSettings["mailSMTPServer"];

            using (SmtpClient client = new SmtpClient(mailSMTPServer, 25))
            {
                // Configure the client
                //client.EnableSsl = true;
                client.Credentials = new NetworkCredential(mailUsername, mailPassword);
                // client.UseDefaultCredentials = true;

                // A client has been created, now you need to create a MailMessage object
                var mailMessage = new MailMessage
                {
                    Subject = subject,
                    SubjectEncoding = Encoding.UTF8,
                    Body = message,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };

                mailMessage.From = new MailAddress(mailFromAddress);
                foreach (var toAddress in mailToAddress.Split(';'))
                {
                    mailMessage.To.Add(new MailAddress(toAddress));
                }

                // Send the message
                client.Send(mailMessage);
            }
        }
    }
}