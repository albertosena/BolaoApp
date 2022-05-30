using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BolaoApp.Models;

namespace BolaoApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Time> Times {get; set;}
        public DbSet<Jogo> Jogos {get; set;}
        public DbSet<Ranking> Rankings {get; set;}
    }
}
