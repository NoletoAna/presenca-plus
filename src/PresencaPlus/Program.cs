using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PresencaPlus;

var culturaPtBr = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culturaPtBr;
CultureInfo.DefaultThreadCurrentUICulture = culturaPtBr;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Padrao")));
builder.Services.AddScoped<FrequenciaService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Login";
        o.AccessDeniedPath = "/AcessoNegado";
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Coordenacao", p => p.RequireRole("Coordenacao"));
    o.AddPolicy("Professor", p => p.RequireRole("Professor"));
    // RNF01: toda página exige autenticação, salvo as marcadas com [AllowAnonymous].
    o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

builder.Services.AddRazorPages(o =>
{
    o.Conventions.AuthorizeFolder("/Turmas", "Coordenacao");
    o.Conventions.AuthorizeFolder("/Alunos", "Coordenacao");
    o.Conventions.AuthorizeFolder("/Aulas", "Coordenacao");
    o.Conventions.AuthorizeFolder("/Chamada", "Professor");
    o.Conventions.AuthorizeFolder("/Relatorio");
})
.AddMvcOptions(o =>
{
    var m = o.ModelBindingMessageProvider;
    m.SetValueMustNotBeNullAccessor(_ => "Preencha este campo.");
    m.SetAttemptedValueIsInvalidAccessor((_, _) => "O valor informado é inválido.");
    m.SetMissingBindRequiredValueAccessor(_ => "Preencha este campo.");
    m.SetMissingKeyOrValueAccessor(() => "Preencha este campo.");
    m.SetUnknownValueIsInvalidAccessor(_ => "O valor informado é inválido.");
    m.SetValueIsInvalidAccessor(_ => "O valor informado é inválido.");
    m.SetValueMustBeANumberAccessor(_ => "Informe um número.");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    Seed.Executar(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRequestLocalization(o =>
{
    o.DefaultRequestCulture = new("pt-BR");
    o.SupportedCultures = [culturaPtBr];
    o.SupportedUICultures = [culturaPtBr];
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();
app.MapGet("/", (HttpContext ctx) =>
    Results.Redirect(ctx.User.IsInRole("Professor") ? "/Chamada" : "/Relatorio"));
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
