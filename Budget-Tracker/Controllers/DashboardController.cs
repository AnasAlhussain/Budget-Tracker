using Budget_Tracker.Data;
using Budget_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Runtime.ExceptionServices;
//using Transaction = Budget_Tracker.Models.Transaction;

namespace Budget_Tracker.Controllers
{
    public class DashboardController : Controller
    {

        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            this._context = context;
        }



        public async Task<ActionResult> Index()
        {

            //Last 7 Days Transactions
            DateTime StratDate = DateTime.Now.AddDays(-6);
            DateTime EndDate = DateTime.Today;


            List<Transaction> SelectedTransactions =await _context.Transactions
                .Include(x => x.Category).
                Where(t => t.Date >= StratDate && t.Date <= EndDate).ToListAsync();



            //Total Income
            int TotlaIncom = SelectedTransactions.
                Where(i => i.Category.Type == "Income")
                .Sum(i => i.Amount);
            ViewBag.TotalIncome = TotlaIncom.ToString("C0");


            //Total Expense
            int TotalExpense = SelectedTransactions.
                Where(i => i.Category.Type == "Expense")
                .Sum(i => i.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");


            //balance
            int balance = TotlaIncom - TotalExpense;
            CultureInfo culture =  CultureInfo.CreateSpecificCulture("en-US");

            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C0}", balance);



            //Doughnut Chart - Expense by Category 

            ViewBag.DoughnutChartData = SelectedTransactions.Where(i => i.Category.Type == "Expense")
                .GroupBy(i => i.Category.CategoryId)
                .Select(k => new
                {
                    CategoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(i => i.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                }).OrderByDescending(l => l.amount).ToList();



            //SpLine Chart - Income vs Expense

            //Income
            List<SplineChartData> IncomeSummary = SelectedTransactions.
                Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date).
                Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                }).ToList();


            //Expense
            List<SplineChartData> ExpenseSummary = SelectedTransactions.
                Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date).Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                }).ToList();



            //Combain Income and Expense
            string[] Last7Days = Enumerable.Range(0, 7).Select(i => StratDate.AddDays(i).ToString("dd-MMM")).ToArray();

            ViewBag.SpLineChartData = from day in Last7Days
                                      join incom in IncomeSummary on day equals incom.day into dayIncomeJoined
                                      from incom in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpenseSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = incom == null ? 0 : incom.income,
                                          expense = expense == null ? 0 : expense.expense

                                      };

            //Recent Tranasctions 
            ViewBag.RecentTransactions = await _context.Transactions.
               Include(i => i.Category)
               .OrderByDescending(j => j.Date).Take(5).ToListAsync();


            return View();
        }
    }

    public class SplineChartData 
    {
        public string day;
        public int income;
        public int expense;
    }
}
