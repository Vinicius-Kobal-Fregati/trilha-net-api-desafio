using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;
using TrilhaApiDesafio.Entities;
using System.Text.RegularExpressions;

namespace TrilhaApiDesafio.Controllers
{
    /// <summary>
    /// Controller da entidade Funcionario, ela manipula os eventos, determinando qual resposta enviar para o usuário dependendo da requisição
    /// e do que foi passado.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        /// <summary>
        /// Construtor da controller funcionario.
        /// </summary>
        /// <param name="context">Contexto passado a controller.</param>
        public FuncionarioController(OrganizadorContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Método do tipo GET, utilizado para receber todos os funcionários que estão cadastrados.
        /// </summary>
        /// <returns>Pode retornar o status 404 ou 200, tendo em seu corpo o json com todos os funcionários cadastrados.</returns>
        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
             var funcionarios = _context.Funcionarios;

            if (funcionarios.Count() == 0)
                return NotFound(new {Error = Textos.NaoEncontrado("Funcionário")} );
            
            return Ok(funcionarios);
        }

        /// <summary>
        /// Método GET usado para se obter um funcionário pelo seu Id.
        /// </summary>
        /// <param name="id">Id do usuário que se deseja encontrar.</param>
        /// <returns>Retorna status 404 ou 200, possuindo um json em seu corpo com as informações do funcionário.</returns>
        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            if (id == 0)
                return NotFound(new {Error = Textos.NaoSelecionado("Funcionário")} );
            
            var funcionario = _context.Funcionarios.Find(id);
            
            if (funcionario == null)
                return NotFound(new {Error = Textos.NaoEncontrado("Funcionário")} );

