using Microsoft.AspNetCore.Mvc;
using Rinha_de_back_end.Dados;
using Rinha_de_back_end.Modelos;
using System.Data;

namespace Rinha_de_back_end.Controllers
{
    [Route("api/[controller]")]
    public class PessoasController : Controller
    {
        /// <summary>
        /// Metodo de criação de pessoa
        /// </summary>
        /// <param name="pessoa">objeto para criãção</param>
        /// <returns>Status de criação</returns>
        [HttpPost]
        public IActionResult Pessoas([FromBody] Pessoa pessoa)
        {
            if(pessoa == null)
            {
                return StatusCode(400, "Parametros invalidos");
            }
            var conn = new Conexao();

            conn.Append("insert into pessoas (apelido, nome, nascimento) ");
            conn.Append("values (@apelido, @nome, @nascimento);");
            if (!conn.GetConnection().status)
            {
                return StatusCode(500, conn.GetError());
            }

            conn.Add("apelido", pessoa.apelido);
            conn.Add("nome", pessoa.nome);
            conn.Add("nascimento", pessoa.nascimento.ToString("yyyy-MM-dd"));
            var result = conn.ExecuteNonQuery();

            if (result < 0)
            {
                return StatusCode(422, "Erro ao adicionar pessoa");
            }

            conn.Close();

            conn = new Conexao();
            conn.Append("select id from pessoas order by id desc limit 1");
            if (!conn.GetConnection().status)
            {
                return StatusCode(500, conn.GetError());
            }
            var id = conn.ExecuteEscalar();
            if (id == null)
                return StatusCode(500, "Erro ao obter id da pessoa");

            pessoa.id = Convert.ToInt32(id);
            conn.Close();

            if(pessoa.stacks.Count() > 0)
            {
                conn = new Conexao();
                foreach (string stack in pessoa.stacks)
                {
                    conn.Append("insert into stacksporpessoa (idPessoa, nomeStack) values (@idPessoa, @nomeStack);");
                    if (!conn.GetConnection().status)
                    {
                        return StatusCode(500, conn.GetError());
                    }

                    conn.Add("idPessoa", pessoa.id);
                    conn.Add("nomeStack", stack);
                    
                    if (conn.ExecuteNonQuery() < 0)
                    {
                        conn.Close();
                        return StatusCode(500, "Erro ao adicionar stacks");
                    }
                    conn.Close();
                }
            }

            return Created("http://localhost:5209/api/pessoas/" + pessoa.id, pessoa.id);
        }
        [HttpGet("{id}")]
        public IActionResult Pessoas(int id)
        {
            var conn = new Conexao();
            conn.Append("select * from pessoas where id = @id;");
            if (!conn.GetConnection().status)
            {
                conn.Close();
                return StatusCode(500, conn.GetError());
            }

            conn.Add("id", id);

            var dt = conn.GetTable();

            if(dt.Rows.Count <= 0)
            {
                conn.Close();
                return StatusCode(404, "pessoa não encontrada");
            }

            Pessoa pessoa = new Pessoa(dt.Rows[0]);

            return StatusCode(200, pessoa);
        }
        [HttpGet()]
        public IActionResult Pessoas([FromQuery] string t)
        {
            var result = new List<Pessoa>();
            var conn = new Conexao();

            conn.Append(" select distinct P.* from pessoas P");
            conn.Append(" left join stacksporpessoa S");
            conn.Append(" on S.idPessoa = P.id");
            conn.Append(" where nome like CONCAT('%', @termo, '%') ");
            conn.Append("     or apelido like CONCAT('%', @termo, '%') ");
            conn.Append("     or S.nomeStack like CONCAT('%', @termo, '%')");
            conn.Append(" LIMIT 50;");

            if (!conn.GetConnection().status)
            {
                return StatusCode(500, "Erro ao conectar com o banco de dados");
            }
            
            conn.Add("termo", t);

            var dt = conn.GetTable();
            conn.Close();

            foreach (DataRow row in dt.Rows)
            {
                Pessoa p = new Pessoa(row);
                conn = new Conexao();
                conn.Append("select nomeStack from stacksporpessoa where idPessoa = @idPessoa;");
                if (!conn.GetConnection().status)
                {
                    return StatusCode(500, "Erro ao conectar com o banco de dados");
                }
                conn.Add("idPessoa", p.id);
                var stacks = conn.GetTable();
                foreach(DataRow r in stacks.Rows)
                {
                    p.stacks.Add(r["nomeStack"].ToString()!);
                }

                conn.Close();
                result.Add(p);
            }

            return Ok(result);
        }
        [HttpGet("contagem-pessoas")]
        public IActionResult ContagemPessoas()
        {
            var conn = new Conexao();
            conn.Append("select Count(id) from pessoas;");
            if (!conn.GetConnection().status)
            {
                conn.Close();
                return StatusCode(500, conn.GetError());
            }
            var count = conn.ExecuteEscalar();

            return Ok(new { quantidade = count });
        }
    }
}
