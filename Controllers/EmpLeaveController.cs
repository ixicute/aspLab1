using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkolprojektLab1.Data;
using SkolprojektLab1.Models;
using SkolprojektLab1.ViewModels;
using SkolprojektLab1.CustomIdentity;
using System.Security.Claims;

namespace SkolprojektLab1.Controllers
{
    [Authorize]
    public class EmpLeaveController : Controller
    {
        private readonly ApplicationDbContext context;

        public EmpLeaveController(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<IActionResult> Index()
        {
            List<MyLeavesViewModel> MyLeavesList = new List<MyLeavesViewModel>();

            //gets current users id as string and convert it to int
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userInt = int.TryParse(currentUserId, out int parsedValue) ? parsedValue : 0;
            var leaves = await (from leaveTable in context.EmpLeaves
                                join emps in context.Employees on leaveTable.FK_Employee equals emps.Id
                                join leaveType in context.Leaves on leaveTable.FK_Leave equals leaveType.Id
                                where leaveTable.FK_Employee.Equals(userInt)
                                select new
                                {
                                    LType = leaveType.LeaveType,
                                    Description = leaveTable.LeaveDescription,
                                    Start = leaveTable.Start,
                                    End = leaveTable.End,
                                    Created = leaveTable.CreatedOn
                                }).ToListAsync();

            foreach(var item in leaves)
            {
                MyLeavesViewModel myLeave = new MyLeavesViewModel();
                myLeave.LeaveType = item.LType;
                myLeave.LeaveDescription = item.Description;
                myLeave.Start = item.Start;
                myLeave.End = item.End;
                myLeave.CreatedOn = item.Created;
                MyLeavesList.Add(myLeave);
            }
            return View(MyLeavesList);
        }

        public async Task<IActionResult> CreateLeave()
        {
            List<Leave> leavesList = new List<Leave>();
            leavesList = await context.Leaves.ToListAsync();

            var leavesType = new CreateLeaveViewModel();
            leavesType.leaveList = new List<Leave>();
            leavesType.leaveList.AddRange(leavesList);

            return View(leavesType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeave(CreateLeaveViewModel createLeave)
        {
            var currentUsername = User.Identity.Name;
            var user = await context.Employees.FirstOrDefaultAsync(e => e.UserName == currentUsername);

            if(!ModelState.IsValid && user == null)
            {
                TempData["Error"] = "An error occured. Please try again!";
                return RedirectToAction("CreateLeave", "EmpLeave");
            }

            
            if(createLeave.LeaveTypeId < 1)
            {
                TempData["LeaveError"] = "You need to pick a leave type!";
                return RedirectToAction("CreateLeave", "EmpLeave");
            }

            if (createLeave.Start == null || createLeave.Start.Value.Date < DateTime.Now.Date)
            {
                TempData["StartDateError"] = "You can't pick a date previous to today's date.";
                return RedirectToAction("CreateLeave", "EmpLeave");
            }

            if(createLeave.End == null)
            {
                TempData["EndDateError"] = "You must pick an end date.";
                return RedirectToAction("CreateLeave", "EmpLeave");
            }

            if (createLeave.LeaveDescription == null)
            {
                createLeave.LeaveDescription = "";
            }

            var newLeave = new EmpLeave()
            {
                FK_Employee = user.Id,
                FK_Leave = createLeave.LeaveTypeId,
                LeaveDescription = createLeave.LeaveDescription,
                Start = (DateTime)createLeave.Start,
                End = (DateTime)createLeave.End,
                CreatedOn = DateTime.UtcNow.Date
            };
            context.EmpLeaves.Add(newLeave);

            await context.SaveChangesAsync();
            TempData["Confirm"] = "Your leave application has been added successfully!";
            return RedirectToAction("Confirmation", "Home");
        }

        public async Task<IActionResult> SearchEmpLeave(string searchString = "", bool withLeaveOnly = false)
        {
            List<EmpLeave> allLeaves = new List<EmpLeave>();
            allLeaves = await context.EmpLeaves.ToListAsync();

            //here we save data about the user/users with id, username, fullname and address.
            var EmployeeData = await (from empsData in context.Employees
                                      join adrs in context.Addresses on empsData.FK_Adress equals adrs.Id
                                      where empsData.UserName.Contains(searchString) || empsData.Email.Contains(searchString)
                                      select new
                                      {
                                          EmpId = empsData.Id,
                                          EmpUsername = empsData.UserName,
                                          EmpFullName = empsData.FirstName + " " + empsData.LastName,
                                          EmpEmail = empsData.Email,
                                          EmpAddress = adrs.City + " " + adrs.Street
                                      }).ToListAsync();

            List<SearchEmpLeaveViewModel> unsortedSearchResult = new List<SearchEmpLeaveViewModel>();

            //here we go through that list of user/users and check if the user id exists in the Rrelationship table of Leaves
            //and add it as a true or false.
            foreach (var item in EmployeeData)
            {
                var searchResult = new SearchEmpLeaveViewModel
                {
                    UserId = item.EmpId,
                    Username = item.EmpUsername,
                    EmployeeName = item.EmpFullName,
                    EmployeeEmail = item.EmpEmail,
                    EmpAddress = item.EmpAddress,
                    HasLeave = allLeaves.Any(l => l.FK_Employee == item.EmpId)
                };
                unsortedSearchResult.Add(searchResult);
            }

            List<SearchEmpLeaveViewModel> sortedSearchResults = new List<SearchEmpLeaveViewModel>();
            if (withLeaveOnly)
            {
                sortedSearchResults = unsortedSearchResult.Where(s => s.HasLeave.HasValue && s.HasLeave.Value).ToList();
            }

            else
            {
                sortedSearchResults = unsortedSearchResult;
            }

            return View(sortedSearchResults);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> SearchLeaveByDate(DateTime fromDate, DateTime toDate)
        {
            List<EmpLeave> allLeaves = new List<EmpLeave>();
            allLeaves = await context.EmpLeaves.ToListAsync();

           
                var EmployeeData = await (from leaveTable in context.EmpLeaves
                                          join emps in context.Employees on leaveTable.FK_Employee equals emps.Id
                                          join leaveType in context.Leaves on leaveTable.FK_Leave equals leaveType.Id
                                          select new
                                          {
                                              EmpId = emps.Id,
                                              EmpName = emps.FirstName + " " + emps.LastName,
                                              Start = leaveTable.Start,
                                              End = leaveTable.End,
                                              Created = leaveTable.CreatedOn
                                          }).ToListAsync();
            

            List<LeaveByDate> sorted = new List<LeaveByDate>();

            foreach (var item in EmployeeData)
            {
                // Check if fromDate and toDate are not equal to their default value
                bool applyFilter = false;
                if(!fromDate.Equals(DateTime.MinValue) || !toDate.Equals(DateTime.MinValue)){
                    applyFilter = true;
                }

                // Apply the filter condition based on the check
                if (!applyFilter || (item.Start.Date >= fromDate.Date && item.Start.Date <= toDate.Date))
                {
                    LeaveByDate leaveData = new LeaveByDate();
                    leaveData.EmployeeId = item.EmpId;
                    leaveData.EmployeeName = item.EmpName;
                    leaveData.Started = item.Start;
                    leaveData.Ends = item.End;
                    leaveData.CreatedOn = item.Created;

                    sorted.Add(leaveData);
                } else if(fromDate.Equals(DateTime.MinValue) || toDate.Equals(DateTime.MinValue))
                {
                    LeaveByDate leaveData = new LeaveByDate();
                    leaveData.EmployeeId = item.EmpId;
                    leaveData.EmployeeName = item.EmpName;
                    leaveData.Started = item.Start;
                    leaveData.Ends = item.End;
                    leaveData.CreatedOn = item.Created;

                    sorted.Add(leaveData);
                }
            }
            return View(sorted);
        }
    }
}