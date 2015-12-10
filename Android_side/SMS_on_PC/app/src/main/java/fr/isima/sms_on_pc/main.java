package fr.isima.sms_on_pc;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.TextView;

import java.io.IOException;

import fr.isima.sms_on_pc.USB;

public class Main extends AppCompatActivity implements USB.Listener {

    private USB link = null;
    private TextView console;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        console = (TextView) findViewById(R.id.console);
        if(console != null) {
           try {
               link = new USB(this, this);
           } catch (Exception e) {
               console.append(e.getMessage());
               link = null;
           }
           if (link != null) {
                if (link.connect()) {
                    console.append("Connecté.");
                } else {
                    console.append("Impossible de se connecter !");
                }
            }
        }
    }

    @Override
    protected void onDestroy() {
        link.stop();
        link = null;
        super.onDestroy();
    }

    @Override
    public void hasRead(final String data) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                console.append("Données reçues : " + data);
            }
        });
    }

    @Override
    public void UsbStop() {
        console.append("Connexion arrêtée");
    }
}