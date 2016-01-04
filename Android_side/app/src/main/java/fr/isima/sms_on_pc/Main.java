package fr.isima.sms_on_pc;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;                    // TODO : to remove
import android.view.View;
import android.widget.TextView;
import android.widget.Button;

public class Main extends AppCompatActivity implements USB.Listener {

    private USB link = null;
    private TextView console;
    private Button connect_button;

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
                console.setText(link.get_descriptors());
                connect_button = (Button) findViewById(R.id.connect_button);
                connect_button.setOnClickListener(new View.OnClickListener() {
                    // TODO : mettre un neutraliseur du bouton afin de ne pas pouvoir pas recliquer pendant la tentative de connexion
                    public void onClick(View v) {
                        try {
                            link.connect();
                        }
                        catch(Exception e) {
                            console.append("Une exception est survenue :" + e);
                        }
                    }
                });
            }
            else {
                console.setText("Erreur lors de l'initialisation de la connexion USB !");
            }
        }
    }

    @Override
    protected void onDestroy() {
        link.stop();
        link = null;
        super.onDestroy();
    }

    // TODO : to remove ?
    @Override
    public void hasBeenConnected() {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                console.append("\nConnecté !");
            }
        });
    }

    @Override
    public void hasRead(final String data) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(Main.class.getSimpleName(), "J'ai lu un truc : " + data);
                console.append("\n" + data);
                link.write("Coucou");
            }
        });
    }

    @Override
    public void UsbStop() {
        console.append("Connexion arrêtée");
    }
}