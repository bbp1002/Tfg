using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TFG_Cultivos.Models
{
    public class PacContext : IdentityDbContext<ApplicationUser>
    {
        public PacContext(DbContextOptions<PacContext> options)
            : base(options)
        {
        }

        public DbSet<Parcelas> Parcelas { get; set; }
        public DbSet<Recintos> Recintos { get; set; }
        public DbSet<DatoAgronomico> DatoAgronomico { get; set; }
        public DbSet<PropuestaCultivo> PropuestasCultivo { get; set; }

    }
}
