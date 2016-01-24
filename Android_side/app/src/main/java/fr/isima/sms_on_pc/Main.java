package fr.isima.sms_on_pc;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;                    // TODO : to remove
import android.view.View;
import android.widget.TextView;
import android.widget.Button;

import fr.isima.sms_on_pc.USB.LinkManager;
import fr.isima.sms_on_pc.USB.UsbInterface;

public class Main extends AppCompatActivity implements UsbInterface {

    private LinkManager link = null;
    private TextView console;
    private Button button;
    private Button testButton;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);



        button = (Button) findViewById(R.id.button);
        if(button != null) {
            button.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    Main.this.finish();
                }
            });
        }

        testButton = (Button) findViewById(R.id.testButton);
        if(testButton != null) {
            testButton.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    console.append("\nEnvoi d'un message...");
                    link.send("Coucou !");
                }
            });
        }


        // Récupération du champ texte
        console = (TextView) findViewById(R.id.console);

        if(console != null) {
            try {
                link = new LinkManager(this, this);                     // Lance la connexion
            } catch (Exception e) {
                console.append(e.getMessage());
                link.disconnect();
                link = null;
            }
            if (link.is_connected()) {
                console.setText(link.get_descriptors());
            }
            else {
                console.setText("Aucun appareil détecté !");
            }
        }
    }

    @Override
    protected void onDestroy() {
        if(link != null)
            link.disconnect();

        super.onDestroy();
    }

    @Override
    public void hasRead(final String data) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                console.append("\n>>" + data);
            }
        });
    }

    @Override
    public void UsbStop() {
        console.setText("Connexion arrêtée");
        try {
            Thread.sleep(3000);
        }
        catch (Exception e) {
            Log.d(Main.class.getSimpleName(), "Erreur lors d'un sleep");
        }
        finish();                                                       // Arrêt de l'application
    }
}