using ClosedXML.Excel;
using ExcelDataReader;
using Postal;
using Scholarship_Application.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace Scholarship_Application.Controllers
{
    public class UserController : Controller
    {
        DemoEntities db = new DemoEntities();
        // GET: User
        public ActionResult Index()
        {
            ViewBag.Message = Session["Message"];
            UserViewModel user = new UserViewModel();
            if (Session["UserName"] != null)
                user.UserName = Session["UserName"].ToString();
            if (Session["Password"] != null)
                user.Password = Session["Password"].ToString();
            return View(user);
        }
        [HttpPost]
        public ActionResult Login(UserViewModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (user.CheckUserNameAndPassword())
                    {
                        // Sucess Login
                        Session["UserName"] = user.UserName;
                        Session["Password"] = user.Password;
                        Session["Message"] = null;
                        return RedirectToAction("StudentApplication", "User");
                    }
                    else
                    {
                        Session["Message"] = "User Name Or Password Is Not Correct";
                        Session["UserName"] = user.UserName;
                        Session["Password"] = user.Password;
                        return RedirectToAction("Index", "User");
                    }
                }
                return RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult Register()
        {
            Session["Message"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserViewModel userviewmodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (userviewmodel.CheckDuplicateUserName())
                    {
                        Session["UserName"] = userviewmodel.UserName;
                        Session["Password"] = userviewmodel.Password;
                        User user = new User();
                        user.user_name = userviewmodel.UserName;
                        user.password = userviewmodel.Password;
                        user.isadmin = userviewmodel.IsAdmin;
                        db.Users.Add(user);
                        db.SaveChanges();
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        Session["Message"] = "User Name Is Duplicate";
                        return View(userviewmodel);
                    }
                }
                return View(userviewmodel);
            }
            catch (Exception ex)
            {
                return View(userviewmodel);
            }
        }
        public ActionResult StudentApplication()
        {
            try
            {
                if (Session["UserName"] != null)
                {
                    string username = Session["UserName"].ToString();
                    User user = db.Users.Where(t => t.user_name == username).FirstOrDefault();
                    Session["UserID"] = user.user_id;
                    if (user.isadmin.HasValue && user.isadmin==true)
                    {
                        // Admin 
                        return RedirectToAction("AdminApplication", "User");
                    }
                    else
                    {
                        // Student
                        StudentViewModel s = new StudentViewModel();
                        Student student = db.Students.Where(t => t.userid == user.user_id).FirstOrDefault();
                        if (student != null)
                        {
                            s = s.GetStudentViewModel(student);
                        }
                        return View(s);
                    }
                }
                return RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "User");
            }
        }
        public ActionResult SubmitApplication(StudentViewModel studentviewmodel, HttpPostedFileBase File)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string path = Server.MapPath("~/App_Data/File");
                    string fileName = Path.GetFileName(File.FileName);
                    string fullpath = Path.Combine(path, fileName);
                    File.SaveAs(fullpath);
                    Student student = new Student();
                    student = db.Students.Where(t => t.studentid == student.studentid).FirstOrDefault();
                    if (student != null)
                    {
                        db.Students.Remove(student);
                        db.SaveChanges();
                    }
                    student = new Student();
                    student = studentviewmodel.GetStudent(studentviewmodel, Convert.ToInt32(Session["UserID"].ToString()), File.FileName);
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("StudentApplication", "User");
                }
                return RedirectToAction("StudentApplication", "User");
            }
            catch (Exception ex)
            {
                return RedirectToAction("StudentApplication", "User");
            }

        }
        public ActionResult AdminApplication()
        {
            try
            {
                List<StudentViewModel> ListStudent = new List<StudentViewModel>();
                StudentViewModel studentviewmodel = new StudentViewModel();
                foreach (var student in db.Students.ToList())
                {
                    studentviewmodel = new StudentViewModel();
                    studentviewmodel = studentviewmodel.GetStudentViewModel(student);
                    ListStudent.Add(studentviewmodel);
                }
                return View(ListStudent);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult AcceptApplication(int id)
        {
            try
            {
                Student student = db.Students.Where(t => t.studentid == id).FirstOrDefault();
                if (student != null)
                {
                    student.isaccept = true;
                    db.SaveChanges();
                    string Email = student.emailaddress;
                    string Body = "Hello " + student.firstname + " " + student.lastname + "<br/> Congratulations, you have been accepted into the scholarship";
                    SendEmail(Email, Body);
                }
                return RedirectToAction("AdminApplication", "User");
            }
            catch (Exception ex)
            {
                return RedirectToAction("AdminApplication", "User");
            }
        }
        public ActionResult RejectApplication(int id)
        {
            try
            {
                Student student = db.Students.Where(t => t.studentid == id).FirstOrDefault();
                if (student != null)
                {
                    student.isaccept = false;
                    db.SaveChanges();
                    string Email = student.emailaddress;
                    string Body = "Hello " + student.firstname + " " + student.lastname + "<br/> Your application for the scholarship has been rejected";
                    SendEmail(Email, Body);
                }
                return RedirectToAction("AdminApplication", "User");
            }
            catch (Exception ex)
            {
                return RedirectToAction("AdminApplication", "User");
            }
        }
        public FileResult DownloadExcel()
        {
            DataTable dt = new DataTable("StudentsGrid");
            dt.Columns.Add(new DataColumn("Student ID"));
            dt.Columns.Add(new DataColumn("First Name"));
            dt.Columns.Add(new DataColumn("Last Name"));
            dt.Columns.Add(new DataColumn("Email Address"));
            dt.Columns.Add(new DataColumn("Birth Date"));
            dt.Columns.Add(new DataColumn("National ID"));
            dt.Columns.Add(new DataColumn("University"));
            dt.Columns.Add(new DataColumn("Major"));
            dt.Columns.Add(new DataColumn("GPA"));
            dt.Columns.Add(new DataColumn("Resume"));
            dt.Columns.Add(new DataColumn("Status"));
            foreach (var student in db.Students.ToList())
            {
                dt.Rows.Add(student.studentid, student.firstname, student.lastname, student.emailaddress,
                            student.birthdate, student.nationalid, student.university, student.major,
                            student.gpa, student.resumepath, student.isaccept == true ? "Accept" : "Reject");
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentApplicationExcel.xlsx");
                }
            }
        }
        public bool SendEmail(string toEmail,string body)
        {
            try
            {
                // Set Email and Password Will Send notification email in Web.config File . 
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["senderPassword"].ToString();
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                client.Credentials = new System.Net.NetworkCredential(senderEmail,senderPassword);
                MailMessage mailMessage = new MailMessage(senderEmail, toEmail, "Status Scholarship Application", body);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                client.Send(mailMessage);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public FileResult Download(string fileName)
        {
            string fullPath = Path.Combine(Server.MapPath("~/App_Data/File"), fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}