using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BolaoApp.Data;
using BolaoApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace BolaoApp.Controllers
{
    [Authorize]
    public class JogoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jogo
        public async Task<IActionResult> Index()
        {
            return View(await _context.Jogos
                                      .AsNoTracking()
                                      .Where(x=>x.User == User.Identity.Name)
                                      .ToListAsync());
        }

        // GET: Jogo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogo = await _context.Jogos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }

        // GET: Jogo/Create
        public IActionResult Create()
        {
            if(User.Identity.Name == "administrador@gmail.com"){
                return View();
            }else{
                return Redirect("/Jogo");
            }
        }

        // POST: Jogo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,User,Rodada,IdMandante,GolsMandante,IdVisitante,GolsVisitante")] Jogo jogo)
        {     
            if(User.Identity.Name == "administrador@gmail.com"){
                if (ModelState.IsValid)
                {
                    var allUsers = await _context.Users.ToListAsync();

                    foreach(Microsoft.AspNetCore.Identity.IdentityUser user in allUsers){
                        jogo.User = user.UserName;

                        _context.Add(jogo);
                        await _context.SaveChangesAsync();
                        jogo.Id = jogo.Id + 1;
                    }
                    return RedirectToAction(nameof(Index));
                }   
            }else{
                return Redirect("/Time");
            }
            
            return View(jogo);
        }

        // GET: Jogo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo == null)
            {
                return NotFound();
            }
            return View(jogo);
        }

        // POST: Jogo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,User,Rodada,IdMandante,GolsMandante,IdVisitante,GolsVisitante")] Jogo jogo)
        {
            if (id != jogo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    jogo.User = User.Identity.Name;
                    _context.Update(jogo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JogoExists(jogo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jogo);
        }

        // GET: Jogo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if(User.Identity.Name != "administrador@gmail.com"){
                return Redirect("/Jogo");
            }

            var jogo = await _context.Jogos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jogo == null)
            {
                return NotFound();
            }
            return View(jogo);
        }

        // POST: Jogo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            _context.Jogos.Remove(jogo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JogoExists(int id)
        {
            return _context.Jogos.Any(e => e.Id == id);
        }
    }
}
