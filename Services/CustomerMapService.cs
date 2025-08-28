using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMauiMvvmApp.Services
{
    public partial class CustomerMapService : BaseService
    {
        protected DbContext _dbContext;
        private readonly IMapper _mapper;
        public CustomerMapService(DbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<List<ReadingDto>> GetCustomersWithCoordinatesAsync()
        {
            try
            {
                var response = await dbContext.Database.Table<Reading>().ToListAsync();

                if (response.Count > 0)
                {
                    var mappedResult = _mapper.Map<List<ReadingDto>>(response);
                    return mappedResult;
                }
                else
                {
                    StatusMessage = "No customers with coordinates found.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }
            return new List<ReadingDto>();
        }
    }
}
