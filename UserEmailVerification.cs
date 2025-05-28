using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace KeepTrackApp.Utils
{
    public static class UserHelper
    {
        private static int _userCounter = 0;

        public static string GenerateUniqueUsername()
        {
            _userCounter++;
            return $"ki{_userCounter.ToString("D7")}";
        }

        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool ValidatePassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }

        public static string GenerateVerificationCode()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }

        public static void SendVerificationEmail(string email, string code, string userId)
        {
            Console.WriteLine($"[Simulated Email] Sending verification code to {email}...");
            Console.WriteLine($"[Simulated Email] Your code is: {code}");

            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("aq542665@gmail.com", "aackrzirnkvpkqlz"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("aq542665@gmail.com"),
                    Subject = "Your KeepTrack Verification Code",
                    Body = $"Your KeepTrack User ID is: {userId}\nYour verification code is: {code}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);
                Console.WriteLine(" Verification email sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to send email: {ex.Message}");
            }
            
        }
    }
}
