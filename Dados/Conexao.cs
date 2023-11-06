using System;
using System.Data;
using Microsoft.Extensions.WebEncoders.Testing;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace Rinha_de_back_end.Dados
{
    public class Conexao
    {
        private MySqlConnection conn {  get; set; }
        private MySqlCommand command {  get; set; }
        private string error {  get; set; }

        public Conexao()
        {
            conn = new MySqlConnection("server=192.168.10.50;User Id=subaru3rem; database=rinhadev; password=93439978");
            command = new MySqlCommand("",conn);
        }
        public void Append(string comand)
        {
            command.CommandText += comand;
        }
        public void Add(string key, object value)
        {
            command.Parameters.AddWithValue(key, value);
        }
        public ResultRequest<object> GetConnection() 
        {
            ResultRequest<object> result = new ResultRequest<object>();
            try
            {
                conn.Open();
                return result;
            }
            catch (MySqlException ex)
            {
                if(ex.Number == 1045)
                {
                    this.error = "Usuario ou senha incorretos.";
                    result.message = "Usuario ou senha incorretos.";
                    result.status = false;
                    return result;
                }
                this.error = "Não foi possivel conectar ao servidor.";
                result.message = "Não foi possivel conectar ao servidor.";
                result.status = false;
                return result;
            }
        }
        public ResultRequest<object> Close()
        {
            ResultRequest<object> result = new ResultRequest<object>();
            try
            {
                conn.Close();
                this.error = "";
            }
            catch (MySqlException ex)
            {
                this.error = ex.Message;
                result.message = ex.Message;
                result.status = false;
            }
            return result;
        }
        public int ExecuteNonQuery()
        {
            try
            {
                return command.ExecuteNonQuery();
            }catch (MySqlException ex)
            {
                if(ex.Number == 1062 || ex.Number == 1048)
                {
                    return -1;
                }
                return 0;
            }
            finally
            {
                command = new MySqlCommand("", conn);
            }
        }
        public object ExecuteEscalar()
        {
            var result = command.ExecuteScalar();
            command = new MySqlCommand("", conn);
            return result;
        }
        public DataTable GetTable()
        {
            DataTable dt = new DataTable();
            MySqlDataAdapter data = new MySqlDataAdapter();
            data.SelectCommand = command;
            data.Fill(dt);

            command = new MySqlCommand("", conn);
            return dt;
        }
        public string GetError()
        {
            return this.error;
        }
    }
}
