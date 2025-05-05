using AutoMapper;
using CashFlow.Application.AutoMapper;

namespace CommonTesteUtilities.Mapper
{
    public class MapperBuilder
    {
        public static IMapper Build()
        {
            var mapper = new MapperConfiguration(config => 
            {
                config.AddProfile(new AutoMapping());
            });

            return mapper.CreateMapper();
        }
    }
}
