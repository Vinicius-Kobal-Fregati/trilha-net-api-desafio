using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Entities;

namespace TrilhaApiDesafio.Controllers
{
    /// <summary>
    /// Controller da entidade Tarefa, ela manipula os eventos, determinando qual resposta enviar para o usuário dependendo da requisição
    /// e do que foi passado.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;
        private FuncionarioController _funcionarioController;
        private HistoricoTarefaController _historicoTarefaController;

        /// <summary>
        /// Construtor da controller funcionario, passa o context para a propriedade _context e instancia dois objetos, o _funcionarioController
        /// e _historicoTarefaController. 
        /// </summary>
        /// <param name="context">Contexto passado a controller.</param>
        public TarefaController(OrganizadorContext context)
        {
            _context = context;
            _funcionarioController = new FuncionarioController(context);
            _historicoTarefaController = new HistoricoTarefaController(context);
        }

        /// <summary>
        /// Método GET, obtém a tarefa pelo seu Id.
        /// </summary>
        /// <param name="id">Id da tarefa que se deseja obter.</param>
        /// <returns>Pode retornar o status 200 ou 404, com as informações da tarefa no corpo.</returns>
        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var tarefa = _context.Tarefas.Find(id);

            if (tarefa == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });
            
            return Ok(tarefa);
        }

        /// <summary>
        /// Método GET, exibe todas as tarefas cadastradas.
        /// </summary>
        /// <returns>Pode retornar status 200 (com as informações no corpo) ou 404.</returns>
        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var tarefa = _context.Tarefas;

            if (tarefa == null)
                return NotFound(new { Error = Textos.NaoCadastrado("Tarefa") });
            
            return Ok(tarefa);
        }

        /// <summary>
        /// Método GET, obtém a tarefa pelo seu título.
        /// </summary>
        /// <param name="titulo">Titulo da tarefa que se deseja receber.</param>
        /// <returns>Pode retornar 200 com a tarefa em seu corpo ou 404.</returns>
        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            var tarefa = _context.Tarefas.Where(x => x.Titulo.Contains(titulo));

            if (tarefa.Count() == 0)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });
            
            return Ok(tarefa);
        }

        /// <summary>
        /// Método GET utilizado para receber informações da tarefa pela sua data de conclusão.
        /// </summary>
        /// <param name="data">Data a qual se deseja receber a tarefa.</param>
        /// <returns>Pode retornar 200 com a tarefa em seu corpo ou 404.</returns>
        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            if (tarefa.Count() == 0)
                return NotFound(new {Error = Textos.NaoEncontrado("Tarefa") });
            
            return Ok(tarefa);
        }

        /// <summary>
        /// Método GET utilizado para receber informações da tarefa pelo seu status.
        /// </summary>
        /// <param name="status">Status da tarefa que se deseja receber.</param>
        /// <returns>Pode retornar 200 com a tarefa em seu corpo ou 404.</returns>
        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            var tarefa = _context.Tarefas.Where(x => x.Status == status);

            if (tarefa.Count() == 0)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });
            
            return Ok(tarefa);
        }

        /// <summary>
        /// Método GET utilizado para receber informações do funcionário responsável pela tarefa.
        /// </summary>
        /// <param name="id">Id da tarefa da qual se deseja receber informações do funcionário.</param>
        /// <returns>Pode retornar 200 com a tarefa em seu corpo ou 404.</returns>
        [HttpGet("ObterResponsavel/{id}")]
        public IActionResult ObterResponsavel(int id)
        {
            var tarefa = _context.Tarefas.Find(id);

            if (tarefa == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });

            return _funcionarioController.ObterPorId(tarefa.FuncionarioId);
        }

        /// <summary>
        /// Método post, adiciona uma tarefa ao database.
        /// </summary>
        /// <param name="tarefa">Tarefa que se deseja adicionar.</param>
        /// <returns>Pode retornar o status 201 ou 400.</returns>
        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            DateOnly dataAtual = DateOnly.FromDateTime(DateTime.Now);
            DateOnly dataRecebida = DateOnly.FromDateTime(tarefa.Data);

            if (tarefa == null)
                return BadRequest(new { Erro = Textos.NaoNulo("Tarefa") });
            else if (tarefa.Titulo == null)
                return BadRequest(new { Error = Textos.NaoNulo("Título") });
            else if (tarefa.Titulo == "")
                return BadRequest(new { Erro = Textos.NaoVazio("Título") });
            else if (dataRecebida < dataAtual)
                return BadRequest(new { Error = Textos.DataMenorQueAtual() });

            _context.Add(tarefa);
            _context.SaveChanges();
            _historicoTarefaController.Criar(new HistoricoTarefa(tarefa.Id, tarefa.FuncionarioId, tarefa.Status));

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        /// <summary>
        /// Método PUT, deve ser utilizado para atualizar uma tarefa já existente.
        /// </summary>
        /// <param name="id">Id da tarefa que se quer atualizar.</param>
        /// <param name="tarefa">Nova tarefa.</param>
        /// <returns>Pode retornar status 204, 400 ou 404</returns>
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);
            var funcionarioNovo = _context.Funcionarios.Find(tarefa.FuncionarioId);
            DateOnly dataAtual = DateOnly.FromDateTime(DateTime.Now);
            DateOnly dataRecebida = DateOnly.FromDateTime(tarefa.Data);

            if (tarefaBanco == null)
                return NotFound();
            else if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = Textos.DataMenorQueMinimo() });
            else if (tarefa.Titulo == null)
                return BadRequest(new { Error = Textos.NaoNulo("Título") });
            else if (tarefa.Titulo == "")
                return BadRequest(new { Erro = Textos.NaoVazio("Título") });
            else if (dataRecebida < dataAtual)
                return BadRequest(new { Error = Textos.DataMenorQueAtual() });

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Status = tarefa.Status;

            if (tarefa.FuncionarioId == 0)
                tarefaBanco.FuncionarioId = 0;
            else if (funcionarioNovo == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });
            else
                tarefaBanco.FuncionarioId = tarefa.FuncionarioId;

            _context.SaveChanges();
            _historicoTarefaController.Criar(new HistoricoTarefa(id, tarefa.FuncionarioId, tarefa.Status));

            return NoContent();
        }

        /// <summary>
        /// Método PATCH, utilizado para atualizar o título de uma tarefa.
        /// </summary>
        /// <param name="id">Id da tarefa que se deseja atualizar.</param>
        /// <param name="titulo">Novo título</param>
        /// <returns>Pode retornar os status 204, 400 e 404.</returns>
        [HttpPatch("AtualizarTitulo/{id}")]
        public IActionResult AtualizarTitulo(int id, string titulo)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });
            else if (titulo == null)
                return BadRequest(new { Error = Textos.NaoNulo("Título") });
            else if (titulo == "")
                return BadRequest(new { Error = Textos.NaoVazio("Título") });

            tarefaBanco.Titulo = titulo;
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Método PATCH, utilizado para atualizar a descrição de uma tarefa.
        /// </summary>
        /// <param name="id">Id da tarefa que se deseja atualizar.</param>
        /// <param name="descricao">Nova descrição</param>
        /// <returns>Pode retornar os status 204, 400 e 404.</returns>
        [HttpPatch("AtualizarDescricao/{id}")]
        public IActionResult AtualizarDescricao(int id, string descricao)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });
            else if (descricao == null)
                return BadRequest(new { Error = Textos.NaoNulo("Descrição") });

            tarefaBanco.Descricao = descricao;
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Método PATCH, utilizado para atualizar a data de uma tarefa.
        /// </summary>
        /// <param name="id">Id da tarefa que se deseja atualizar.</param>
        /// <param name="data">Nova data final.</param>
        /// <returns>Pode retornar os status 204, 400 e 404.</returns>
        [HttpPatch("AtualizarData/{id}")]
        public IActionResult AtualizarData(int id, DateTime data)
        {
            var tarefaBanco = _context.Tarefas.Find(id);
            DateOnly dataAtual = DateOnly.FromDateTime(DateTime.Now);
            DateOnly dataRecebida = DateOnly.FromDateTime(data);

            if (tarefaBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });
            else if (data == null)
                return BadRequest(new { Error = Textos.NaoNulo("Data") });
            else if (dataRecebida < dataAtual)
                return BadRequest(new { Error = Textos.DataMenorQueAtual() });

            tarefaBanco.Data = data;
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Método PATCH, utilizado para atualizar o funcionário de uma tarefa.
        /// </summary>
        /// <param name="id">Id da tarefa que se deseja atualizar.</param>
        /// <param name="idFuncionario">Novo funcionário responsável pela tarefa.</param>
        /// <returns>Pode retornar os status 204, 400 e 404.</returns>
        [HttpPatch("AtualizarFuncionario/{id}")] //Trocar esse nome
        public IActionResult AtualizarFuncionario(int id, int idFuncionario)
        {
            var tarefaBanco = _context.Tarefas.Find(id);
            var funcionarioNovo = _context.Funcionarios.Find(idFuncionario);

            if (tarefaBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });
            else if (idFuncionario == 0)
                tarefaBanco.FuncionarioId = 0;
            else if (funcionarioNovo == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });
            else
                tarefaBanco.FuncionarioId = funcionarioNovo.Id;
                
            _context.SaveChanges();
            _historicoTarefaController.Criar(new HistoricoTarefa(id, tarefaBanco.FuncionarioId, tarefaBanco.Status));

            return NoContent();
        }

        /// <summary>
        /// Método PATCH, utilizado para atualizar o status de uma tarefa.
        /// </summary>
        /// <param name="id">Id da tarefa que se deseja atualizar.</param>
        /// <param name="status">Novo status.</param>
        /// <returns>Pode retornar os status 204 ou 400.</returns>
        [HttpPatch("AtualizarStatus/{id}")]
        public IActionResult AtualizarStatus(int id, EnumStatusTarefa status)
        {
            var tarefaBanco = _context.Tarefas.Find(id);
            var funcionarioBanco = tarefaBanco.FuncionarioId;

            if (tarefaBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });
            else if (funcionarioBanco == 0)
                return BadRequest(new { Error = Textos.NaoSelecionado("Funcionário") });

            tarefaBanco.Status = status;
            _context.SaveChanges();
            _historicoTarefaController.Criar(new HistoricoTarefa(id, funcionarioBanco, status));

            return NoContent();
        }

        /// <summary>
        /// Método DELETE utilizado para apagar uma tarefa do banco de dados.
        /// </summary>
        /// <param name="id">Id da tarefa que se deseja apagar.</param>
        /// <returns>Pode retornar os status 204 ou 404.</returns>
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Tarefa") });

            _historicoTarefaController.Criar(new HistoricoTarefa(tarefaBanco.Id, tarefaBanco.FuncionarioId, tarefaBanco.Status));
            _context.Tarefas.Remove(tarefaBanco);
            _context.SaveChanges();

            return NoContent();
        }
    }
}