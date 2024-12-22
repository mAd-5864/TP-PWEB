﻿using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RCLAPI.DTO;
using RCLAPI.Services;
using RCLProdutos.Services.Interfaces;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using Xamarin.Essentials;

namespace RCLProdutos.Shared.Slider;

public partial class SlideComponent
{

    [SupplyParameterFromQuery]
    public int prodSugereId { get; set; }
    
    [Parameter]
    public ProdutoDTO? produto { get; set; } = new ProdutoDTO();

    [Parameter]
    public float? width { get; set; }

    [Parameter]
    public string? marginLeft { get; set; }

    [Inject]
    public IApiServices? _apiServices { get; set; }
    public int countSlide { get; set; } = 0;

    public ProdutoFavorito? produtoFavorito { get; set; } = new ProdutoFavorito();
    private string? uidprod { get; set; }
    private string? favoritoicon { get; set; }
    private string? pathurlimg { get; set; }

    protected override async Task OnInitializedAsync()
    {
        modalDisplay2 = "none";
        modalDisplay1 = "none";
       
        mostraInfo = "none";
        fazcompra = "none";
        if (!produto.Favorito)
            favoritoicon = $"images/heart.png";
        else
        {
            favoritoicon = $"images/heartfilltransp.png";
        }
    }

    private string mostraInfo;
    private string fazcompra;

    private string acao;
    private string acaocompra;
    private void Info()
    {
        acao = mostraInfo;

        if (acao == "none")
            mostraInfo = "block";
        else mostraInfo = "none";
    }

    private void Comprar()
    {
        acaocompra = fazcompra;

        if (acaocompra == "none")
            fazcompra = "block";
        else fazcompra = "dispose";
    }

    //*************** MODAIS ************

    private string modalDisplay1 = "none;";
    private string modalDisplay2 = "none;";

    private string modalClass = string.Empty;

    private int quantidade = 0;
    private decimal total = 0;
    public string limiteQtd = "";

    private bool abreModal1 = false;
    private bool abreModal2 = false;
    
    public async void AbreFecha(string janela1, string janela2)
    {
        if (janela1 == "abre")
        {
            modalDisplay1 = "block";
            abreModal1 = true;
        }
        else if (janela1 == "fecha")
        {
            modalDisplay1 = "none";
            abreModal1 = false;
        }
        else if (janela1 == "grava")
        {
            modalDisplay2 = "none";
            abreModal2 = false;

            var carrinhoCompra = new ItemCarrinhoCompra()
            {
                Quantidade = quantidade,
                PrecoUnitario = produto.Preco,
                ValorTotal = total,
                ProdutoId = produto.Id,
                UserId = "user"
            };

            var response = await _apiServices.AdicionaItemNoCarrinho(carrinhoCompra);

        }

        if (janela2 == "abre")
        {
            modalDisplay2 = "block";
            abreModal2 = true;
        }
        else if (janela2 == "fecha")
        {
            modalDisplay2 = "none";
            abreModal2 = false;
            quantidade = 0;
        }

    }

    public async void Favoritos(string acao, int pId)
    {
        if (favoritoicon == $"images/heart.png")
        {
            favoritoicon = $"images/heartfilltransp.png";
            var altFavorito = await _apiServices.ActualizaFavorito("heartfill", pId);
        }
        else
        {
            favoritoicon = $"images/heart.png";
            var altFavorito = await _apiServices.ActualizaFavorito("heartsimples", pId);
        }
    }

    public void Incrementa(string incredec, string janela2)
    {
        if (incredec == "incrementa")
        {
            quantidade++;
            if(quantidade > produto.EmStock)
            {
                quantidade--;

                limiteQtd = "Lamentamos mas não existe mais stock!";
            }
            
        }

        else if (incredec == "desincrementa" && quantidade > 0)
        {
            quantidade--;
            limiteQtd = "";
        }

        total = quantidade * produto.Preco;
    }
}
