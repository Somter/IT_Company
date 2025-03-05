using IT_Company.Models;

namespace IT_Company.ViewModels
{
    public class PositionViewModel : ViewModelBase
    {
        private Position p;
        public PositionViewModel(Position pos) { p = pos; }
        public int Id => p.Id;

        public string Title
        {
            get => p.Title;
            set
            {
                p.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
    }
}