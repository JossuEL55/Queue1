namespace Queue1.AppQueue.Models
{
    public class MailMessages
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        // Propiedades opcionales adicionales
        public string Cc { get; set; }
        public string Bcc { get; set; }

        public MailMessages(
            string to,
            string subject,
            string body,
            string cc = null,
            string bcc = null)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email (To) cannot be empty.", nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be empty.", nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body cannot be empty.", nameof(body));

            To = to;
            Subject = subject;
            Body = body;
            Cc = cc;
            Bcc = bcc;
        }

        public override string ToString()
        {
            return $"To: {To}\n" +
                   $"Subject: {Subject}\n" +
                   $"Body: {Body}\n" +
                   $"{(string.IsNullOrWhiteSpace(Cc) ? "" : $"Cc: {Cc}\n")}" +
                   $"{(string.IsNullOrWhiteSpace(Bcc) ? "" : $"Bcc: {Bcc}\n")}";
        }
    }
}