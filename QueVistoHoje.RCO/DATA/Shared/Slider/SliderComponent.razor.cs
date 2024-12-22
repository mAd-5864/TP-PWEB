using Microsoft.AspNetCore.Components;
using RCLAPI.DTO;
using RCLAPI.Services;
using RCLProdutos.Services.Interfaces;

namespace RCLProdutos.Shared.Slider
{
    public partial class SliderComponent
    {
        [SupplyParameterFromQuery]
        public string nomeCat { get; set; }

        [SupplyParameterFromQuery]
        public int Id { get; set; }

        [SupplyParameterFromQuery]
        private int compraSugerida { get; set; }

        [Parameter]
        public int? initProd { get; set; }

        [Inject]
        public IApiServices? _apiServices { get; set; }
       
        [Inject]
        public ISliderUtilsServices sliderUtilsService { get; set; }
        private List<ProdutoDTO>? produtos { get; set; }
        private List<ProdutoFavorito>? userFavoritos { get; set; }

        public ProdutoDTO sugestaoProduto = new ProdutoDTO();
        private int witdthPerc { get; set; } = 0;
        private bool IsDisabledNext { get; set; } = false;
        private bool IsDisbledPrevious { get; set; } = false;

        public static int? actualProd=0;
        protected override async Task OnInitializedAsync()
        {
            int? categoriasenviadaID;
            string? produtosEspecificos;

            if (Id == 0 && actualProd == 0 || nomeCat == "Todos")
            {
                produtosEspecificos = "todos";
                categoriasenviadaID = null;
            }
            else if (actualProd == Id)
            {
                categoriasenviadaID = Id;
                produtosEspecificos = "categoria";
            }
            else 
            {
                if (Id > 0)
                {
                    categoriasenviadaID = Id;
                    actualProd = Id;
                    produtosEspecificos = "categoria";
                }

                else
                {
                    categoriasenviadaID = actualProd;
                    produtosEspecificos = "categoria";
                }

            }

            try
            {
                produtos = await _apiServices!.GetProdutosEspecificos(produtosEspecificos, categoriasenviadaID);

                userFavoritos = await _apiServices!.GetFavoritos("Jorge");

                for (int i = 0; i < userFavoritos.Count; i++)
                    for (int j = 0; j < produtos.Count; j++)
                        if (produtos[j].Id == userFavoritos[i].ProdutoId)
                            produtos[j].Favorito = userFavoritos[i].Efavorito;

                Random random = new Random();

                int[]? indices = produtos
                                       .Where(item => item is not null)
                                       .Select(item => item.Id)
                                       .ToArray();

                int sugestaoProdutoId = random.Next(0, produtos.Count - 1);

                sugestaoProduto = produtos[indices[sugestaoProdutoId] - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

                await LoadMarginsLeft();

                int qtdProd = produtos.Count;

                witdthPerc = qtdProd * 100;

                sliderUtilsService.WidthSlide2 = 100f / qtdProd;

                sliderUtilsService.OnChange += StateHasChanged;
        }

        async Task LoadMarginsLeft()
        {
            foreach (var produto in produtos)
            {
                sliderUtilsService.MarginLeftSlide.Add("margin-left:0%");
            }
        }

        void PreviousSlide()
        {
            if (sliderUtilsService.CountSlide != 0)
            {
                sliderUtilsService.MarginLeftSlide[sliderUtilsService.CountSlide - 1] = "margin-left:0%";
                sliderUtilsService.CountSlide--;
                IsDisabledNext = false;
                IsDisbledPrevious = false;
            }
            else
            {
                sliderUtilsService.MarginLeftSlide[0] = "margin-lef:0%";
                IsDisbledPrevious = true;
            }
            sliderUtilsService.Index = sliderUtilsService.CountSlide;
        }

        void NextSlide()
        {
            sliderUtilsService.CountSlide++;
            sliderUtilsService.Index = sliderUtilsService.CountSlide;
            if (sliderUtilsService.CountSlide < sliderUtilsService.MarginLeftSlide.Count)
            {
                string WidthSlideS = (Convert.ToString(sliderUtilsService.WidthSlide2));

                WidthSlideS = WidthSlideS.Replace(",", ".");

                //sliderUtilsService.MarginLeftSlide[sliderUtilsService.CountSlide - 1] = $"margin-left:-{WidthSlide}%";

                sliderUtilsService.MarginLeftSlide[sliderUtilsService.CountSlide - 1] = $"margin-left:-{sliderUtilsService.WidthSlide2}%";


                sliderUtilsService.MarginLeftSlide[sliderUtilsService.CountSlide - 1] = $"margin-left:-12.5%";

                IsDisabledNext = false;
                IsDisbledPrevious = false;
            }
            else
            {
                IsDisabledNext = true;
            }
        }
    }
}
