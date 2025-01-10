using Microsoft.AspNetCore.Mvc;
using CarRentalManagement.Models;
using System.Threading.Tasks;
using CarRentalManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IRepository;

namespace CarRentalManagement.Controllers
{
    public class CarController : Controller
    {
        private readonly ILogger<CarController> _logger;
        private readonly IGenericRepository<Car> _genericRepository;
        public CarController(ILogger<CarController> logger,IGenericRepository<Car> genericRepository)
        {
            _logger = logger;
            _genericRepository = genericRepository;
            _logger.LogInformation("CarController initialized.");
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching car list.");
            try
            {
                var cars = await _genericRepository.GetAll();
                if (cars == null || !cars.Any())
                {
                    _logger.LogWarning("No cars available in the database.");
                    ViewBag.NotificationMessage = "No cars available.";
                }

                _logger.LogInformation("Car list fetched successfully.");
                return View(cars);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while fetching the car list.");
                return StatusCode(500, "An error occurred while fetching data.");
            }
        }

        public IActionResult Add()
        {
            _logger.LogInformation("Rendering car addition form.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Car newCar)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for adding a car.");
                return View(newCar);
            }

            try
            {
                _logger.LogInformation("Adding a new car: {CarMake} - {CarModel}", newCar.Make, newCar.Model);
                await _genericRepository.Register(newCar);
                _logger.LogInformation("Car added successfully.");
                TempData["SuccessMessage"] = "Car added successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while adding a new car.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the car. Please try again.");
                return View(newCar);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid CarId: {CarId}", id);
                return BadRequest("Invalid car ID.");
            }

            try
            {
                _logger.LogInformation("Fetching details for CarId {CarId}.", id);
                var car = await _genericRepository.GetByData(id);

                if (car == null)
                {
                    _logger.LogWarning("Car with Id {CarId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Car details fetched successfully for CarId {CarId}.", id);
                return View(car);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while fetching details for CarId {CarId}.", id);
                return StatusCode(500, "An error occurred while fetching data.");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid CarId: {CarId}", id);
                return BadRequest("Invalid car ID.");
            }

            try
            {
                _logger.LogInformation("Fetching car details for editing with CarId {CarId}.", id);
                var car = await _genericRepository.GetByData(id);

                if (car == null)
                {
                    _logger.LogWarning("Car with Id {CarId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Car details fetched successfully for editing.");
                return View(car);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while fetching car details for editing.");
                return StatusCode(500, "An error occurred while fetching data.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Car updatedCar)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for editing car.");
                return View(updatedCar);
            }

            try
            {
                _logger.LogInformation("Updating car: {CarId}", updatedCar.Id);
                await _genericRepository.Update(updatedCar);
                _logger.LogInformation("Car updated successfully.");
                TempData["SuccessMessage"] = "Car updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while updating car.");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the car. Please try again.");
                return View(updatedCar);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid CarId: {CarId}", id);
                return BadRequest("Invalid car ID.");
            }

            try
            {
                _logger.LogInformation("Fetching car for deletion with CarId {CarId}.", id);
                var car = await _genericRepository.GetByData(id);

                if (car == null)
                {
                    _logger.LogWarning("Car with Id {CarId} not found.", id);
                    return NotFound();
                }

               await _genericRepository.Delete(car);
                _logger.LogInformation("Car deleted successfully with CarId {CarId}.", id);
                TempData["SuccessMessage"] = "Car deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while deleting car.");
                return StatusCode(500, "An error occurred while deleting the car.");
            }
        }
    }
}
