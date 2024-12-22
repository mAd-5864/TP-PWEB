using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using RCLAPI.DTO;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Entity.Core.Objects;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.NetworkInformation;

namespace RCLAPI.Services;
public class ApiService : IApiServices
{
    private readonly ILogger<ApiService> _logger;
    private readonly HttpClient _httpClient = new();

    private readonly IHttpContextAccessor _httpContextAccessor;

    JsonSerializerOptions _serializerOptions;

    private List<ProdutoDTO> produtos;

    private List<Categoria> categorias;

    private ProdutoDTO _detalhesProduto;
        public ApiService(ILogger<ApiService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _detalhesProduto = new ProdutoDTO();
        categorias = new List<Categoria>();
    }
    //private void AddAuthorizationHeader()
    //{
    //    if (!string.IsNullOrEmpty(token))
    //    {
    //        _httpClient.DefaultRequestHeaders.Authorization =
    //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    //    }
    //}

    // ********************* Categorias  **********
    public async Task<List<Categoria>> GetCategorias()
    {
        string endpoint = $"api/Categorias";

        try
        {
            HttpResponseMessage httpResponseMessage = 
                await _httpClient.GetAsync($"{AppConfig.BaseUrl}{endpoint}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string content = "";

                content = await httpResponseMessage.Content.ReadAsStringAsync();
                categorias = JsonSerializer.Deserialize<List<Categoria>>(content, _serializerOptions)!;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

        return categorias;
    }

    // ********************* Produtos  **********
    public async Task<List<ProdutoDTO>> GetProdutosEspecificos(string produtoTipo,int? IdCategoria)
    {
        string endpoint = "";

        if (produtoTipo == "categoria" && IdCategoria != null)
        {
            endpoint = $"api/Produtos?tipoProduto=categoria&categoriaId={IdCategoria}";
        }
        else if (produtoTipo == "todos")
        {
            endpoint = $"api/Produtos?tipoProduto=todos";
        }
        else
        {
            return null;
        }
        try
        {
            HttpResponseMessage httpResponseMessage = 
                await _httpClient.GetAsync($"{AppConfig.BaseUrl}{endpoint}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string content = "";
                content = await httpResponseMessage.Content.ReadAsStringAsync();
                produtos = JsonSerializer.Deserialize<List<ProdutoDTO>>(content, _serializerOptions)!;           
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
        return produtos;
    }
    public async Task<(T?Data, string?ErrorMessage)>GetAsync<T>(string endpoint)
    {
        try
        {
            //AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(AppConfig.BaseUrl + endpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<T>(responseString, _serializerOptions);
                return (data ?? Activator.CreateInstance<T>(), null);
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    string errorMessage = "Unauthorized";
                    _logger.LogWarning(errorMessage);
                    return (default, errorMessage);
                }
                string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                _logger.LogError(generalErrorMessage);
                return (default, generalErrorMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            string errrMessage = $"Erro de requisição HTTP: {ex.Message}";
            _logger.LogError(errrMessage);
            return (default, errrMessage);
        }
        catch (JsonException ex)
        {
            string errorMessage = $"Erro de desserialização JSON: {ex.Message}";
            _logger.LogError(ex.Message);
            return (default, errorMessage);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Erro inesperado: {ex.Message}";
            _logger.LogError(ex.Message);
            return (default, errorMessage);
        }
    }

    // ***************** Compras ******************
    public async Task<ApiResponse<bool>> AdicionaItemNoCarrinho(ItemCarrinhoCompra carrinhoCompra)
    {
        try
        {
            var json = JsonSerializer.Serialize(carrinhoCompra, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PostRequest("api/ItensCarrinhoCompra", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erro ao enviar requisição HTTP: {response.StatusCode}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                };
            }
            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao adicionar item no carrinho de compras: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }

    // ****************** Utilizadores ********************
    public async Task<ApiResponse<bool>> RegistarUtilizador(Utilizador novoUtilizador)
    {
        try
        {
            var json = JsonSerializer.Serialize(novoUtilizador, _serializerOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PostRequest("api/Utilizadores/RegistarUser", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erro ao enviar requisitos Http: {response.StatusCode}");
                return new ApiResponse<bool>
                {
                    ErrorMessage = $"Erro ao enviar requisição HTTP: {response.StatusCode}"
                };
            }

            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao registar o utiizador: {ex.Message}");
            return new ApiResponse<bool> { ErrorMessage = ex.Message };
        }
    }

    public async Task<ApiResponse<bool>> Login(LoginModel login)
    {
        try
        {
            string endpoint = "api/Utilizadores/LoginUser";

            var json = JsonSerializer.Serialize(login, _serializerOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PostRequest($"{AppConfig.BaseUrl}{endpoint}", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Erro ao enviar requisição Http: {response.StatusCode}");

            }
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Token>(jsonResult, _serializerOptions);
     
            return new ApiResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            string errorMessage = $"Erro inesperado: {ex.Message}";
            _logger.LogError(ex.Message);
            return (default);
        }

    }
    private async Task<HttpResponseMessage> PostRequest(string enderecoURL, HttpContent content)
    {
        try
        {
            var result = await _httpClient.PostAsync(enderecoURL, content);
            return result;
        }
        catch (Exception ex)
        {
            // Log o erro ou trata conforme necessario
            _logger.LogError($"Erro ao enviar requisição POST para enderecoURL: {ex.Message}");
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    // *************** Gerir Favoritos ******************

    public async Task<List<ProdutoFavorito>> GetFavoritos(string utilizadorId)
    {
        string endpoint = $"api/Favoritos/{utilizadorId}";

       // AddAuthorizationHeader();
        HttpResponseMessage response = await _httpClient.GetAsync($"{AppConfig.BaseUrl}{endpoint}");

        var responseString = await response.Content.ReadAsStringAsync();
        List<ProdutoFavorito> data = JsonSerializer.Deserialize<List<ProdutoFavorito>>(responseString, _serializerOptions);

        return data;

    }
    public async Task<(bool Data, string? ErrorMessage)> ActualizaFavorito(string acao, int produtoId)
    {
        try
        {
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var response = await FavoritosPutRequest($"api/Favoritos/{produtoId}/{acao}", content );

            if (!response.IsSuccessStatusCode)
            {
                return (true, null);
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    string errorMessage = "Unauthorized";
                    _logger.LogWarning(errorMessage);
                    return (false, errorMessage);
                }
                string generalErrorMessage = $"Erro na requisição: {response.ReasonPhrase}";
                _logger.LogError(generalErrorMessage);
                return (false, generalErrorMessage);
            }
        }
        catch (HttpRequestException ex)
        {
            string errorMessage = $"Erro de requisição HTTP: {ex.Message}";
            _logger.LogError(errorMessage);
            return (false, errorMessage);
        }
        catch (Exception ex)
        {
            string errorMessage = $"Erro inesperado: {ex.Message}";
            _logger.LogError(errorMessage);
            return (false, errorMessage);
        }
    }

    private async Task<HttpResponseMessage> FavoritosPutRequest(string uri, HttpContent content)
    {
        var enderecoUrl = AppConfig.BaseUrl + uri;
        try
        {
           // AddAuthorizationHeader();
            var result = await _httpClient.PutAsync(enderecoUrl, content);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao enviar requisição PUT para {uri}: {ex.Message}");
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }
}
