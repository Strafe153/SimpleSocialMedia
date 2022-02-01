using System.Collections.Generic;
using SimpleSocialMedia.Models;

namespace SimpleSocialMedia.ViewModels
{
    public class FilterSortPageViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
