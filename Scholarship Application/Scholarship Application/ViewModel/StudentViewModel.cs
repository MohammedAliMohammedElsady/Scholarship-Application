using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scholarship_Application.Models
{
    public class StudentViewModel
    {
        public int studentid { get; set; }
        [Required(ErrorMessage = "First Name Is Required.")]
        public string firstname { get; set; }
        [Required(ErrorMessage = "Last Name Is Required.")]
        public string lastname { get; set; }
        [Required(ErrorMessage = "Email Address Is Required.")]
        [DataType(DataType.EmailAddress)]
        public string emailaddress { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Birth Date Is Required.")]
        public DateTime birthdate { get; set; }
        [Required(ErrorMessage = "National Id Is Required.")]
        public string nationalid { get; set; }
        [Required(ErrorMessage = "University Is Required.")]
        public string university { get; set; }
        [Required(ErrorMessage = "Major Is Required.")]
        public string major { get; set; }
        [Required(ErrorMessage = "GPA Is Required.")]
        public double gpa { get; set; }
        public string resumepath { get; set; }
        public int? userid { get; set; }
        public bool isaccept { get; set; }
        public StudentViewModel GetStudentViewModel(Student s)
        {
            StudentViewModel studentviewmodel = new StudentViewModel();
            studentviewmodel.studentid = s.studentid;
            studentviewmodel.firstname = s.firstname;
            studentviewmodel.lastname = s.lastname;
            studentviewmodel.emailaddress = s.emailaddress;
            studentviewmodel.nationalid = s.nationalid;
            studentviewmodel.university = s.university;
            studentviewmodel.birthdate = s.birthdate;
            studentviewmodel.major = s.major;
            studentviewmodel.gpa = s.gpa;
            studentviewmodel.resumepath = s.resumepath;
            studentviewmodel.userid = s.userid;
            return studentviewmodel;
        }
        public Student GetStudent(StudentViewModel studentviewmodel, int userid, string filename)
        {
            Student student = new Student();
            student.firstname = studentviewmodel.firstname;
            student.lastname = studentviewmodel.lastname;
            student.emailaddress = studentviewmodel.emailaddress;
            student.birthdate = studentviewmodel.birthdate;
            student.nationalid = studentviewmodel.nationalid;
            student.university = studentviewmodel.university;
            student.major = studentviewmodel.major;
            student.gpa = studentviewmodel.gpa;
            student.resumepath = filename;
            student.userid = userid;
            return student;
        }
    }
}