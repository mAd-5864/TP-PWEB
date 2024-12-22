
namespace RCLAPI.DTO;
public class CarOrder
{
    public decimal PrecoUnitario {  get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public int ProdutoId { get; set; }
    public int ClienteID { get; set; }

}
