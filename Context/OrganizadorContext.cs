using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Entities;

namespace TrilhaApiDesafio.Context
{
    /// <summary>
    /// Classe Context, ela administra as nossas entidades, persiste seus dados, controla as alterações e preenche suas informações através
    /// do banco de dados.
    /// </summary>
    public class OrganizadorContext : DbContext
    {
        /// <summary>
        /// Construtor da nossa context, através dele pegamos as informações de conexão do banco de dados (presente no appsettings) e 
        /// passamos para a classe mãe, a DbContext.
        /// </summary>
        /// <param name="options">Informações sobre a forma de conexão ao banco de dados, como: servidor, banco de dados utilizado, nome do 
        /// database e se terá autenticação ou não.</param>
        public OrganizadorContext(DbContextOptions<OrganizadorContext> options) : base(options) { }

        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<HistoricoTarefa> HistoricoTarefas { get; set; }
    }
}