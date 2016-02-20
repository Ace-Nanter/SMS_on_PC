package fr.isima.sms_on_pc;

import android.database.Cursor;
import android.net.Uri;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.util.Log;                    // TODO : to remove
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Button;

import fr.isima.sms_on_pc.SMS.SMS;
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

        // Ajout listener "Stop"
        if(button != null) {
            button.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    System.exit(0);
                }
            });
        }
        else {
            Log.d(Main.class.getSimpleName(), "No way to get exit button !");
            System.exit(0);
        }

        // Récupération du champ texte
        console = (TextView) findViewById(R.id.console);

        if(console != null) {
            console.setText("Connexion en cours...");

            // Launch the connexion
            try {
                link = new LinkManager(this, this);
                SMS.init(this);
            } catch (Exception e) {
                console.append(e.getMessage());
                Log.d(Main.class.getSimpleName(), "An exception occurred : " + e);
                link.disconnect();
                link = null;
            }
        }
        else {
            Log.d(Main.class.getSimpleName(), "No way to get text display !");
            System.exit(0);
        }

        // TODO : a supprimer
        testButton = (Button) findViewById(R.id.testButton);
        if(testButton != null) {
            testButton.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    EditText form = (EditText) findViewById(R.id.editText);


                    if(link != null && link.is_connected()) {
                        String buffer = "" + form.getText();

                        console.append("\nEnvoi de " + buffer);
                        link.send(buffer);
                    }
                    else {
                        console.append("Envoi impossible !");
                    }
                }
            });
        }
    }

    @Override
    protected void onDestroy() {

        // Disconnect the threads
        if(link != null)
            link.disconnect();

        // Force kill
        android.os.Process.killProcess(android.os.Process.myPid());
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
        System.exit(0);
    }
}