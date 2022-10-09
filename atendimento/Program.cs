using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;



public class Solicitacao
{
    public int SolicitacaoId { get; set; }
    public String? Prioridade { get; set; }
    public string? Setor { get; set; }

    public String? MotivoSolicitacao { get; set; }
    public int CodigoDeAcessoMaquina { get; set; } //para o suporte acessa ex:anydesk

    public ICollection<Suporte>? Suportes { get; set; }
}
public class Suporte
{
    public int SuporteId { get; set; }
    public string? Nome { get; set; }
    public string? Nivel { get; set; }
    public bool Ativo { get; set; }
    public ICollection<Solicitacao>? Solicitacoes { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Solicitacao>? Solicitacoes { get; set; }
    public DbSet<Suporte>? Suportes { get; set; }

    //Reforso do mapeamento
    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Suporte>().HasKey(s => s.SuporteId);
        mb.Entity<Solicitacao>().HasKey(s => s.SolicitacaoId);
    }


}
class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

         //builder.Services.AddDbContext<AppDbContext>(options =>
          //  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        var app = builder.Build();

    //endpoints Suporte
    //adicionado sup
    app.MapPost("/suportes", async (Suportec suporte,AppDbContext db ) => {
        db.Suportes.Add(suporte);
        await db.SaveChangesAsync();

        return Results.Created($"/suportes/{suporte.SuporteId}",suporte);
    });

    //retorna lista de suporte
    app.MapGet("/suportes", async (AppDbcontext db) => await db.Suportes.ToListAsync());

    //Para Obter um suporte pelo seu id 
    app.MapGet("/suportes/{id:int}",async (int id, AppDbContext db) => 
    {
        return await db.Suportes.FindAsync(id)
        is Suporte suporte ? Results.Ok(suporte) : Result.NotFound();
    });

        //Atualizar um Suporte pelo ID
        app.MapPut("/suportes/{id:int}",async (int id,Suporte suporte, AppDbContext db) =>
        {
            if(suporte.SuporteId != id){
                return Result.BadRequst();
            }
            var suporteDB = await db.Suportes.FindAsync(id);
            if (suporteDB is null) return Result.NotFound();
            
            suporteDB.Nome = suporte.Nome;
            suporteDB.Nivel = suporte.Nivel;
            suporteDB.Ativo = suporte.Ativo;

            await db.SaveChangesAsync();
            return Results.Ok(suporteDB);
        });

        //Deletar pelo ID
         app.MapGet("/suportes/{id:int}",async (int id, AppDbContext db) => 
        {  
            var suporte = await db.Suportes.FindAsync(id);
            if(categoria is null) return Result.NotFound();

            db.Suportes.remove(suporte);
            await db.SaveChangesAsync();
            return Results.NoCOntent();
        });

        app.Run();
    }
}