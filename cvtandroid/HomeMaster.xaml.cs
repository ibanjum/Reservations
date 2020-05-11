using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cvtandroid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeMaster : ContentPage
    {
        public ListView ListView;

        public HomeMaster()
        {
            InitializeComponent();

            BindingContext = new HomeMasterViewModel();
            ListView = MenuItemsListView;
        }
        class HomeMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<HomeMenuItem> MenuItems { get; set; }

            public HomeMasterViewModel()
            {
                 MenuItems = new ObservableCollection<HomeMenuItem>(new[]
                {
                    new HomeMenuItem { Id = 0, Title = "Log Out" },
                    new HomeMenuItem { Id = 1, Title = "Terms"}
                }); 
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
