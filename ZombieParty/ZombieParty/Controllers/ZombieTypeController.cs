using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZombieParty.Models;
using ZombieParty.Models.Data;
using ZombieParty.ViewModels;

namespace ZombieParty.Controllers
{
    public class ZombieTypeController : Controller
    {
        private ZombiePartyDbContext _baseDonnees { get; set; }

        public ZombieTypeController(ZombiePartyDbContext baseDonnees)
        {
            _baseDonnees = baseDonnees;
        }

        public async Task<IActionResult> Index()
        {
            this.ViewBag.MaListe = await this._baseDonnees.ZombieTypes.ToListAsync();

            return View();
        }


        public async Task <IActionResult> Details(int id)
        {
            var zombies = _baseDonnees.Zombies.Where(z => z.ZombieTypeId == id); // pas de requête envoyée, pas d'await

            ZombieTypeVM zombieTypeVM = new()
            {
                ZombieType = new(),
                ZombiesList = await zombies.ToListAsync(),      // ← ici la requête part vers la BD
                ZombiesCount = await zombies.CountAsync(),       // ← une autre requête part ici
                PointsAverage = await zombies.AverageAsync(p => p.Point)  // ← et une autre ici
            };

            zombieTypeVM.ZombieType = _baseDonnees.ZombieTypes.FirstOrDefault(zt => zt.Id == id);
            return View(zombieTypeVM);
        }

        
        //GET CREATE
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZombieType zombieType)
        {
            if (ModelState.IsValid)
            {
                await _baseDonnees.ZombieTypes.AddAsync(zombieType);
                await _baseDonnees.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(zombieType);
        }
        public async Task<IActionResult> Edit(int id)
        {
            ZombieType zombieType = await _baseDonnees.ZombieTypes.FindAsync(id);

            return View(zombieType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Edit(ZombieType zombieType)
        {
            if (ModelState.IsValid)
            {
                _baseDonnees.ZombieTypes.Update(zombieType);
               await _baseDonnees.SaveChangesAsync();
                TempData["Success"] = $"ZombieType {zombieType.TypeName} has been modified";
                return this.RedirectToAction("Index");
            }

            return View(zombieType);
        }

        public async Task<IActionResult> Delete(int id)
        {
            ZombieType? zombieType = await _baseDonnees.ZombieTypes.FindAsync(id);
            if (zombieType == null)
            {
                return NotFound();
            }

            return View(zombieType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> DeletePost(int id)
        {
            ZombieType? zombieType =await _baseDonnees.ZombieTypes.FindAsync(id);
            if (zombieType == null)
            {
                return NotFound();
            }

            _baseDonnees.ZombieTypes.Remove(zombieType);
            await _baseDonnees.SaveChangesAsync();
            TempData["Success"] = $"ZombieType {zombieType.TypeName} has been removed";
            return RedirectToAction("Index");
        }
    }
}
