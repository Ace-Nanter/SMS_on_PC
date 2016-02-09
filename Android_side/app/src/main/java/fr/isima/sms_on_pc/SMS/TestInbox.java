package fr.isima.sms_on_pc.SMS;

/**
 * Created by Ace Nanter on 26/01/2016.
 */

import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import fr.isima.sms_on_pc.R;


public class TestInbox extends AppCompatActivity {

    private TextView console;
    private Button button;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);                     // Création de l'activité
        setContentView(R.layout.activity_main);                 // Définition de la vue

        button = (Button) findViewById(R.id.button);            // Recherche du bouton
        if (button != null) {
            button.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    TestInbox.this.finish();
                }
            });
        } else {
            Log.d(TestInbox.class.getSimpleName(), "No way to get exit button !");
            TestInbox.this.finish();
        }

        // Récupération du champ texte
        console = (TextView) findViewById(R.id.console);

        if (console != null) {
            // Create Sent box URI
            Uri sentURI = Uri.parse("content://sms/sent");

            Cursor cur = getContentResolver().query(Uri.parse("content://sms/inbox"), null, null, null, null);

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
                console.setText("Problème : vide !");
            }
        } else {
            console.setText("Problème !");
        }
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
    }
}