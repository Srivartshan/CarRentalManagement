using Microsoft.AspNetCore.Mvc;
using CarRentalManagement.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IRepository;

namespace CarRentalManagement.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IApiRepository<Customer> _apiRepository;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(IApiRepository<Customer> apiRepository, ILogger<CustomerController> logger)
        {
            _apiRepository = apiRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Fetching all customers.");
            var customers = await _apiRepository.GetAllAsync();
            return View(customers);
        }

        public IActionResult Add()
        {
            _logger.LogInformation("Rendering Add customer form.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Customer model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Adding a new customer.");
                await _apiRepository.AddAsync(model);
                return RedirectToAction("Index");
            }

            _logger.LogWarning("Invalid model state.");
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Fetching details for customer ID {id}.");
            var customer = await _apiRepository.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Fetching customer ID {id} for editing.");
            var customer = await _apiRepository.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Updating customer ID {model.Id}.");
                await _apiRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }

            _logger.LogWarning("Invalid model state.");
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Deleting customer ID {id}.");
            await _apiRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
