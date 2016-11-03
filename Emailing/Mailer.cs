using System;
using System.Net;
using System.Net.Mail;
using Fabio.SharpTools.String;
using Fabio.SharpTools.Convertion;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Fabio.SharpTools.Emailing
{
    /// <summary>
    /// Provides an access to and management of Mail objects.
    /// This class provides the mechanism to create, manage, store and send e-mail messages.
    /// </summary>
    public sealed class Mailer
    {
        //protected static int ThreadCount = 0;
        //protected static int ThreadMax = 20;

        private Collection<string> toAddresses;
        private Collection<string> ccAddresses;
        private Collection<string> bccAddresses;
        private Collection<string> replyToAddresses;

        private NameValueCollection tokens;

        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }


        public Collection<string> ToAddresses
        {
            get { return toAddresses; }
        }

        public Collection<string> CcAddresses
        {
            get { return ccAddresses; }
        }

        public Collection<string> BccAddresses
        {
            get { return bccAddresses; }
        }

        public Collection<string> ReplyToAddresses
        {
            get { return replyToAddresses; }
        }

        public NameValueCollection Tokens
        {
            get { return tokens; }
        }

        /// <summary>
        /// Provides a Mail API
        /// </summary>
        public Mailer()
        {
            toAddresses = new Collection<string>();
            ccAddresses = new Collection<string>();
            bccAddresses = new Collection<string>();
            replyToAddresses = new Collection<string>();
            tokens = new NameValueCollection();
        }

        /// <summary>
        /// Sends Email
        /// </summary>
        public void Send()
        {
            try
            {
                string[] to = new string[toAddresses.Count];
                string[] cc = new string[ccAddresses.Count];
                string[] bcc = new string[bccAddresses.Count];
                string[] reply = new string[replyToAddresses.Count];

                toAddresses.CopyTo(to,0);
                ccAddresses.CopyTo(cc,0);
                bccAddresses.CopyTo(bcc,0);
                replyToAddresses.CopyTo(reply,0);

                Send(FromAddress, to, cc, bcc, Subject, Body, reply, SmtpServer, SmtpUserName, SmtpPassword, Tokens);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Sends Email
        /// </summary>
        /// <param name="FromAddress">Recipient Email Adress</param>
        /// <param name="ToAddresses">List of Destination Email Adresses, separated by comma or semicolon, cannot be null or empty</param>
        /// <param name="CcAddresses">List of Destination Email Adresses to send a copy to, separated by comma or semicolon, cannot be null or empty</param>
        /// <param name="BccAddresses">List of Destination Email Adresses to send a hidden copy to, separated by comma or semicolon, cannot be null or empty</param>
        /// <param name="Subject">Mail subject can be null or empty</param>
        /// <param name="Body">Mail body can be null or empty</param>
        /// <param name="ReplyToAddresses">List of ReplyTo Email Adresses, separated by comma or semicolon, can be null or empty</param>
        /// <param name="SmtpServer">SmtpServer Adress, cannot be null or empty</param>
        /// <param name="SmtpUserName">Smtp UserName, cannot be null or empty</param>
        /// <param name="SmtpPassword">Smtp Password, cannot be null or empty</param>
        public static void Send(string FromAddress, string ToAddresses, string CcAddresses, string BccAddresses,
            string Subject, string Body, string ReplyToAddresses,
            string SmtpServer, string SmtpUserName, string SmtpPassword)
        {
            Send(FromAddress, ToAddresses, CcAddresses, BccAddresses, Subject, Body, ReplyToAddresses, SmtpServer, SmtpUserName, SmtpPassword, null);
        }

        /// <summary>
        /// Sends Email
        /// </summary>
        /// <param name="FromAddress">Recipient Email Adress</param>
        /// <param name="ToAddresses">List of Destination Email Adresses, separated by comma or semicolon, cannot be null or empty</param>
        /// <param name="CcAddresses">List of Destination Email Adresses to send a copy to, separated by comma or semicolon, cannot be null or empty</param>
        /// <param name="BccAddresses">List of Destination Email Adresses to send a hidden copy to, separated by comma or semicolon, cannot be null or empty</param>
        /// <param name="Subject">Mail subject can be null or empty</param>
        /// <param name="Body">Mail body can be null or empty</param>
        /// <param name="ReplyToAddresses">List of ReplyTo Email Adresses, separated by comma or semicolon, can be null or empty</param>
        /// <param name="SmtpServer">SmtpServer Adress, cannot be null or empty</param>
        /// <param name="SmtpUserName">Smtp UserName, cannot be null or empty</param>
        /// <param name="SmtpPassword">Smtp Password, cannot be null or empty</param>
        /// <param name="Tokens">Tokens to be replaced</param>
        public static void Send(string FromAddress, string ToAddresses, string CcAddresses, string BccAddresses,
            string Subject, string Body, string ReplyToAddresses,
            string SmtpServer, string SmtpUserName, string SmtpPassword, NameValueCollection Tokens)
        {
            string[] toList = null;
            string[] ccList = null;
            string[] bccList = null;
            string[] replyToList = null;

            if (!string.IsNullOrEmpty(ToAddresses))
                toList = ToAddresses.Split(new Char[] { ',', ';' });

            if (!string.IsNullOrEmpty(CcAddresses))
                ccList = CcAddresses.Split(new Char[] { ',', ';' });

            if (!string.IsNullOrEmpty(BccAddresses))
                bccList = BccAddresses.Split(new Char[] { ',', ';' });

            if (!string.IsNullOrEmpty(ReplyToAddresses))
                replyToList = ReplyToAddresses.Split(new Char[] { ',', ';' });

            try
            {
                Send(FromAddress, toList, ccList, bccList, Subject, Body, replyToList, SmtpServer, SmtpUserName, SmtpPassword, Tokens);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Sends Email
        /// </summary>
        /// <param name="FromAddress">Recipient Email Adress</param>
        /// <param name="ToAddresses">List of Destination Email Adresses cannot be null or empty</param>
        /// <param name="CcAddresses">List of Destination Email Adresses to send a copy to cannot be null or empty</param>
        /// <param name="BccAddresses">List of Destination Email Adresses to send a hidden copy to cannot be null or empty</param>
        /// <param name="Subject">Mail subject can be null or empty</param>
        /// <param name="Body">Mail body can be null or empty</param>
        /// <param name="ReplyToAddresses">List of ReplyTo Email Adresses, can be null or empty</param>
        /// <param name="SmtpServer">SmtpServer Adress, cannot be null or empty</param>
        /// <param name="SmtpUserName">Smtp UserName, cannot be null or empty</param>
        /// <param name="SmtpPassword">Smtp Password, cannot be null or empty</param>
        /// <param name="Tokens">Tokens to be replaced</param>
        public static void Send(string FromAddress, string[] ToAddresses, string[] CcAddresses, string[] BccAddresses,
            string Subject, string Body, string[] ReplyToAddresses,
            string SmtpServer, string SmtpUserName, string SmtpPassword, NameValueCollection Tokens)
        {
            if (ToAddresses == null)
                throw new ArgumentNullException("ToAddresses cannot be null");
            if (FromAddress == null)
                throw new ArgumentNullException("FromAddress cannot be null");

            FromAddress = FromAddress.Trim().ToLower(CultureInfo.CurrentCulture);

            if (!StringRegexValidator.IsEmail(FromAddress))
                throw new ArgumentException("FromAddress is not a valid email address: " + FromAddress);


            //for (int i = 0; i < ToAddresses.Length; i++)
            //{
            //    ToAddresses[i] = ToAddresses[i].Trim();
            //    if (!StringRegexValidator.IsEmail(ToAddresses[i]))
            //        throw new ArgumentException("ToAddresses contains not valid email address: " + ToAddresses[i]);
            //}

            if (ToAddresses.Length == 0)
                throw new ArgumentException("ToAddresses is empty");

            //if (CcAddresses != null)
            //{
            //    for (int i = 0; i < CcAddresses.Length; i++)
            //    {
            //        CcAddresses[i] = CcAddresses[i].Trim();
            //        if (!StringRegexValidator.IsEmail(CcAddresses[i]))
            //            throw new ArgumentException("CcAddresses contains not valid email address: " + CcAddresses[i]);
            //    }
            //}

            //if (BccAddresses != null)
            //{
            //    for (int i = 0; i < BccAddresses.Length; i++)
            //    {
            //        BccAddresses[i] = BccAddresses[i].Trim();
            //        if (!StringRegexValidator.IsEmail(BccAddresses[i]))
            //            throw new ArgumentException("BccAddresses contains not valid email address: " + BccAddresses[i]);
            //    }
            //}

            //if (ReplyToAddresses != null)
            //{
            //    for (int i = 0; i < ReplyToAddresses.Length; i++)
            //    {
            //        ReplyToAddresses[i] = ReplyToAddresses[i].Trim();
            //        if (!StringRegexValidator.IsEmail(ReplyToAddresses[i]))
            //            throw new ArgumentException("ReplyToAddresses contains not valid email address: " + ReplyToAddresses[i]);
            //    }

            //}
            
            if (Tokens != null && Tokens.Count > 0)
                replaceTokens(ref Body, ref Subject, Tokens);

            MailMessage mail = null;

            foreach (string to in ToAddresses)
            {
                if (mail == null)
                    mail = new MailMessage(FromAddress, to, Subject, Body);
                else
                    mail.To.Add(to);
            }

            if (CcAddresses != null)
            {
                foreach (string cc in CcAddresses)
                    mail.CC.Add(cc);
            }

            if (BccAddresses != null)
            {
                foreach (string bcc in BccAddresses)
                    mail.Bcc.Add(bcc);
            }

            if (ReplyToAddresses != null)
            {
                foreach (string replyTo in ReplyToAddresses)
                    mail.ReplyToList.Add(new MailAddress(replyTo));
            }

            mail.IsBodyHtml = true;

            mail.From = new MailAddress(FromAddress);

            SmtpClient scilent = new SmtpClient(SmtpServer);
            scilent.Credentials = new NetworkCredential(SmtpUserName, SmtpPassword);

            try
            {
                scilent.Send(mail);
            }
            catch
            {
                throw;
            }

        }

        public void replaceTokens(NameValueCollection tokens)
        {
            foreach (string token in tokens.Keys)
            {
                Body = Body.Replace("[" + token + "]", tokens[token]);
                Subject = Subject.Replace("[" + token + "]", tokens[token]);
            }

        }

        public static void replaceTokens(ref string body, ref string subject, NameValueCollection tokens)
        {
            foreach (string token in tokens.Keys)
            {
                body = body.Replace("[" + token + "]", tokens[token]);
                subject = subject.Replace("[" + token + "]", tokens[token]);
            }

        }

    }
}