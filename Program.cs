
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

// Cliente HTTP padrão: API de autenticação.
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("http://localhost:8081/")
    });

// Autenticação e sessão.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthSession, AuthSession>();
builder.Services.AddScoped<AuthStorage>();
builder.Services.AddScoped<AuthSessionInitializer>();

// Carrinho e usuário.
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();
builder.Services.AddScoped<ICadastroUsuarioService, CadastroUsuarioService>();
builder.Services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

// Handler JWT.
builder.Services.AddScoped<JwtAuthorizationHandler>();


// Cliente compartilhado da Catalog API.


// Perfil.
builder.Services.AddScoped<IPerfilService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:8081/")
    };

    return new PerfilService(httpClient);
});

// Catálogo.
builder.Services.AddScoped<ICatalogoService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:8082/")
    };

    return new CatalogoService(httpClient);
});

// Avaliações.
builder.Services.AddScoped<IAvaliacaoService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:8082/")
    };

    return new AvaliacaoService(httpClient);
});

// Checkout: utiliza exclusivamente o Catálogo (8082).
builder.Services.AddScoped<ICheckoutService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:8082/")
    };

    return new CheckoutService(httpClient);
});

// Biblioteca.
builder.Services.AddScoped<IBibliotecaService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:8082/")
    };

    return new BibliotecaService(httpClient);
});

// Administração de jogos.
builder.Services.AddScoped<IAdminJogosService>(sp =>
{
    var authSession = sp.GetRequiredService<IAuthSession>();

    var handler = new JwtAuthorizationHandler(authSession)
    {
        InnerHandler = new HttpClientHandler()
    };

    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:8082/")
    };

    return new AdminJogosService(httpClient);
});

var host = builder.Build();

// Inicialização da sessão de autenticação.
var authSessionInitializer =
    host.Services.GetRequiredService<AuthSessionInitializer>();

await authSessionInitializer.InitializeAsync();

await host.RunAsync();
