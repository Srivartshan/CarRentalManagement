using Microsoft.AspNetCore.Mvc;
using CarRentalManagement.Models;
using System.Threading.Tasks;
using CarRentalManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System;

namespace CarRentalManagement.Controllers
{
    public class BookingController : Controller
    {
        private readonly CarRentalContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(CarRentalContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
            _logger.LogInformation("BookingController initialized.");
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Index action called: Fetching booking list.");

            var bookings = await _context.Bookings.Include(b => b.Car).Include(b => b.Customer).ToListAsync();

            if (bookings == null || !bookings.Any())
            {
                _logger.LogWarning("Index action: No bookings found.");
                ViewBag.Message = "No bookings available.";
            }

            _logger.LogInformation("Index action: Booking list fetched successfully.");
            return View(bookings);
        }

        [HttpGet]
        public IActionResult Add()
        {
            _logger.LogInformation("Add (GET) action called: Rendering booking addition form.");

            var cars = _context.Cars.ToList();
            if (!cars.Any())
            {
                _logger.LogWarning("Add (GET) action: No cars available.");
                ViewBag.NoCarsMessage = "No cars available.";
            }
            ViewBag.Cars = cars;

            var customers = _context.Customers.ToList();
            if (!customers.Any())
            {
                _logger.LogWarning("Add (GET) action: No customers available.");
                ViewBag.NoCustomersMessage = "No customers available.";
            }
            ViewBag.Customers = customers;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Booking model)
        {
            _logger.LogInformation("Add (POST) action called: Adding a new booking.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Add (POST) action: Validation failed.");

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation Error: {ErrorMessage}", error.ErrorMessage);
                }

                // Fetch cars and customers list for the view again if validation fails
                ViewBag.Cars = await _context.Cars.ToListAsync();
                ViewBag.Customers = await _context.Customers.ToListAsync();
                return View(model);
            }

            _logger.LogInformation("Add (POST) action: Booking details - CarId: {CarId}, CustomerId: {CustomerId}, BookingDate: {BookingDate}, ReturnDate: {ReturnDate}",
                model.CarId, model.CustomerId, model.BookingDate, model.ReturnDate);

            try
            {
                var successParam = new SqlParameter("@Success", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                var errorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

                // Execute stored procedure to insert booking
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC AddBooking @CarId, @CustomerId, @BookingDate, @ReturnDate, @Success OUTPUT, @ErrorMessage OUTPUT",
                    new SqlParameter("@CarId", model.CarId),
                    new SqlParameter("@CustomerId", model.CustomerId),
                    new SqlParameter("@BookingDate", SqlDbType.DateTime) { Value = model.BookingDate },
                    new SqlParameter("@ReturnDate", SqlDbType.DateTime) { Value = model.ReturnDate },
                    successParam,
                    errorMessageParam
                );

                _logger.LogInformation("Stored procedure executed: Success = {Success}, ErrorMessage = {ErrorMessage}", successParam.Value, errorMessageParam.Value);

                if (!(bool)successParam.Value)
                {
                    _logger.LogWarning("Add (POST) action: Booking failed - {ErrorMessage}", errorMessageParam.Value);
                    ModelState.AddModelError("", $"Booking failed: {errorMessageParam.Value}");
                    ViewBag.Cars = await _context.Cars.ToListAsync();
                    ViewBag.Customers = await _context.Customers.ToListAsync();
                    return View(model);
                }

                _logger.LogInformation("Add (POST) action: Booking added successfully.");
                return RedirectToAction("Index");
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception occurred while adding booking.");
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again.");
                ViewBag.Cars = await _context.Cars.ToListAsync();
                ViewBag.Customers = await _context.Customers.ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception occurred while adding booking.");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                ViewBag.Cars = await _context.Cars.ToListAsync();
                ViewBag.Customers = await _context.Customers.ToListAsync();
                return View(model);
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Details action called: Fetching details for BookingId {BookingId}.", id);

            var booking = await _context.Bookings.Include(b => b.Car).Include(b => b.Customer).FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                _logger.LogWarning("Details action: Booking with Id {BookingId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Details action: Booking details fetched successfully for BookingId {BookingId}.", id);
            return View(booking);
        }
    }
}
