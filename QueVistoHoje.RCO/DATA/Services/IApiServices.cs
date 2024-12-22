using RCLAPI.DTO;

namespace RCLAPI.Services;

public interface IApiServices
{
    public Task<List<ProdutoDTO>> GetProdutosEspecificos(string produtoTipo, int? IdCategoria);
    public Task<(T? Data, string? ErrorMessage)> GetAsync<T>(string endpoint);
    public Task<List<Categoria>> GetCategorias();
    public Task<(bool Data, string? ErrorMessage)> ActualizaFavorito(string acao,int produtoId);
    public Task<List<ProdutoFavorito>> GetFavoritos(string utilizadorId);
    public Task<ApiResponse<bool>> RegistarUtilizador(Utilizador novoUtilizador);
    public Task<ApiResponse<bool>> Login(LoginModel login);

    public Task<ApiResponse<bool>> AdicionaItemNoCarrinho(ItemCarrinhoCompra carrinhoCompra);
}
