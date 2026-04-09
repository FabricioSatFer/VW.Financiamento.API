using Financiamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Financiamento.Infrastructure.Data
{
    public class FinanciamentoDbContext : DbContext
    {
        public FinanciamentoDbContext(DbContextOptions<FinanciamentoDbContext> options) : base(options)
        {
        }

        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // map to existing schema/tables
            modelBuilder.HasDefaultSchema("volkswagen");

            modelBuilder.Entity<Contrato>(b =>
            {
                b.ToTable("contratos");
                b.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd().HasDefaultValueSql("gen_random_uuid()");
                b.Property(x => x.ClienteCpfCnpj).HasColumnName("clientecpfcnpj").IsRequired();
                b.Property(x => x.ValorTotal).HasColumnName("valortotal").HasColumnType("numeric(18,2)");
                b.Property(x => x.TaxaMensal).HasColumnName("taxamensal").HasColumnType("numeric(5,2)");
                b.Property(x => x.PrazoMeses).HasColumnName("prazomeses");   
                b.Property(x => x.DataVencimentoPrimeiraParcela).HasColumnName("datavencimentoprimeiraparcela");
                b.Property(x => x.TipoVeiculo).HasColumnName("tipoveiculoid");
                b.Property(x => x.CondicaoVeiculo).HasColumnName("condicaoveiculoid");
                b.HasMany(x => x.Pagamentos).WithOne().HasForeignKey(p => p.ContratoId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Pagamento>(b =>
            {
                b.ToTable("pagamentos");
                b.Property(x => x.Id).HasColumnName("id");
                b.Property(x => x.ContratoId).HasColumnName("contratoid");
                b.Property(x => x.ParcelaNumero).HasColumnName("numeroparcela");
                b.Property(x => x.ValorPago).HasColumnName("valorpago").HasColumnType("numeric(18,2)");
                b.Property(x => x.DataPagamento).HasColumnName("datapagamento").IsRequired();
                b.Property(x => x.DataVencimento).HasColumnName("datavencimentoparcela").IsRequired();
                b.Property(x => x.Status).HasColumnName("statuspagamentoid");
            });
        }
    }
}
