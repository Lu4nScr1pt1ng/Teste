using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.ConstrainedExecution;
using static CepContext;

namespace TesteProjetoWEB.Endpoints
{
    public static class CepEndpoints
    {
        public static void MapCepEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => "CEP API com Cadastro");


            app.MapGet("/v1/cep", async (GetConnection connectionGetter) =>
            {
                using var con = await connectionGetter();
                return con.GetAll<Cep>().ToList();
            });

            app.MapPost("/v1/cep", async (GetConnection connectionGetter, OnlyCep onlycep) =>
            {
            // fail first
            if (onlycep.Cep == null) return Results.BadRequest(new { message = "Insira um CEP no JSON para ser adicionado(exemplo: 54589210 -> sem traço)" });
            if (onlycep.Cep.Length != 8) return Results.BadRequest(new { message = "Insira um CEP valido para ser buscado(exemplo: 54589210 -> sem traço)" });

            var con = await connectionGetter();
            var all = con.GetAll<Cep>().Where(x => x.cep == $"{onlycep.Cep} ");
            if (all.Count() != 0) return Results.BadRequest(new { message = "Esse CEP já existe no banco de dados" });

            string viaCEPUrl = "https://viacep.com.br/ws/" + onlycep.Cep + "/json/";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            dynamic result;
            try
            {
                using var client = new HttpClient();
                result = await client.GetStringAsync(viaCEPUrl);
                result = JsonConvert.DeserializeObject<dynamic>(result);
            }
            catch (Exception ex)
            {

                return Results.BadRequest(new { message = "Ocorreu um erro em buscar esse cep na API" });

            }

            if (result.erro == true) return Results.BadRequest(new { message = "Cep informado é invalido" });

            var newCep = new Cep(0, onlycep.Cep, $"{result.logradouro}", $"{result.complemento}", $"{result.bairro}", $"{result.localidade}", $"{result.uf}", Int64.Parse("0"), Int32.Parse($"{result.ibge}"), $"{result.gia}");

            var id = con.Insert(newCep);
            return Results.Created($"/v1/cep/{id}", newCep);
            });



            app.MapGet("/v1/cep/{numerocep}", async (GetConnection connectionGetter, string numerocep) =>
            {
                using var con = await connectionGetter();
                var all = con.GetAll<Cep>().Where(x => x.cep == $"{numerocep} ");
                return all;
            });
        }
    }
}
