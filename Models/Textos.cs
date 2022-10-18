namespace TrilhaApiDesafio.Models
{
    /// <summary>
    /// Classe estática auxiliar, ela possuí os textos de erro.
    /// </summary>
    public static class Textos
    {
        public static string NaoEncontrado(string nome)
        {
            return $"{nome} não encontrado(a)";
        }

        public static string NaoNulo(string nome)
        {
            return $"{nome} não pode ser nulo(a)";
        }

        public static string NaoVazio(string nome)
        {
            return $"{nome} não pode ser vazio";
        }

        public static string NaoCadastrado(string nome)
        {
            return $"{nome} não cadastrado(a)";
        }

        public static string TelefoneForaPadrao()
        {
            return $"Telefone está fora do formato padrão, sendo ele: 11111-1111 ou (11)11111-1111";
        }

        public static string JaExistente(string nome)
        {
            return $"{nome} já existente";
        }

        public static string DataMenorQueAtual()
        {
            return "Data não pode ser menor que a atual";
        }

        public static string DataMenorQueMinimo()
        {
            return "A data da tarefa não pode ser menor que o mínimo";
        }

        public static string NaoSelecionado(string nome)
        {
            return $"{nome} não selecionado";
        }
    }
}