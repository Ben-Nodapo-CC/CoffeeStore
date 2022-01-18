using AutoMapper;
using CoffeeStore.Dtos;
using CoffeeStore.Models;
using SharedModels.Dtos;
using SharedModels.Enums;

namespace CoffeeStore.Mapping
{
    public class CoffeesProfile : Profile
    {
        public CoffeesProfile()
        {
            //maps are created from the source to a target
            //since the domain object and the dto have properties with the same name, automapper does not need any additional info.
            CreateMap<Coffee, CoffeeReadDto>();

            CreateMap<CoffeeCreateDto, Coffee>()
                .AfterMap((src, dest) =>
                {
                    dest.Price = dest.Name == CoffeeType.Arabica.ToString() ? 5 : 10;
                });

            CreateMap<CoffeePublishMessageDto, Coffee>()
                .AfterMap((src, dest) =>
                {
                    dest.Price = dest.Name == CoffeeType.Arabica.ToString() ? 5 : 10;
                });
        }
    }
}
