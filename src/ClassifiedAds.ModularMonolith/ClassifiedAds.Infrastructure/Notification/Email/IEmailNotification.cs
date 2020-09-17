﻿namespace ClassifiedAds.Infrastructure.Notification.Email
{
    public interface IEmailNotification
    {
        void Send(EmailMessageDTO emailMessage);
    }

    public class EmailMessageDTO
    {
        public string From { get; set; }

        public string Tos { get; set; }

        public string CCs { get; set; }

        public string BCCs { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
