using GerenciaVendas.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Adiciona e configura servi�os de MVC
builder.Services.AddControllersWithViews();

// Configura��o de autentica��o baseada em Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Login"; // Substitua pelo seu caminho de login, se diferente
                    options.AccessDeniedPath = "/Usuario/AccessDenied"; // Substitua pelo seu caminho de acesso negado, se necess�rio
                });

// Inje��o de depend�ncia para seus servi�os
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<ItemVendaService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<HashService>();
builder.Services.AddScoped<AdministradorService>();

// Adicione outras inje��es de depend�ncia conforme necess�rio

var app = builder.Build();

// Configurar o pipeline de requisi��es HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Page}/{id?}");
});

app.Run();
