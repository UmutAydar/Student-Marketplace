using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using UniMarket.Web.Data;
using UniMarket.Web.Models;

namespace UniMarket.Web.Controllers
{
    public class ClubsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ClubsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Clubs
        public async Task<IActionResult> Index()
        {
            var clubs = await _context.Clubs.ToListAsync();
            return View(clubs);
        }

        // GET: Clubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var club = await _context.Clubs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (club == null)
                return NotFound();

            return View(club);
        }

        // GET: Clubs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clubs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Club club, IFormFile? logo)
        {
            if (ModelState.IsValid)
            {
                if (logo != null && logo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/clubs");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await logo.CopyToAsync(stream);

                    club.LogoPath = "/uploads/clubs/" + fileName;
                }

                _context.Add(club);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(club);
        }

        // GET: Clubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var club = await _context.Clubs.FindAsync(id);
            if (club == null)
                return NotFound();

            return View(club);
        }

        // POST: Clubs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Club club, IFormFile? logo)
        {
            if (id != club.Id)
                return NotFound();

            var existingClub = await _context.Clubs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (existingClub == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Yeni logo geldiyse: eskisini sil, yenisini kaydet
                if (logo != null && logo.Length > 0)
                {
                    // eski dosyayı sil
                    if (!string.IsNullOrEmpty(existingClub.LogoPath))
                    {
                        var oldPath = existingClub.LogoPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                        var oldFileFullPath = Path.Combine(_env.WebRootPath, oldPath);

                        if (System.IO.File.Exists(oldFileFullPath))
                            System.IO.File.Delete(oldFileFullPath);
                    }

                    // yeni dosyayı kaydet
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/clubs");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(logo.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await logo.CopyToAsync(stream);

                    club.LogoPath = "/uploads/clubs/" + fileName;
                }
                else
                {
                    // yeni logo yoksa eskisi kalsın
                    club.LogoPath = existingClub.LogoPath;
                }

                _context.Update(club);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(club);
        }

        // GET: Clubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var club = await _context.Clubs.FirstOrDefaultAsync(c => c.Id == id);
            if (club == null)
                return NotFound();

            return View(club);
        }


        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club == null)
                return RedirectToAction(nameof(Index));

            if (!string.IsNullOrEmpty(club.LogoPath))
            {
                var relativePath = club.LogoPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
