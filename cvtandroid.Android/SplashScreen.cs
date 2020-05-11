using Android.App;
using Android.OS;


namespace cvtandroid.Droid
{
    [Activity(Theme = "@style/splashscreen",
              MainLauncher = true,
              NoHistory = true)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            StartActivity(typeof(MainActivity));
        }
    }
}
