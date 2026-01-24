using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.TourPurchaseTokens;

namespace Explorer.Payments.Core.Mappers;

public class PaymentsProfile : Profile
{
    public PaymentsProfile()
    {
        CreateMap<TourPurchaseTokenDto, TourPurchaseToken>().ReverseMap();
        CreateMap<WalletDto,Wallet>().ReverseMap();
        CreateMap<BundleDto, Explorer.Payments.Core.Domain.Bundles.Bundle>().ReverseMap();
    }
}