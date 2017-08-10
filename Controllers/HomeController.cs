using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContactFormWithMultiLanguageValidation.Models;

namespace ContactFormWithMultiLanguageValidation.Controllers
{
    public class HomeController : Controller
    {
        private ContactDbEntities db = new ContactDbEntities();
        private string culture = "TR";
        [HttpGet]
        public JsonResult ChangeCulture(string cultureCode)
        {
            culture = cultureCode;
            Session["Culture"] = cultureCode;
            Info info = new Info();
            if (cultureCode == "TR")
            {
                var result = db.FormField.Select(x => new
                {
                    x.SubmitTR,
                    x.ContactTR,
                    x.DescriptionTR,
                }).First();

                info = new Info
                {
                    Contact = result.ContactTR,
                    Submit = result.SubmitTR,
                    Description = result.DescriptionTR
                };
                return new JsonResult { Data = info, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                var result = db.FormField.Select(x => new
                {
                    x.SubmitEN,
                    x.ContactEN,
                    x.DescriptionEN,
                }).First();

                info = new Info
                {
                    Contact = result.ContactEN,
                    Submit = result.SubmitEN,
                    Description = result.DescriptionEN
                };
                return new JsonResult { Data = info, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //public JsonResult Contact(UserMessage usrMsg)
        public JsonResult Contact(string person, string mail, string subject, string message)
        {
            var post = new UserMessage
            {
                FullName = person,
                Email = mail,
                Subject = subject,
                Message = message
            };
            if (ModelState.IsValid)
            {
                //SendMailToOwner(post);
                SendMailToUser(post);
                return Json("success");
            }
            return Json("fail");
        }

        public void SendMailToUser(UserMessage info)
        {
            try
            {
                var message = "";
                message = Session["Culture"].ToString() == "EN" ? "We have recorded your message to our system.<br>Your message will be evaluated as soon as possible.<br><br>Thank you." : "Mesajınızı sistemimize kaydettik.<br>Mesajınız en kısa sürede değerlendirilecek.<br><br>Teşekkür ederiz.";
                MailSender.GmailUsername = "your gmail";
                MailSender.GmailPassword = "your password";
                var mailer = new MailSender
                {
                    ToEmail = info.Email,
                    Subject = "",
                    Body = message,
                    IsHtml = true
                };
                mailer.Send();
            }
            catch { }

        }
    }
}