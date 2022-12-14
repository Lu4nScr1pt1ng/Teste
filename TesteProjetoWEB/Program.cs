using TesteProjetoWEB.Endpoints;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5173").AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddPersistence();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.MapCepEndpoints();

app.UseSwagger();
app.UseSwaggerUI(c => { });

app.Run();
