using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
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

            // TODO: Fazer um projeto WEB
            // estarei fazendo uma api como descrito pelo e-mail da Maria sobre a descrição disso.
            // TesteProjetoWEB - projeto api está incluido nessa solução

            //TODO: Perguntar se o usuário quer consultar se logradouro existe na base
            // conexão com database localdb
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CEP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
            var db = new SqlConnection(connectionString);

            //Abrir conexão
            try
            {
                db.Open();
            }
            catch(Exception ex)
            {
                throw new Exception("Ocorreu um erro ao tentar conectar com a db");
            }

            string reslog;
            void ObterLogradouro()
            {
                Console.WriteLine("Desejar buscar se um logradouro existe no banco de dados? caso queira responda com 'sim', caso não responda com 'não'");

                reslog = Console.ReadLine();
            }
            ObterLogradouro();

            void ValidaResposta()
            {
                if(reslog == "sim")
                {
                    Console.WriteLine("Digite o logradouro desejado:");
                    string logradouro = Console.ReadLine();

                    string queryFindLog = $"SELECT * FROM [CEP].[dbo].[CEP] WHERE logradouro = '{logradouro}'";
                    var response = db.QueryFirstOrDefault(queryFindLog);

                    if (response != null)
                    {
                        Console.WriteLine("Esse logradouro já existe no banco de dados");
                        Console.WriteLine("==================================================================");
                        Console.WriteLine(response.logradouro + " - " + response.localidade + '-' + response.uf + " - " + response.cep);
                        Console.WriteLine("==================================================================");
                    } else
                    {
                        Console.WriteLine("Logradouro não encontrado no banco de dados");
                        Console.WriteLine("==================================================================");
                    }

                    return;
                }
                else
                {
                    return;
                }
            }
            ValidaResposta();

            //TODO: Criar banco de dados - LocalDB com o nome CEP
            //TODO: Adicionar tabela conforme script abaixo
            // CREATE DATABASE [CEP]
            // GO

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
            bool valid = false;
            void ObterCep()
            {
                Console.WriteLine("Digite um CEP para ser buscado:");

                cep = Console.ReadLine();
                ValidaCep();
            }
            ObterCep();

            //TODO: Implementar forma de fazer o usuário poder errar várias vezes o CEP informado
            //TODO: Melhorar validação do CEP.
            bool ValidaCep()
            {
                // Remover .
                cep = cep.Replace(".", "");
                // Remover -
                cep = cep.Replace("-", "");
                // Remover espaços
                cep = cep.Replace(" ", "");

                Regex Rgx = new Regex(@"^\d{8}$");
                if (!Rgx.IsMatch(cep))
                {
                    Console.WriteLine("CEP Invalido!");
                    ObterCep();
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
                    throw new Exception("Erro na requisição API");
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
            } else
            {
                valid = true;
            }

            //TODO: Validar CEP existente

            string queryFindOne = $"SELECT * FROM [CEP].[dbo].[CEP] WHERE cep = '{cep}'";
            var exists = db.QueryFirstOrDefault(queryFindOne);
            if(exists != null)
            {
                Console.WriteLine("Esse CEP já existe no banco de dados");
            } else
            {
                // Query para adicionar novo CEP a DB
                string queryAddCep = "INSERT INTO [dbo].[CEP] ([cep], [logradouro], [complemento], [bairro], [localidade], [uf], [unidade], [ibge], [gia]) VALUES (@cep, @logradouro, @complemento, @bairro, @localidade, @uf, @unidade, @ibge, @gia)";
                var dp = new DynamicParameters();
                dp.Add("@cep", cep);
                dp.Add("@logradouro", $"{jsonRetorno.logradouro}");
                dp.Add("@complemento", $"{jsonRetorno.complemento}");
                dp.Add("@bairro", $"{jsonRetorno.bairro}");
                dp.Add("@localidade", $"{jsonRetorno.localidade}");
                dp.Add("@uf", $"{jsonRetorno.uf}");
                // Unidade não existe no retorno da api mas deixei para obdecer o script de criação da DB
                dp.Add("@unidade", $"{jsonRetorno.unidade}");
                dp.Add("@ibge", $"{jsonRetorno.ibge}");
                dp.Add("@gia", $"{jsonRetorno.gia}");

                // Executar Query
                if(valid)
                {
                    try
                    {
                        int res = db.Execute(queryAddCep, dp);
                        if(res > 0)
                        {
                            Console.WriteLine("Novo CEP inserido no Database");
                        }

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Ocorreu um erro ao tentar adicionar CEP a DB");
                    }
                }
            }




            //TODO: Retornar os dados do CEP infomado no início para o usuário

            if (valid)
            {
                string retornaInfoCep = $"CEP:{cep}\n";
                retornaInfoCep += $"LOGRADOURO:{jsonRetorno.logradouro}\n";
                retornaInfoCep += $"COMPLEMENTO:{jsonRetorno.complemento}\n";
                retornaInfoCep += $"BAIRRO:{jsonRetorno.bairro}\n";
                retornaInfoCep += $"LOCALIDADE:{jsonRetorno.localidade}\n";
                retornaInfoCep += $"UF:{jsonRetorno.uf}\n";
                retornaInfoCep += $"IBGE:{jsonRetorno.ibge}\n";
                retornaInfoCep += $"GIA:{jsonRetorno.gia}";

                Console.WriteLine("==================================================================");
                Console.WriteLine(retornaInfoCep);
                Console.WriteLine("==================================================================");

            }

            string resposta;

            void ObterUf()
            {
                Console.WriteLine("Deseja visualizar todos os CEPs de alguma UF? Se sim, informar UF, se não, informar sair.");
                resposta = Console.ReadLine();

            }
            ObterUf();


            if (resposta == "sair")
            {
                return;
            }

            Regex RgxUf = new Regex(@"[aA-zZ]{2}");
            if (RgxUf.IsMatch(resposta))
            {
                try
                {
                    var query = $"SELECT * FROM[CEP].[dbo].[CEP] WHERE uf = '{resposta}'";

                    var queryFindAllCepByUf = db.Query(query).ToList();
                    Console.WriteLine("==================================================================");
                    queryFindAllCepByUf.ForEach(x => Console.WriteLine(x.cep + "-" + x.localidade));
                    Console.ReadLine();
                    return;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro ao buscar UF na DB");
                    return;
                }
            } else
            {
                Console.WriteLine("UF Invalida!");
                return;
            }

        }
    }
}
