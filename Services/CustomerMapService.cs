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

        public async Task<bool> UpdateCustomerLocationAsync(string customerNo, decimal latitude, decimal longitude)
        {
            try
            {
                var reading = await dbContext.Database.Table<Reading>()
                    .FirstOrDefaultAsync(r => r.CUSTOMER_NUMBER == customerNo);

                if (reading != null)
                {
                    reading.ReadingNotTaken = false;
                    reading.ReadingTaken = true;
                    reading.ReadingSync = false;
                    reading.Latitude = latitude;
                    reading.Longitude = longitude;

                    await dbContext.Database.UpdateAsync(reading);

                    StatusMessage = "Customer location updated successfully.";
                    return true;
                }
                else
                {
                    StatusMessage = "Customer not found.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "An error occurred while updating the customer location.";
                return false;
            }
        }
    }
}
