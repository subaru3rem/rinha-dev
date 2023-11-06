using System;
using System.Data;

namespace Rinha_de_back_end.Modelos
{
    public class Pessoa
    {
        public int id { get; set; }
        public string apelido { get; set; }
        public string nome { get; set; }
        public DateTime nascimento { get; set;}
        public List<string> stacks { get; set; }

        public Pessoa()
        {
            this.id = -1;
            this.apelido = string.Empty;
            this.nome = string.Empty;
            this.nascimento = DateTime.MinValue;
            this.stacks = new List<string>();
        }
        public Pessoa(DataRow row)
        {
            this.id = Convert.ToInt32(row["id"]) ;
            this.apelido = row["apelido"].ToString()!;
            this.nome = row["nome"].ToString()!;
            this.nascimento = Convert.ToDateTime(row["nascimento"]);
            this.stacks = new List<string>();
        }
    }
}
