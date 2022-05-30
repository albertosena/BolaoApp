using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BolaoApp.Data;
using BolaoApp.Models;

namespace BolaoApp.Controllers
{
    public class RankingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RankingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ranking
        public async Task<IActionResult> Index()
        { 
            return View(await _context.Rankings.OrderByDescending(x => x.Pontos)
            .Where(x => x.User != "administrador@gmail.com")
            .ToListAsync());
        }

        // GET: Ranking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ranking = await _context.Rankings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ranking == null)
            {
                return NotFound();
            }

            return View(ranking);
        }

        // GET: Ranking/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Atualizar()
        {

            if(User.Identity.Name == "administrador@gmail.com"){
                if (ModelState.IsValid)
                {
                    Ranking ranking = new Ranking();
                    ranking.Id = 0;

                    var allUsers = await _context
                    .Users
                    .ToListAsync();

                    var all = _context.Rankings;
                    _context.Rankings.RemoveRange(all);
                    
                    var jogosModelo = await _context
                        .Jogos
                        .AsNoTracking()
                        .Where(x => x.User == "administrador@gmail.com")
                        .ToListAsync();  

                    foreach(Microsoft.AspNetCore.Identity.IdentityUser user in allUsers){
                        ranking.User = user.UserName;
                        ranking.Pontos = 0;
                       
                        var allJogos = await _context
                        .Jogos
                        .Where(x => x.User == user.UserName)
                        .ToListAsync();

                        foreach(Jogo jogo in allJogos){
                            Jogo jogoModelo = jogosModelo.Find(x => x.IdMandante == jogo.IdMandante);

                            ranking.Pontos = ranking.Pontos + acertouGolsMandante(jogoModelo, jogo);
                            ranking.Pontos = ranking.Pontos + acertouGolsVisitante(jogoModelo, jogo);
                        }

                        await Create(ranking);

                        ranking.Id = ranking.Id + 1;
                    }
                    return RedirectToAction(nameof(Index));
                }   
            }else{
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public int acertouGolsMandante(Jogo jogoModelo, Jogo jogo){
            return jogoModelo.GolsMandante == jogo.GolsMandante ? 1 : 0; 
        }

        public int acertouGolsVisitante(Jogo jogoModelo, Jogo jogo){
            return jogoModelo.GolsVisitante == jogo.GolsVisitante ? 3 : 0; 
        }

        // POST: Ranking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,User,Pontos")] Ranking ranking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ranking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ranking);
        }

        // GET: Ranking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ranking = await _context.Rankings.FindAsync(id);
            if (ranking == null)
            {
                return NotFound();
            }
            return View(ranking);
        }

        // POST: Ranking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,User,Pontos")] Ranking ranking)
        {
            if (id != ranking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ranking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RankingExists(ranking.Id))
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
            return View(ranking);
        }

        // GET: Ranking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ranking = await _context.Rankings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ranking == null)
            {
                return NotFound();
            }

            return View(ranking);
        }

        public async void DeleteConfirmedByUser(string user)
        {
            var ranking = await _context.Rankings.Where(x => x.User == user).ToListAsync();

            _context.Rankings.Remove(ranking.First());
            await _context.SaveChangesAsync();
        }

        // POST: Ranking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ranking = await _context.Rankings.FindAsync(id);
            _context.Rankings.Remove(ranking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RankingExists(int id)
        {
            return _context.Rankings.Any(e => e.Id == id);
        }
    }
}
