package YOUR_PACKAGE_NAME_HERE;

import android.support.v7.app.AppCompatActivity;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.webkit.WebView;
import android.webkit.WebSettings;
import android.webkit.WebViewClient;
import android.webkit.WebChromeClient;

public class MainView extends AppCompatActivity {

    private int currentApiVersion;

    private WebView gameView;
    private int UIOptionFlags;
    private View decorView;

    //Called when the current activity view is created.
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        currentApiVersion = android.os.Build.VERSION.SDK_INT;

        decorView = getWindow().getDecorView();
        InitNavigationControl();

        setContentView(R.layout.activity_main_view);

        //TODO : If it is a release build, make this code inactive.
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT) {
            WebView.setWebContentsDebuggingEnabled(true);
        }

        //TODO : Prepare to load

        gameView = (WebView) findViewById(R.id.game_webview);
        gameView.setWebViewClient(new WebViewClient());

        WebSettings webSettings = gameView.getSettings();
        webSettings.setDomStorageEnabled(true);
        webSettings.setJavaScriptEnabled(true);
        webSettings.setAllowUniversalAccessFromFileURLs(true);

        StringBuilder URL = new StringBuilder();
        URL.append("file:///android_asset/index.html");

        if (currentApiVersion <= Build.VERSION_CODES.LOLLIPOP_MR1)
            URL.append("?android-legacy=true");

        gameView.loadUrl(URL.toString());

        gameView.setWebChromeClient(new WebChromeClient(){

            @Override
            public void onCloseWindow(WebView w) {
                super.onCloseWindow(w);

                //Since the webview is closed, app will terminated.
                
                finishAffinity();
                System.exit(0);
            }
        });
    }

    //Prepare the ability to hide the navigation bar.
    private void InitNavigationControl()
    {
        UIOptionFlags = View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                        | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                        | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN;

        if( Build.VERSION.SDK_INT >= Build.VERSION_CODES.ICE_CREAM_SANDWICH )
            UIOptionFlags |= View.SYSTEM_UI_FLAG_HIDE_NAVIGATION;

        if( Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN )
            UIOptionFlags |= View.SYSTEM_UI_FLAG_FULLSCREEN;

        //This feature only works on Android 4.4 or later.
        if(currentApiVersion >= Build.VERSION_CODES.KITKAT)
        {
            UIOptionFlags |= View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
            getWindow().getDecorView().setSystemUiVisibility(UIOptionFlags);

            //The code below is for handling volume up or volume down.
            //Without this feature, a navigation bar will appear when you press the volume button and it will not be hidden.
            decorView.setOnSystemUiVisibilityChangeListener(
                    new View.OnSystemUiVisibilityChangeListener()
                    {
                        @Override
                        public void onSystemUiVisibilityChange(int visibility)
                        {
                            if((visibility & View.SYSTEM_UI_FLAG_FULLSCREEN) == 0)
                            {
                                decorView.setSystemUiVisibility(UIOptionFlags);
                            }
                        }
                    }
            );
        } else {
            // Hide both the navigation bar and the status bar.
            // SYSTEM_UI_FLAG_FULLSCREEN is only available on Android 4.1 and higher, but as
            // a general rule, you should design your app to hide the status bar whenever you
            // hide the navigation bar.
            // https://developer.android.com/training/system-ui/navigation

            decorView.setSystemUiVisibility(UIOptionFlags);
        }
    }

    @Override
    public void onWindowFocusChanged(boolean hasfocus) {
        if (hasfocus)
            decorView.setSystemUiVisibility(UIOptionFlags);
    }
}