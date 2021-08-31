using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityApp.ViewModels
{
    public class PageViewModel
    {
        public int CurrentPageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage { get { return CurrentPageNumber > 1; } }
        public bool HasNextPage { get { return CurrentPageNumber < TotalPages; } }

        public PageViewModel(int currentPageNumber, int totalUsers, int pageSize)
        {
            CurrentPageNumber = currentPageNumber;
            TotalPages = (int)(Math.Ceiling(totalUsers / (double)pageSize));
        }
    }
}
