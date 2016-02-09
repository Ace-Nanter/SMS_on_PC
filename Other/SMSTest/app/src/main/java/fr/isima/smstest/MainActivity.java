package fr.isima.smstest;

import android.content.Intent;
import android.database.Cursor;
import android.net.Uri;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.telephony.SmsManager;
import android.telephony.SmsMessage;
import android.view.View;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.TextView;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

       FloatingActionButton fab = (FloatingActionButton) findViewById(R.id.fab);
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                SmsManager manager = SmsManager.getDefault();
                manager.sendTextMessage("0647657049", null, "Coucou !", null, null);

                Snackbar.make(view, "Done !", Snackbar.LENGTH_LONG)
                        .setAction("Action", null).show();
            }
        });
        TextView console = (TextView) findViewById(R.id.Console);
        if(console != null) {
            console.setText("Données : \n");

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
            } else {
                console.append("Problème : messagerie vide !");
            }
        }

    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }
}
