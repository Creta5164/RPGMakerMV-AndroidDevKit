package YOUR_PACKAGE_NAME_HERE;

import android.app.Activity;
import android.webkit.JavascriptInterface;
import android.webkit.WebView;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;

public class SaveDataManager {
    
    private WebView client;
    private Activity context;
    private File file;
    
    public SaveDataManager(WebView client, Activity context)
    {
        this.client  = client;
        this.context = context;
        
        file = new File(context.getFilesDir(), "save");
        
        if (!file.isDirectory())
            file.mkdir();
    }
    
    @JavascriptInterface
    public void Save(String name, String base64Data) {
        
        File locate = new File(file.getPath(), String.format("%s.bin", name));
        
        try {
            
            if (!locate.exists())
                locate.createNewFile();
            
            FileOutputStream stream = new FileOutputStream(locate);
            
            stream.write(base64Data.getBytes());
            
            stream.close();
            
        } catch (FileNotFoundException notExistEx) {
            
            //TODO : Handle if file doesn't exist.
            
        } catch (IOException ioex) {
            
            //TODO : Handle if file cannot write.
            
        }
    }
    
    @JavascriptInterface
    public String Load(String name) {
        
        File locate = new File(file.getPath(), String.format("%s.bin", name));
        
        if (!locate.exists())
            return "";
        
        int len = (int)locate.length();
        byte[] data = new byte[len];
        
        try {
            
            FileInputStream stream = new FileInputStream(locate);
            stream.read(data);
            stream.close();
            
        } catch (FileNotFoundException notExistEx) {
            
            //TODO : Handle if file doesn't exist.
            return "";
            
        } catch (IOException ioex) {
            
            //TODO : Handle if file cannot read.
            return "";
            
        }
        
        return new String(data);
    }
    
    @JavascriptInterface
    public boolean Exists(String name) {
        File locate = new File(file.getPath(), String.format("%s.bin", name));
        return locate.exists();
    }
}