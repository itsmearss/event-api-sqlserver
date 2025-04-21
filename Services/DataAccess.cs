using TestProjectAnnur.Data;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Services
{
    public class DataAccess
    {
        private readonly ApplicationDbContext _context;

        public DataAccess(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Z_Production_Schedule> GetStitchingData(string factoryId, DateOnly startDate, DateOnly endDate)
        {
            return _context.Z_Production_Schedule
                .Where(s => s.Factory_ID == factoryId &&
                            s.ST_Start_Date >= startDate &&
                            s.ST_Start_Date <= endDate)
                .OrderBy(s => s.ST_Team)
                .ThenBy(s => s.ST_Start_Date)
                .ThenByDescending(s => s.UA_sequence)
                .ToList();
        }

        public List<Z_Production_Schedule> GetAssemblyData(string factoryId, DateOnly startDate, DateOnly endDate)
        {
            return _context.Z_Production_Schedule
                .Where(a => a.Factory_ID == factoryId &&
                            a.AS_Start_Date >= startDate &&
                            a.AS_Start_Date <= endDate)
                .OrderBy(a => a.AS_Team)
                .ThenBy(a => a.AS_Start_Date)
                .ThenByDescending(a => a.FA_sequence)
                .ToList();
        }

        public List<A_T_NonWorking_Date> GetNonWorkingDates(string factoryId)
        {
            return _context.A_T_NonWorking_Date
                .Where(n => n.Factory_ID == factoryId)
                .ToList();
        }


    }
}
