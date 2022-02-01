using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlashLight : ContentPage
    {
        bool flashOn = false;
        public FlashLight()
        {
            InitializeComponent();
        }

        private async void btnFlashLightSwitch_Clicked(object sender, EventArgs e)
        {
            if (!flashOn)
            {
                await Flashlight.TurnOnAsync();
                flashOn = true;
            }
            else
            {
                await Flashlight.TurnOffAsync();
                flashOn = false;
            }
        }
    }
}