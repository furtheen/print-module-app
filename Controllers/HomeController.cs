using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrintModuleApp.Models;
using UglyToad.PdfPig;

namespace PrintModuleApp.Controllers;

public class HomeController : Controller
{
    // 🔥 ADD DB CONTEXT
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    // 🔹 Home Page
    public IActionResult Index()
    {
        return View();
    }

    // 🔹 Privacy Page
    public IActionResult Privacy()
    {
        return View();
    }

    // 🔥 FILE UPLOAD + SAVE + CALCULATE
    [HttpPost]
    public IActionResult Upload(IFormFile file, string printType, int copies)
    {
        if (file != null && file.ContentType == "application/pdf")
        {
            using var stream = file.OpenReadStream();
            using var pdf = PdfDocument.Open(stream);

            int pages = pdf.NumberOfPages;

            int pricePerPage = printType == "bw" ? 2 : 5;
            int totalPrice = pages * copies * pricePerPage;

            // 🔥 SAVE TO DATABASE
            var order = new Order
            {
                FileName = file.FileName,
                Pages = pages,
                PrintType = printType,
                Copies = copies,
                TotalPrice = totalPrice
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // 🔥 REDIRECT TO ORDERS PAGE
            return RedirectToAction("Orders");
        }

        return Content("Invalid file. Please upload a PDF.");
    }

    // 🔥 ORDERS PAGE
    public IActionResult Orders()
    {
        var orders = _context.Orders.ToList();
        return View(orders);
    }

    // 🔹 Error handler
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}