using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class SortViewModel
    {
        public SortState NameSort { get; private set; }
        public SortState EmailSort { get; private set; }
        public SortState YearSort { get; private set; }
        public SortState CountrySort { get; private set; }
        public SortState CurrentSort { get; private set; }

        public SortViewModel(SortState sortOrder)
        {
            NameSort = sortOrder == SortState.NameAscending ? SortState.NameDescending : SortState.NameAscending;
            EmailSort = sortOrder == SortState.EmailAscending ? SortState.EmailDescending : SortState.EmailAscending;
            YearSort = sortOrder == SortState.YearAscending ? SortState.YearDescending : SortState.YearAscending;
            CountrySort = sortOrder == SortState.CountryAscending ? SortState.CountryDescending
                : SortState.CountryAscending;
            CurrentSort = sortOrder;
        }
    }
}
