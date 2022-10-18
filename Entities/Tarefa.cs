using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Entities
{
    /// <summary>
    /// Entidade que representa uma tarefa, ela tem propriedades importantes como o título, a descrição, data para se finalizar, status e o 
    /// Id do funcionário.
    /// </summary>
    public class Tarefa
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public EnumStatusTarefa Status { get; set; }
        [ForeignKey("Funcionario")]
        public int FuncionarioId { get; set; }
    }
}