            return Ok(funcionario);
        }

        /// <summary>
        /// Método GET usado para se obter todos os funcionários que tenham um determinado nome.
        /// </summary>
        /// <param name="nome">Nome do(s) funcionário(s) que se deseja pesquisar</param>
        /// <returns>Pode retornar status 404 ou 200, com um json em seu corpo com a informação de todos os funcionários com esse
        /// nome encontrados.</returns>
        [HttpGet("ObterPorNome")]
        public IActionResult ObterPorNome(string nome)
        {
            var funcionarios = _context.Funcionarios.Where(funcionario => funcionario.Nome.Contains(nome));
            if (funcionarios == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });
            else if (funcionarios.Count() == 0)
                return NotFound(new { Error = Textos.NaoCadastrado("Funcionário") });

            return Ok(funcionarios);
        }

        /// <summary>
        /// Método GET usado para se obter o funcionário que tenha um determinado e-mail.
        /// </summary>
        /// <param name="email">Nome do funcionário que se deseja pesquisar</param>
        /// <returns>Pode retornar status 404 ou 200, com um json em seu corpo com a informação do funcionário com esse e-mail 
        /// encontrado. Note que se só passar uma parte do e-mail, ele poderá devolver outros que contenham essa parte.</returns>
        [HttpGet("ObterPorEmail")]
        public IActionResult ObterPorEmail(string email)
        {
            var funcionarios = _context.Funcionarios.Where(funcionario => funcionario.Email.Contains(email));
            if (funcionarios == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });
            else if (funcionarios.Count() == 0)
                return NotFound(new { Error = Textos.NaoCadastrado("Funcionário") });

            return Ok(funcionarios);
        }

        /// <summary>
        /// Método GET usado para se obter o funcionário que tenha um determinado telefone.
        /// </summary>
        /// <param name="telefone">Nome do funcionário que se deseja pesquisar</param>
        /// <returns>Pode retornar status 404 ou 200, com um json em seu corpo com a informação do funcionário com esse telefone 
        /// encontrado. Note que se só passar uma parte do telefone, ele poderá devolver outros que contenham essa parte.</returns>
        [HttpGet("ObterPorTelefone")]
        public IActionResult ObterPorTelefone(string telefone)
        {
            var funcionario = _context.Funcionarios.Where(funcionario => funcionario.Telefone.Contains(telefone));
            if (funcionario == null || funcionario.Count() == 0)
                return NotFound(new { Error = Textos.NaoCadastrado("Funcionário") });

            return Ok(funcionario);
        }

        /// <summary>
        /// Método POST, adiciona um novo funcionário no database, verificando-se algumas informações são vazias, como nome, email, telefone
        /// e se o nome já não existe.
        /// </summary>
        /// <param name="funcionario">Objeto da classe funcionário que se deseja adicionar</param>
        /// <returns>Pode retornar status 400 ou 201, possuindo informações como o nome da ação, a rota e o objeto adicionado.</returns>
        [HttpPost]
        public IActionResult CadastrarFuncionario(Funcionario funcionario)
        {
            if (funcionario.Nome == "")
                return BadRequest(new { Error = Textos.NaoVazio("Nome") });
            else if (funcionario.Email == "")
                return BadRequest(new { Error = Textos.NaoVazio("E-mail") });
            else if (funcionario.Telefone == "")
                return BadRequest(new { Error = Textos.NaoVazio("Telefone") });
            else if (NomeJaExistente(funcionario.Nome))
                return BadRequest(new { Error = Textos.JaExistente("Nome") });
            else if (!VerificaTelefone(funcionario.Telefone))
                return BadRequest(new { Error = Textos.TelefoneForaPadrao() });

            _context.Add(funcionario);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObterPorId), new { id = funcionario.Id}, funcionario);
        }

        /// <summary>
        /// Método PUT, utilizado para atualizar as informações de um funcionário já existente.
        /// </summary>
        /// <param name="id">Id do usuário que se quer atualizar</param>
        /// <param name="funcionario">Novas informações do usuário</param>
        /// <returns>Pode retornar status 204, 400 ou 404, não possuindo informações sobre o funcionário em seu corpo, visto que é uma
        /// atualização.</returns>
        [HttpPut("{id}")]
        public IActionResult AtualizarFuncionario(int id, Funcionario funcionario)
        {
            var funcionarioBanco = _context.Funcionarios.Find(id);

            if (funcionarioBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });
            else if (funcionario.Nome == "")
                return BadRequest(new { Error = Textos.NaoVazio("Nome") });
            else if (funcionario.Nome == null)
                return BadRequest(new { Error = Textos.NaoNulo("Nome") });
            else if (NomeJaExistente(funcionario.Nome))
                return BadRequest(new { Error = Textos.JaExistente("Nome") });
            else if (funcionario.Email == "")
                return BadRequest(new { Error = Textos.NaoVazio("E-mail") });
            else if (funcionario.Telefone == "")
                return BadRequest(new { Error = Textos.NaoVazio("Telefone")});
            else if (!VerificaTelefone(funcionario.Telefone))
                return BadRequest(new { Error = Textos.TelefoneForaPadrao() });

            funcionarioBanco.Nome = funcionario.Nome;
            funcionarioBanco.Email = funcionario.Email;
            funcionarioBanco.Telefone = funcionario.Telefone;
            _context.SaveChanges();
            
            return NoContent();
        }

        /// <summary>
        /// Método PATCH, ele permite atualizar uma parte das informações, nesse caso, o nome do funcionário, verificando se o mesmo 
        /// é vazio, nulo ou repetido.
        /// </summary>
        /// <param name="id">Id do usuário que se quer atualizar</param>
        /// <param name="nome">Novo nome que o usuário passará a ter</param>
        /// <returns>Pode retornar 204, 400 ou 404, </returns>
        [HttpPatch("AtualizarNome/{id}")]
        public IActionResult AtualizarNome(int id, string nome)
        {
            var funcionarioBanco = _context.Funcionarios.Find(id);

            if (funcionarioBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });
            else if (nome == null)
                return BadRequest(new { Error = Textos.NaoNulo("Nome") });
            else if (NomeJaExistente(nome))
                return BadRequest(new { Error = Textos.JaExistente("Nome") });

            funcionarioBanco.Nome = nome;
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Método PATCH utilizado para atualizar o e-mail do funcionário, verificando se ele é vazio, nulo e se o funcionário foi encontrado.
        /// </summary>
        /// <param name="id">Id do usuário que se deseja atualizar.</param>
        /// <param name="email">Email novo a ser atribuído ao funcionário.</param>
        /// <returns>Pode retornar status 204, 400 ou 404.</returns>
        [HttpPatch("AtualizarEmail/{id}")]
        public IActionResult AtualizarEmail(int id, string email)
        {
            var funcionarioBanco = _context.Funcionarios.Find(id);

            if (funcionarioBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });
            else if (email == null)
                return BadRequest(new { Error = Textos.NaoNulo("E-mail") });
            else if (email == "")
                return BadRequest(new { Error = Textos.NaoVazio("E-mail") });
            
            funcionarioBanco.Email = email;
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Método PATCH utilizado para atualizar o telefone do funcionário, verificando se ele é nulo, vazio e se encontrou o funcionário.
        /// </summary>
        /// <param name="id">Id do usuário que se quer atualizar.</param>
        /// <param name="telefone">Novo telefone para atribuir ao funcionário.</param>
        /// <returns>Retornar status 204, 400 ou 404.</returns>
        [HttpPatch("AtualizarTelefone/{id}")]
        public IActionResult AtualizarTelefone(int id, string telefone)
        {
            var funcionarioBanco = _context.Funcionarios.Find(id);

            if (funcionarioBanco == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });
            else if (telefone == null)
                return BadRequest(new { Error = Textos.NaoNulo("Telefone") });
            else if (telefone == "")
                return BadRequest(new { Error = Textos.NaoVazio("Telefone") });
            else if (!VerificaTelefone(telefone))
                return BadRequest(new { Error = Textos.TelefoneForaPadrao() });

            funcionarioBanco.Telefone = telefone;
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Método DELETE, deve ser utilizado para apagar um funcionário do banco de dados.
        /// </summary>
        /// <param name="id">Id do usuário que se deseja apagar.</param>
        /// <returns>Pode retornar status 204 ou 404.</returns>
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var funcionario = _context.Funcionarios.Find(id);

            if (funcionario == null)
                return NotFound(new { Error = Textos.NaoEncontrado("Funcionário") });

            _context.Funcionarios.Remove(funcionario);
            _context.SaveChanges();

            return NoContent();
        }
        
        /// <summary>
        /// Método auxiliar que verifica se o nome passado já está cadastrado.
        /// </summary>
        /// <param name="nome">Nome que se deseja verificar.</param>
        /// <returns>Retorna um booleano se o nome é repetido ou não.</returns>
        [NonAction]
        public bool NomeJaExistente(string nome)
        {
            bool nomeRepetido = _context.Funcionarios.Where(x => x.Nome.Contains(nome)).Count() > 0 ? true : false;

            return nomeRepetido;
        }

        /// <summary>
        /// Método auxiliar, verifica se um número está de acordo com os padrões
        /// </summary>
        /// <param name="telefone">Telefone que se deseja verificar</param>
        /// <returns>Retorna um booleano dizendo se o número é válido ou não</returns>
        [NonAction]
        public bool VerificaTelefone(string telefone)
        {
            bool numeroValido = false;
            Regex numeroCelular = new Regex("^[0-9]{5}-[0-9]{4}$");
            Regex numeroCelularComDDD = new Regex("^\\([0-9]{2}\\)[0-9]{5}-[0-9]{4}$");

            if (numeroCelular.IsMatch(telefone) || numeroCelularComDDD.IsMatch(telefone))
                numeroValido = true;

            return numeroValido;
        }
    }
}