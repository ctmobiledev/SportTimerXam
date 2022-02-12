using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

[assembly: ExportFont("4x7Matrix.ttf", Alias = "Numbers")]                      // Custom font support
[assembly: ExportFont("OpenSans-Regular.ttf", Alias = "OpenSansRegular")]
[assembly: ExportFont("WorkSans-Regular.ttf", Alias = "WorkSansRegular")]

namespace SportTimerXam
{

    public partial class MainPage : ContentPage
    {

        private UIOutputVariables uiOutputVariables = new UIOutputVariables();             // define the container with the variables

        private System.Timers.Timer timer = new System.Timers.Timer();


        // Internal variables not showing on UI (unless converted)

        private int seconds = 0;


        public MainPage()
        {

            InitializeComponent();

            this.BindingContext = uiOutputVariables;                                       // Tells XML tags like Text="{Binding seconds}" which property to use

            AdjustForPlatform();

            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;

        }

        private void AdjustForPlatform()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    lblClock.Padding = new Thickness(0, 8, 0, 0);
                    lblClock.FontSize = 90;
                    break;
                case Device.Android:
                    lblClock.Padding = new Thickness(0, 8, 0, 0);
                    lblClock.FontSize = 90;
                    break;
                case Device.UWP:
                    lblClock.Padding = new Thickness(0, 20, 0, 0);
                    break;
            }
        }

        public class UIOutputVariables : INotifyPropertyChanged                            // KEY PIECE 1: THIS INTERFACE MUST BE DECLARED (probably in a ViewModel typically)
        {

            //=========================================================================
            // THESE SHOULD ONLY CORRESPOND TO OUTPUT VARIABLES ON THE SCREEN!
            // Internal variables don't have to be here.
            // Full get/set required because set MUST use NotifyPropertyChanged().
            //=========================================================================

            private String _strSeconds = ":00";                 // Test full size with "00:00"                                                       

            public String strSeconds
            {
                get { return this._strSeconds; }
                set
                {
                    this._strSeconds = value;
                    NotifyPropertyChanged();                                                // KEY PIECE 3: EVERY 'SET' MUST HAVE THIS NPC CALL (no parm required)
                }
            }

            //=========================================================================
            // UI CHANGE MECHANISM (place in every container with properties/variables)     // KEY PIECE 4: THE CLASS/CONTAINER MUST DECLARE THESE TWO ROUTINES
            //=========================================================================

            public event PropertyChangedEventHandler PropertyChanged;                       // can't change this name, it turns out
            public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }


        private String SecondsToMinSec(int secs_parm)
        {
            String result;

            try
            {
                int min = secs_parm / 60;
                int sec = secs_parm % 60;
                result = min.ToString("#:") + sec.ToString("00");
            }
            catch (Exception ex)
            {
                result = "ERR";
            }

            return result;
        }


        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            seconds += 1;
            uiOutputVariables.strSeconds = SecondsToMinSec(seconds);                             // if the UI mechanisms are in place, this should be transparent
        }


        private void btnStart_Clicked(object sender, EventArgs e)
        {
            timer.Enabled = true;

            lblGreenLight.BackgroundColor = Color.FromHex("#00FF44");           // FromArgb will replace in Maui
            lblRedLight.BackgroundColor = Color.DarkRed;                        // Becomes Colors. in Maui
        }

        private void btnStop_Clicked(object sender, EventArgs e)
        {
            timer.Enabled = false;

            lblGreenLight.BackgroundColor = Color.DarkGreen;
            lblRedLight.BackgroundColor = Color.Red;
        }

        private void btnReset_Clicked(object sender, EventArgs e)
        {
            timer.Enabled = false;

            seconds = 0;
            uiOutputVariables.strSeconds = SecondsToMinSec(seconds); ;                            // if the UI mechanisms are in place, this should be transparent

            lblGreenLight.BackgroundColor = Color.DarkGreen;
            lblRedLight.BackgroundColor = Color.DarkRed;
        }

    }
}
