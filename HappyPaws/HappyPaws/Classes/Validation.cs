using System;
using System.Collections.Generic;
using System.Text;

namespace HappyPaws.Classes
{
    public static class Validation
    {
        public static bool ValidateName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(name))
            {
                return true;
            }
            return false;
        }

        public static bool ValidateDates(DateTime date1, DateTime date2)
        {
            if (date1 <= date2)
            {
                return true;
            }
            return false;
        }

        public static bool ValidateAge(string age)
        {
            if (string.IsNullOrWhiteSpace(age) || string.IsNullOrEmpty(age))
            {
                return false;
            }
            try
            {
                float.Parse(age);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool ValidateEmail(string email)
        {
            if (!string.IsNullOrEmpty(email) && email.Contains("@"))
            {
                if (email.IndexOf('@') != 0 && email.IndexOf('@') == email.LastIndexOf('@'))
                {
                    int atPos = email.IndexOf('@');
                    string domain = email.Substring(atPos);
                    string user = email.Substring(0, email.Length - domain.Length);
                    if (user.Length > 0 && domain.Contains(".") && domain.Length > 3)
                    {
                        if (domain.LastIndexOf('.') != domain.Length - 1 && domain.IndexOf('.') != 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool ValidatePhone(string phone)
        {
            if (!string.IsNullOrEmpty(phone) && phone.Length == 12 && phone[3] == '-' && phone[7] == '-')
            {
                try
                {
                    int.Parse(phone.Substring(0, 3));
                    int.Parse(phone.Substring(4, 3));
                    int.Parse(phone.Substring(8, 4));
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
