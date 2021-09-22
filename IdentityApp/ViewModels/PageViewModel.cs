using System;

namespace IdentityApp.ViewModels
{
    public class PageViewModel
    {
        public int CurrentPageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage { get { return CurrentPageNumber > 1; } }
        public bool HasNextPage { get { return CurrentPageNumber < TotalPages; } }

        public PageViewModel(int currentPageNumber, int totalItems, 
            int pageSize)
        {
            CurrentPageNumber = currentPageNumber;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }
    }
}
