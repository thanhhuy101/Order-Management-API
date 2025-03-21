using AutoMapper;
using Order_Management.DTOs;
using Order_Management.Entities;

namespace Order_Management.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Order mappings
            CreateMap<Order, OrderDTO>();
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());

            CreateMap<UpdateOrderDTO, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());

            // OrderDetail mappings
            CreateMap<OrderDetail, OrderDetailDTO>();
            CreateMap<CreateOrderDetailDTO, OrderDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Order, opt => opt.Ignore());
        }
    }
}
