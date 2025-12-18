using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using UniMarket.Web.Data;
using UniMarket.Web.Models;


namespace UniMarket.Web.Controllers
{
    
    public class ProductsController : Controller
    {   
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private readonly UserManager<IdentityUser> _userManager;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var query = _context.Products.AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(p => p.IsApproved);
            }

            var products = query
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(products);
        }



        // GET: /Products/Edit/7
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product);
        }


        // GET: /Products/Edit/7
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product, IFormFile? imageFile)
        {
            var existingProduct = _context.Products.Find(id);
            if (existingProduct == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                existingProduct.Title = product.Title;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.IsSold = product.IsSold;

                if (imageFile != null)
                {
                    // eski resmi sil
                    if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                    {
                        var oldImagePath = Path.Combine(
                            _env.WebRootPath,
                            existingProduct.ImagePath.TrimStart('/')
                        );

                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    // yeni resmi kaydet
                    var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                    var uploadPath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                    using var stream = new FileStream(uploadPath, FileMode.Create);
                    imageFile.CopyTo(stream);

                    existingProduct.ImagePath = "/uploads/" + fileName;
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

           return View(existingProduct);
        }


        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // 1) Resim geldiyse kaydet
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads"
                    );

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    product.ImagePath = "/uploads/" + fileName;
                }

                // 2) Diğer alanlar
                product.CreatedAt = DateTime.Now;

                product.OwnerId = _userManager.GetUserId(User);
                product.IsApproved = User.IsInRole("Admin");

                // 3) DB’ye ekle
                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("Index");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);

            if (product != null)
            {
                // 🔴 RESİM DOSYASINI SİL
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    var fileName = Path.GetFileName(product.ImagePath);
                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        fileName
                    );

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // 🔴 VERİTABANINDAN ÜRÜNÜ SİL
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Pending()
        {
            var products = _context.Products
            .Where(p => !p.IsApproved)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

            return View(products);
        }  

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            product.IsApproved = true;
            _context.SaveChanges();

            return RedirectToAction(nameof(Pending));
        }

    }

}
