using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace TesteCandidato
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Bem vindo ao teste de Back-end da e.Mix!

            Abaixo está desenvolvido de uma forma bem simples e com alguns erros uma consulta de CEP.

            O que esperamos de você neste teste é que faça um novo projeto WEB da forma mais correta, segura e performática na sua avaliação com base no código abaixo.

            Entre os códigos você pode notar que existem observações "To Do" que também devem ser realizadas para que o teste esteja correto.
            Exemplo: "TODO: Criar banco de dados - LocalDB com o nome CEP"

            Observação: Você poderá utilizar qualquer tecnologia ou framework da sua preferência.

            */

            //TODO: Fazer um projeto WEB

            //TODO: Perguntar se o usuário quer consultar se logradouro existe na base

            //TODO: Criar banco de dados - LocalDB com o nome CEP
            //TODO: Adicionar tabela conforme script abaixo
            //USE [CEP]
            //GO

            //SET ANSI_NULLS ON
            //GO

            //SET QUOTED_IDENTIFIER ON
            //GO

            //CREATE TABLE [dbo].[CEP] (
            //    [Id]          INT            IDENTITY (1, 1) NOT NULL,
            //    [cep]         CHAR (9)       NULL,
            //    [logradouro]  NVARCHAR (500) NULL,
            //    [complemento] NVARCHAR (500) NULL,
            //    [bairro]      NVARCHAR (500) NULL,
            //    [localidade]  NVARCHAR (500) NULL,
            //    [uf]          CHAR (2)       NULL,
            //    [unidade]     BIGINT         NULL,
            //    [ibge]        INT            NULL,
            //    [gia]         NVARCHAR (500) NULL
            //);
            string cep;
            string result = string.Empty;
            void ObterCEP()
            {
                Console.WriteLine("Digite um CEP para ser buscado:");

                cep = Console.ReadLine();
                ValidaCEP(cep);
            }
            ObterCEP();

            //TODO: Implementar forma de fazer o usuário poder errar várias vezes o CEP informado
            //TODO: Melhorar validação do CEP.
            bool ValidaCEP(string cepvalid)
            {
                // Remover .
                cepvalid = cepvalid.Replace(".", "");
                // Remover -
                cepvalid = cepvalid.Replace("-", "");
                // Remover espaços
                cepvalid = cepvalid.Replace(" ", "");

                Regex Rgx = new Regex(@"^\d{8}$");
                if (!Rgx.IsMatch(cepvalid))
                {
                    Console.WriteLine("CEP Invalido!");
                    ObterCEP();
                    return false;
                }
                else
                    return true;
            }

            //Exemplo CEP 13050020
            //https://viacep.com.br/ws/72930000/json/
            string viaCEPUrl = "https://viacep.com.br/ws/" + cep + "/json/";

            Console.WriteLine("O CEP informado foi: " + cep);

            //TODO: Resolver dados com caracter especial no retorno do JSON 
            // Evitar ERRO DE SSL.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                WebRequest webRequest = WebRequest.Create(viaCEPUrl);
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    result = reader.ReadToEnd();
                } else
                {
                    throw new Exception("Erro requisição API");
                }
            }
            catch(Exception ex)
            {

                throw new Exception("Ocorreu um erro ao fazer requisição para API");

            }


            //TODO: Tratar CEP Inválido.

            dynamic jsonRetorno = JObject.Parse(result);
            if(jsonRetorno.erro == true)
            {
                Console.WriteLine("CEP não encontrado na API");
            }


            //TODO: Validar CEP existente

            // conexão com database localdb
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CEP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
            SqlConnection connection = new SqlConnection(connectionString);


            string query = "INSERT INTO [dbo].[CEP] ([cep], [logradouro], [complemento], [bairro], [localidade], [uf], [unidade], [ibge], [gia]) VALUES (";
            query = query + "'" + jsonRetorno["cep"] + "'";
            query = query + ",'" + jsonRetorno["logradouro"] + "'";
            query = query + ",'" + jsonRetorno["complemento"] + "'";
            query = query + ",'" + jsonRetorno["bairro"] + "'";
            query = query + ",'" + jsonRetorno["localidade"] + "'";
            query = query + ",'" + jsonRetorno["uf"] + "'";
            query = query + ",'" + jsonRetorno["unidade"] + "'";
            query = query + ",'" + jsonRetorno["ibge"] + "'";
            query = query + ",'" + jsonRetorno["gia"] + "'" + ")";

            SqlCommand sqlCommand = new SqlCommand(query, connection);

            sqlCommand.CommandType = CommandType.Text;

            try
            {
                connection.Open();

                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            sqlCommand.Dispose();
            connection.Close();
            connection.Dispose();

            //TODO: Retornar os dados do CEP infomado no início para o usuário

            Console.WriteLine("Deseja visualizar todos os CEPs de alguma UF? Se sim, informar UF, se não, informar sair.");
            string resposta = Console.ReadLine();

            if (resposta == "sair")
            {
                return;
            }

            if (resposta.Length > 2)
            {
                Console.WriteLine("UF inválida");
                resposta = Console.ReadLine();
            }

            if (resposta == "sair")
            {
                return;
            }



            connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CEP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            sqlCommand = new SqlCommand("Select * from CEP", connection);

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataView dv;
            sqlCommand.CommandType = CommandType.Text;

            try
            {
                connection.Open();
                adapter.SelectCommand = sqlCommand;
                adapter.Fill(ds, "Create DataView");
                adapter.Dispose();

                dv = ds.Tables[0].DefaultView;

                for (int i = 0; i < dv.Count; i++)
                {
                    if (dv[i]["uf"].ToString() == resposta)
                    {
                        if (i == 0)
                        {
                            Console.Write(dv[i]["cep"]);
                        }
                        else
                        {
                            Console.Write(";" + dv[i]["cep"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            sqlCommand.Dispose();
            connection.Close();
            connection.Dispose();

            Console.ReadLine();
        }
    }
}
