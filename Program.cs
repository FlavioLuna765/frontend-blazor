using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Fcg.Web;
using Fcg.Web.Services.Auth;
using Fcg.Web.Services.Catalogo;
using Fcg.Web.Services.Checkout;
using Fcg.Web.Services.Carrinho;
using Fcg.Web.Services.Usuario;
using Fcg.Web.Services.Biblioteca;
using Fcg.Web.Services.Perfil;
using Fcg.Web.Services.Avaliacoes;
using Fcg.Web.Services.AdminJogos;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ✅ Lê as URLs do appsettings.json (sem hardcode)
var authBaseUrl = builder.Configuration["Api:AuthBaseUrl"]
    ?? throw new InvalidOperationException("Api:AuthBaseUrl não configurado no appsettings.json");

var catalogBaseUrl = builder.Configuration["Api:CatalogBaseUrl"]
    ?? throw new InvalidOperationException("Api:CatalogBaseUrl não configurado no appsettings.json");

// Cliente HTTP padrão: API de autenticação (users-api)
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(authBaseUrl)
    });

// Autenticação e sessão
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthSession, AuthSession>();
builder.Services.AddScoped<AuthStorage>();
builder.Services.AddScoped<AuthSessionInitializer>();

// Carrinho e usuário
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();
builder.Services.AddScoped<ICadastroUsuarioService, CadastroUsuarioService>();
builder.Services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

// Handler JWT
builder.Services.AddScoped<JwtAuthorizationHandler>();

// Perfil (users-api)
builder.Services.AddScoped<IPerfilService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(authBaseUrl)
    };

    return new PerfilService(httpClient);
});

// Catálogo (catalog-api)
builder.Services.AddScoped<ICatalogoService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(catalogBaseUrl)
    };

    return new CatalogoService(httpClient);
});

// Avaliações (catalog-api)
builder.Services.AddScoped<IAvaliacaoService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(catalogBaseUrl)
    };

    return new AvaliacaoService(httpClient);
});

// Checkout (catalog-api)
builder.Services.AddScoped<ICheckoutService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(catalogBaseUrl)
    };

    return new CheckoutService(httpClient);
});

// Biblioteca (catalog-api)
builder.Services.AddScoped<IBibliotecaService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(catalogBaseUrl)
    };

    return new BibliotecaService(httpClient);
});

// Administração de jogos (catalog-api)
builder.Services.AddScoped<IAdminJogosService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri(catalogBaseUrl)
    };

    return new AdminJogosService(httpClient);
});

var host = builder.Build();

// Inicialização da sessão de autenticação
var authSessionInitializer =
    host.Services.GetRequiredService<AuthSessionInitializer>();

await authSessionInitializer.InitializeAsync();

await host.RunAsync();