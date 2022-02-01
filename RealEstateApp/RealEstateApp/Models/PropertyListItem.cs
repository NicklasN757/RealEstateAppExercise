using PropertyChanged;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RealEstateApp.Models
{
    [AddINotifyPropertyChangedInterface]
    public class PropertyListItem
    {
        public PropertyListItem(Property property)
        {
            Property = property;
        }
        public Property Property { get; set; }
        public double Distance { get; set; }
    }
}
