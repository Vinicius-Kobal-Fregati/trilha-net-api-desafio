using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Entities
{
    /// <summary>
    /// Entidade que têm todas as propriedades para garantir a rastreabilidade do processo de uma tarefa, contendo as informações do id da
    /// tarefa, id do funcionário, o status da tarefa e a data que ela foi registrada.
    /// </summary>
    public class HistoricoTarefa
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Tarefa")]
        public int TarefaId { get; set; }
        [ForeignKey("Funcionario")]
        public int FuncionarioId { get; set; }
        public EnumStatusTarefa StatusTarefa { get; set; }
        public DateTime DataRegistro { get; set; }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public HistoricoTarefa()
        {

        }

        /// <summary>
        /// Sobregarca do construtor, instancia a classe e atribui os argumentos as suas propriedades.
        /// </summary>
        /// <param name="tarefaId">Id referente a tarefa.</param>
        /// <param name="funcionarioId">Id referente ao funcionário.</param>
        /// <param name="statusTarefa">Status da tarefa.</param>
        public HistoricoTarefa(int tarefaId, int funcionarioId, EnumStatusTarefa statusTarefa)
        {
            TarefaId = tarefaId;
            FuncionarioId = funcionarioId;
            StatusTarefa = statusTarefa;
            DataRegistro = DateTime.Now;
        }
    }
}