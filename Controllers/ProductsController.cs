using Microsoft.AspNetCore.Mvc;
using UniMarket.Web.Data;
using UniMarket.Web.Models;

namespace UniMarket.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;

                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }


        // GET: /Products/Details/5
        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: /Products/Delete/5
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: /Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

    }

}
