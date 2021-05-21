using Microsoft.VisualStudio.TestTools.UnitTesting;
using HappyPaws.Classes;

namespace HappyPawsTests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void ValidateEmailTest()
        {
            string[] validEmails = { "student@wgu.edu", "bob@mail.com", "bob@mail.co.uk", "bob.smith@mail.com", "x@x.x" };

            string[] invalidEmails = { "", "@wgu.edu", "student@.wgu", "student@wguedu", "student@....", "student@", "student@wgu.edu." }; 
            
            foreach (string email in validEmails)Assert.IsTrue(Validation.ValidateEmail(email), "Validation failed for valid email address " + email);

            foreach (string email in invalidEmails) Assert.IsFalse(Validation.ValidateEmail(email), "Validation passed for invalid email address " + email);

        }
    }
}
