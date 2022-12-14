using System.ComponentModel.DataAnnotations.Schema;

[Table("CEP")]
public record Cep(System.Int32 Id, System.String cep, System.String logradouro, System.String complemento, System.String bairro, System.String localidade, System.String uf, System.Int64 unidade, System.Int32 ibge, System.String gia);


public class OnlyCep
{
    public OnlyCep(string cep)
    {
        Cep = cep;
    }

    public string Cep { get; set; }
}