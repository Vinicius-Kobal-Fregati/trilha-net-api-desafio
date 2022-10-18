using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Entities;

namespace TrilhaApiDesafio.Controllers
{
    /// <summary>
    /// Controller da entidade HistoricoTarefa, ela manipula os eventos, determinando qual resposta enviar para o usuário dependendo da 
    /// requisição e do que foi passado.
    /// </summary>
    [ApiController]
    [Route("HistoricoTarefa")]
    public class HistoricoTarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        /// <summary>
        /// Construtor da controller funcionario.
        /// </summary>
        /// <param name="context">Contexto passado a controller.</param>
        public HistoricoTarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Deve ser utilizado para saber todo o histórico de uma tarefa pelo seu Id.
        /// </summary>
        /// <param name="idTarefa">Id da tarefa a qual se deseja ver o histórico.</param>
        /// <returns>Retorna o status 200 ou 400 com suas informações no corpo.</returns>
        [HttpGet("{idTarefa}")]
        public IActionResult ObterHistoricoPorIdDaTarefa(int idTarefa)
        {
            var historico = _context.HistoricoTarefas.Where(item => item.TarefaId == idTarefa);

            if (historico == null || historico.Count() == 0)
                return NotFound(new { Error = Textos.NaoEncontrado("Histórico")} );

            return Ok(historico);
        }

        /// <summary>
        /// Registra um novo histórico na database.
        /// </summary>
        /// <param name="historico">Histórico a ser adicionado.</param>
        /// <returns>Retorna 201 ou 400.</returns>
        [NonAction]
        [HttpPost] //Talvez mudar para bool
        public IActionResult Criar(HistoricoTarefa historico)
        {
            if (historico == null)
                return BadRequest(new { Error = Textos.NaoNulo("Histórico")});

            _context.HistoricoTarefas.Add(historico);
            _context.SaveChanges();

            return CreatedAtAction(nameof(historico), new { id = historico.Id}, historico);
        }
    }
}