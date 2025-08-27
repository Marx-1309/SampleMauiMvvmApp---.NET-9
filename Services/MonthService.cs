using SampleMauiMvvmApp.Interfaces;

namespace SampleMauiMvvmApp.Services
{
    public partial class MonthService : BaseService, IMonthService
    {
        private HttpClient httpClient;

        public MonthService(DbContext dbContext) : base(dbContext)
        {
            this.httpClient = new HttpClient();
        }

        private List<Month> MonthList;

        public async Task<List<Month>> GetMonths()
        {
            if (MonthList?.Count > 0)
                return MonthList;
            var response = await httpClient.GetAsync(Constants.GetMonth);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    MonthList = await response.Content.ReadFromJsonAsync<List<Month>>();
                }
                catch (Exception ex)
                {
                }
            }
            return MonthList;
        }

        public async Task<string> GetMonthNameById()
        {
            var latestExportMonthItem = await dbContext.Database.Table<ReadingExport>()
                   .OrderByDescending(r => r.WaterReadingExportID)
                   .FirstOrDefaultAsync();

            int currentMonthId = latestExportMonthItem.MonthID;

            var Month = await dbContext.Database.Table<Month>().FirstOrDefaultAsync(m => m.MonthID == currentMonthId);

            if (Month != null)
            {
                return Month.MonthName;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> GetCurrentMonthNameById(int Id)
        {
            if (Id > 0)
            {
                try
                {
                    var month = await dbContext.Database.Table<Month>().Where(m => m.MonthID == Id).FirstOrDefaultAsync();
                    string monthName = month.MonthName;
                    return monthName;
                }
                catch (Exception ex)
                {
                    return StatusMessage = ex.Message;
                }
            }
            return "";
        }

        private List<Reading> ReadingList;

        public async Task<List<Reading>> GetReadingsByMonthIdAsync(int MonthId)
        {
            if (ReadingList?.Count > 0)
            {
                ReadingList.Clear();
            }
            try
            {
                var listOfReadings = await dbContext.Database.Table<Reading>().Where(x => x.MonthID == MonthId).ToListAsync();
                foreach (var item in listOfReadings)
                {
                    ReadingList?.Add(item);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }
            return ReadingList;
        }

        private List<Month> listMonths;

        public async Task<List<Month>> GetListOfMonthsFromSql()
        {
            if (listMonths == null)
            {
                try
                {
                    var monthsCount = await dbContext.Database.Table<Month>().Where(m => m.MonthID >= 1).ToListAsync();

                    if (monthsCount.Count > 0)
                    {
                        var existingIds = monthsCount.Select(m => m.MonthID).ToList();

                        var response = await httpClient.GetAsync(SampleMauiMvvmApp.API_URL_s.Constants.GetMonth);

                        if (response.IsSuccessStatusCode)
                        {
                            var newMonths = await response.Content.ReadFromJsonAsync<List<Month>>();

                            var idsToDelete = existingIds.Except(newMonths.Select(m => m.MonthID)).ToList();

                            await dbContext.Database.Table<Month>().DeleteAsync(m => idsToDelete.Contains(m.MonthID));

                            var newItemsToInsert = newMonths.Where(m => !existingIds.Contains(m.MonthID)).ToList();

                            if (newItemsToInsert.Any())
                            {
                                var response2 = await dbContext.Database.InsertAllAsync(newItemsToInsert);

                                foreach (var item in newItemsToInsert)
                                {
                                    listMonths?.Add(item);
                                }
                            }
                        }
                        else
                        {
                            StatusMessage = $"Failed :." + response.StatusCode;
                        }
                    }
                    else
                    {
                        var response = await httpClient.GetAsync(SampleMauiMvvmApp.API_URL_s.Constants.GetMonth);

                        if (response.IsSuccessStatusCode)
                        {
                            listMonths = await response.Content.ReadFromJsonAsync<List<Month>>();
                            foreach (var item in listMonths)
                            {
                                item.MonthID = item.MonthID;
                                item.MonthName = item.MonthName.Trim();

                                var response2 = await dbContext.Database.InsertAsync(item);
                            }
                        }
                        else
                        {
                            StatusMessage = $"Failed :." + response.StatusCode;
                        }
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error." + ex.Message;
                }
            }
            return listMonths;
        }

        public async Task<List<Month>> GetListOfMonthsFromSqlite()
        {
            try
            {
                // Retrieve the first reading with a non-zero WaterReadingExportID
                var reading = await dbContext.Database.Table<Reading>()
                                                     .FirstOrDefaultAsync(r => r.WaterReadingExportID != 0);

                if (reading == null)
                {
                    StatusMessage = "No reading found.";
                    return null;
                }

                // Find the current month from the reading's MonthID
                var currentMonth = dbContext.Database.Table<Month>()
                                                         .FirstOrDefaultAsync(m => m.MonthID == reading.MonthID).GetAwaiter().GetResult();
                if (currentMonth == null)
                {
                    await GetListOfMonthsFromSql();

                    currentMonth = dbContext.Database.Table<Month>()
                                                            .FirstOrDefaultAsync(m => m.MonthID == reading.MonthID).GetAwaiter().GetResult();

                    if (currentMonth == null)
                    {
                        StatusMessage = "Populated month not found.";
                        return null;
                    }
                }

                int year = DateTime.Now.Year;
                int currentMonthNumber = currentMonth.MonthID; // Assuming MonthID represents a month number (1 = January, 2 = February, etc.)

                // Calculate previous and next month numbers
                int previousMonthNumber = currentMonthNumber - 1;
                int nextMonthNumber = currentMonthNumber + 1;

                // Adjust for year-end wrapping (e.g., December -> January)
                if (previousMonthNumber == 0) previousMonthNumber = 12;
                if (nextMonthNumber == 13) nextMonthNumber = 1;

                // Retrieve the months for previous, current, and next months
                var months = await dbContext.Database.Table<Month>()
                                                     .Where(m => m.MonthID == previousMonthNumber ||
                                                                 m.MonthID == currentMonthNumber ||
                                                                 m.MonthID == nextMonthNumber)
                                                     .ToListAsync();

                // If we have all 3 months (previous, current, and next), return them
                if (months.Count == 3)
                {
                    return months;
                }
                else
                {
                    StatusMessage = "Failed to retrieve the requested months from the database.";
                    return null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return null;
            }
        }

        public async Task<string?> GetLatestExportItemMonthName()
        {
            try
            {
                var lastItem = await dbContext.Database.Table<Month>()
                    .OrderByDescending(r => r.MonthName)
                    .FirstOrDefaultAsync();

                return lastItem?.MonthName;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return null;
            }
        }

        public async Task<int?> GetLatestExportItemMonthId()
        {
            try
            {
                var lastItem = await dbContext.Database.Table<ReadingExport>()
                    .OrderByDescending(r => r.WaterReadingExportID)
                    .FirstOrDefaultAsync();

                return lastItem?.MonthID;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return null;
            }
        }

        public async Task<bool> IsMonthPopulated(Month month)
        {
            try
            {
                int isWithReadings = await dbContext.Database.Table<Reading>().Where(r => r.MonthID == month.MonthID).CountAsync();
                if (isWithReadings > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return false;
            }
        }
    }
}