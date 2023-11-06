namespace Rinha_de_back_end.Dados
{
    public class ResultRequest<T>
    {
        #region: Membros da classe
        /// <summary>
        /// Informa se a requisição foi realizada com sucesso.
        /// </summary>
        public bool status { get; set; }
        /// <summary>
        /// Mensagem referente ao status da requisição.
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Objeto de retorno do tipo definido na declaração.
        /// </summary>
        public T data { get; set; }
        #endregion

        #region: Construtor
        /// <summary>
        /// Cria uma nova instância da classe de resultado.
        /// </summary>
        public ResultRequest()
        {
            this.status = true;
            this.message = string.Empty;
            this.data = Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Cria uma nova instância da classe de resultado.
        /// </summary>
        /// <param name="message">Mensagem default.</param>
        public ResultRequest(string message)
        {
            this.status = true;
            this.message = message;
            this.data = Activator.CreateInstance<T>();
        }
        #endregion
    }
}
