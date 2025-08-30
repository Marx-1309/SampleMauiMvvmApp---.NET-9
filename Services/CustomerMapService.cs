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
        public async Task<List<ReadingDto>> GetCustomersWithCoordinatesAsync(string readingStatus)
        {
            try
            {
                List<Reading> response = new();

                if (readingStatus == "Captured")
                {
                    response = await dbContext.Database.Table<Reading>()
                        .Where(r => r.ReadingTaken)
                        .ToListAsync();
                }
                else if (readingStatus == "Uncaptured")
                {
                    response = await dbContext.Database.Table<Reading>()
                        .Where(r => !r.ReadingTaken)
                        .ToListAsync();
                }
                else if (readingStatus == "All Readings")
                {
                    response = await dbContext.Database.Table<Reading>()
                        .ToListAsync();
                }
                else
                {
                    StatusMessage = "Invalid reading status.";
                    return new List<ReadingDto>();
                }

                if (response.Any())
                {
                    return _mapper.Map<List<ReadingDto>>(response);
                }
                else
                {
                    StatusMessage = "No customers with coordinates found.";
                    return new List<ReadingDto>();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
                return new List<ReadingDto>();
            }
        }
    }
}
