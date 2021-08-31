using System;
using System.Collections.Generic;
using IdentityApp.Models;

namespace IdentityApp.ViewModels
{
    public class FilterSortPageViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public FilterViewModel FilterViewModel { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
