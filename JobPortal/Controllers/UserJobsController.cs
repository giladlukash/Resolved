using JobPortal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace JobPortal.Controllers
{
    public class UserJobsController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();

        // candidate get all jobs here

        public ActionResult Index(string title, string Salary, string Location)
        {
            var jobs = db.Jobs.ToList();
            
            if(!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(Salary) && !string.IsNullOrEmpty(Location))
            {
                jobs = db.Jobs.Where(a => a.Title == title && a.Salary == Salary && a.Location == Location).ToList();
                return View(jobs);
            }
            else if(!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(Salary) || !string.IsNullOrEmpty(Location))
            {
                jobs = db.Jobs.Where(a => a.Title == title || a.Salary == Salary || a.Location == Location).ToList();
                return View(jobs);
            }
            else 
            {
                return View(jobs);
            }
        }

        // open detail job page
        [Authorize]
        public ActionResult Apply(int jobid)
        {
            var job = db.Jobs.Where(c => c.JobId == jobid).SingleOrDefault();
            return View(job);
        }

        //getting cv for recruiter in his portal
        [Authorize]
        public string GetCv(string id)
        {
            var job = db.Users.Where(c => c.Id == id).SingleOrDefault();
            return job.CVPath;
        }
        //Applied for job if already not applied
        [Authorize]
        public ActionResult Applied(int jobid)
        {
            var job = db.Jobs.Where(c => c.JobId == jobid).SingleOrDefault();
            var id = User.Identity.GetUserId();

            var any = db.Applied.Where(c => c.UserId == id && c.JobId == jobid).Any();

            if(any)
            {
                TempData["already"] = "הגשת כבר למשרה זו";
                return RedirectToAction("Apply", new { jobid = jobid });
            }
            var applied = new Applied
            {
                AppliedOn = DateTime.Today,
                JobId = jobid,
                RecruiterId = job.UserId,
                Status = "ממתין",
                UserId = id
            };

            db.Applied.Add(applied);
            
            db.SaveChanges();
            TempData["applied"] = "הוגש";
            return RedirectToAction("Apply", new { jobid = jobid });
        }
        //user applied jobs
        public ActionResult Appliedjobs()
        {
            var id = User.Identity.GetUserId();
            var jobs = db.Applied.Include(c=>c.Jobs).Where(c=>c.UserId == id).ToList();
            return View(jobs);
        }

        //delete a applied job
        public ActionResult deleteapplied(int jobid)
        {
            
            var jobs = db.Applied.Where(c => c.JobId == jobid).SingleOrDefault();
            db.Applied.Remove(jobs);
            db.SaveChanges();
            return RedirectToAction(nameof(Appliedjobs));
        }
    }
}