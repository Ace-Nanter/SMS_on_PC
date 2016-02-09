package fr.isima.sms_on_pc;

import android.database.Cursor;
import android.net.Uri;
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
        super.onCreate(savedInstanceState);                     // Création de l'activité
        setContentView(R.layout.activity_main);                 // Définition de la vue

        button = (Button) findViewById(R.id.button);            // Recherche du bouton
        if(button != null) {
            button.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    Main.this.finish();
                }
            });
        }
        else {
            Log.d(Main.class.getSimpleName(), "No way to get exit button !");
            Main.this.finish();
        }

        // Récupération du champ texte
        console = (TextView) findViewById(R.id.console);

        if(console != null) {
            console.setText("Connexion en cours...");
            try {
                link = new LinkManager(this, this);                     // Lance la connexion
            } catch (Exception e) {
                console.append(e.getMessage());
                Log.d(Main.class.getSimpleName(), "An exception occurred : " + e);
                link.disconnect();
                link = null;
            }
            Uri sentURI = Uri.parse("content://sms/sent");

            Cursor cur = getContentResolver().query(sentURI, null, null, null, null);

            if (cur.moveToFirst()) {
                if (cur != null) {
                    String msgData = "";
                    for (int idx = 0; idx < cur.getColumnCount(); idx++) {
                        msgData += "\n" + cur.getColumnName(idx) + ":" + cur.getString(idx);
                    }
                    console.append(msgData);
                }
            }
            else {
                console.append("Problème : messagerie vide !");
            }
        }
        else {
            Log.d(Main.class.getSimpleName(), "No way to get text display !");
            Main.this.finish();
        }


        // TODO : a supprimer
        testButton = (Button) findViewById(R.id.testButton);
        if(testButton != null) {
            testButton.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    console.append("\nEnvoi d'un message...");
                    link.send("Coucou !");
                }
            });
        }
    }

    @Override
    protected void onDestroy() {
        if(link != null)
            link.disconnect();

        super.onDestroy();
    }

    @Override
    public void Connected(boolean is_connected) {
        if(is_connected)
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    console.setText(link.get_descriptors());
                }
            });
        else
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    console.setText("Aucun ordinateur détecté !");
                }
            });
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