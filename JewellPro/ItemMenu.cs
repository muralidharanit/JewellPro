using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using static JewellPro.EnumInfo;

namespace JewellPro
{
    public class ItemMenu
    {
        public AppMenus Id { get; set; }
        public string Header { get; set; }
        public PackIconKind Icon { get; set; }
        public ObservableCollection<SubItem> Members { get; set; }

        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }

        public ItemMenu()
        {
            this.Members = new ObservableCollection<SubItem>();
        }
    }

    public class SubItem
    {
        public AppMenus Id { get; set; }
        public string Header { get; set; }
        public PackIconKind Icon { get; set; }
    }    
}
