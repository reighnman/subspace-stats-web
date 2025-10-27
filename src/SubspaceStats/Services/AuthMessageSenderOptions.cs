namespace SubspaceStats.Services
{

    public class AuthMessageSenderOptions
    {
        public string? SendGridKey { get; set; }
        public bool SendGridOverSmtp { get; set; }
        public string? SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpSSL { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public string? SmtpFromEmail { get; set; }
        public string? SmtpFromName { get; set; }
        public bool SmtpUseDefaultCredentials { get; set; }
    }
}