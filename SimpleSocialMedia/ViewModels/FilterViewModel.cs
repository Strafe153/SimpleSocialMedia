namespace SimpleSocialMedia.ViewModels
{
    public class FilterViewModel
    {
        public string SelectedUserName { get; set; }
        public string SelectedEmail { get; set; }
        public int? SelectedYear { get; set; }
        public string SelectedCountry { get; set; }

        public FilterViewModel(string name, string email, int? year, string country)
        {
            SelectedUserName = name;
            SelectedEmail = email;
            SelectedYear = year;
            SelectedCountry = country;
        }
    }
}
