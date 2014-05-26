using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    public interface IEmailSettingsProvider
    {
        string MailFromAddress { get; set; }
        bool UseSsl { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string ServerName { get; set; }
        int ServerPort { get; set; }
        bool WriteAsFile { get; set; }
        string FileLocation { get; set; }
    }
}
