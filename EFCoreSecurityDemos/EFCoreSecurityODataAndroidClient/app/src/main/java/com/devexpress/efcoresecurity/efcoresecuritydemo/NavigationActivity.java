package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.app.ProgressDialog;
import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.devexpress.efcoresecurity.efcoresecuritydemo.authentication.PreemptiveAuthInterceptor;
import com.devexpress.efcoresecurity.efcoresecuritydemo.authentication.PreemptiveBasicAuthHttpClientFactory;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;
import com.jeremyfeinstein.slidingmenu.lib.SlidingMenu;
import org.apache.olingo.client.api.ODataClient;
import org.apache.olingo.client.core.ODataClientFactory;

import java.util.ArrayList;
import java.util.concurrent.ExecutionException;

public class NavigationActivity extends AppCompatActivity {

    String[] navigationItems = { "Contacts", "Departments", "Tasks" };

    Context context;
    String currentUserName;
    SlidingMenu menu;
    TextView logView;
    TextView title;
    LoadEntitiesTask loadEntitiesTask;
    NavigationListViewAdapter navigationListViewAdapter;
    ArrayList<BaseSecurityEntity> loadedEntities;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_navigation);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        getSupportActionBar().setHomeButtonEnabled(true);

        this.context = this;

        menu = new SlidingMenu(this);
        menu.setMode(SlidingMenu.LEFT);
        menu.setTouchModeAbove(SlidingMenu.TOUCHMODE_FULLSCREEN);
        menu.setFadeDegree(0.35f);
        menu.attachToActivity(this, SlidingMenu.SLIDING_WINDOW);
        menu.setMenu(R.layout.sidemenu);
        menu.setBehindWidthRes(R.dimen.slidingmenu_behind_width);

        ListView sideMenuListView = (ListView) findViewById(R.id.sidemenu);

        sideMenuListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                menu.toggle();
                loadEntities(navigationItems[position]);
            }
        });

        sideMenuListView.setAdapter(
                new ArrayAdapter<Object>(
                        this,
                        R.layout.sidemenu_item,
                        R.id.text,
                        navigationItems
                )
        );

        sideMenuListView.setItemChecked(0, true);

        Intent intent =  getIntent();
        currentUserName = intent.getStringExtra("userName");
        Toast.makeText(this, "Logged in as " + currentUserName, Toast.LENGTH_SHORT).show();

        title = (TextView) findViewById(R.id.navigationCaptionTextView);

        loadedEntities = new ArrayList<>();

        ListView navigationListView = (ListView) findViewById(R.id.navigationListView);
        navigationListViewAdapter = new NavigationListViewAdapter(this, loadedEntities, getResources());

        logView = new TextView(this);
        logView.setText("OK");
        navigationListView.addFooterView(logView);

        navigationListView.setAdapter(navigationListViewAdapter);
        navigationListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                BaseSecurityEntity currentEntity = null;

                if(loadedEntities.size() - 1 >= position)
                    currentEntity =  loadedEntities.get(position);

                if(currentEntity == null) {
                    Toast.makeText(context, "Entity is null!", Toast.LENGTH_SHORT).show();
                    return;
                }

                Intent intent = new Intent(context, DetailViewActivity.class);
                intent.putExtra("entity", currentEntity);
                startActivity(intent);
            }
        });

        loadEntities(navigationItems[0]);
    }

    @Override
    protected void onResume() {
        super.onResume();

        loadEntitiesTask = null;
    }

    @Override
    protected void onPause() {
        super.onPause();

        loadEntitiesTask = null;
    }

    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK) {
            if(menu.isMenuShowing()){
                menu.toggle(true);
                return false;
            }
        }
        return super.onKeyDown(keyCode, event);
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case android.R.id.home:
                menuToggle();
                return true;
        }
        return super.onOptionsItemSelected(item);
    }

    public void menuToggle(){
        if(menu.isMenuShowing())
            menu.showContent();
        else
            menu.showMenu();
    }

    public void loadEntities(String name) {
        getSupportActionBar().setTitle(name);
        title.setText(name);
        loadedEntities.clear();

        /*
        if(loadEntitiesTask != null) {
            loadEntitiesTask.cancel(true);
        }
        */

        ODataClient client = ODataClientFactory.getClient();
        String currentUserPassword = currentUserName;
        client.getConfiguration().setHttpClientFactory(new PreemptiveBasicAuthHttpClientFactory(currentUserName, currentUserPassword));

        loadEntitiesTask = new LoadEntitiesTask(client);

        loadEntitiesTask.execute(name);

        // sync call, to avoid exceptions
        /*
        try {
            loadEntitiesTask.get();
            navigationListViewAdapter.notifyDataSetChanged();
        } catch (InterruptedException e) {
            e.printStackTrace();
        } catch (ExecutionException e) {
            e.printStackTrace();
        }
        */
    }

    class LoadEntitiesTask extends AsyncTask<String, Integer, Void> {

        private final ODataClient client;
        ProgressDialog progressDialog;

        public LoadEntitiesTask(final ODataClient client) {
            this.client = client;
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            logView.setText("Begin");

            progressDialog = new ProgressDialog(context);
            progressDialog.setMessage("Loading...");
            progressDialog.setCancelable(false);
            progressDialog.show();
        }

        @Override
        protected Void doInBackground(String... entityNames) {
            try {
                String entityName = entityNames[0];
                ODataEntityLoader entityLoader = new ODataEntityLoader(client);

                loadedEntities.clear();

                for(BaseSecurityEntity entity : entityLoader.loadEntities(entityName))
                    loadedEntities.add(entity);

                publishProgress(loadedEntities.size());
            } catch (Exception e) {
                // TODO: implement
                Log.d("EXCEPTION-odata", e.getMessage());
                Log.d("EXCEPTION-odata", e.toString());
                e.printStackTrace();
            }
            return null;
        }

        @Override
        protected void onProgressUpdate(Integer... values) {
            super.onProgressUpdate(values);
            logView.setText("Loaded " + values[0] + " entities");
        }

        @Override
        protected void onPostExecute(Void result) {
            super.onPostExecute(result);
            // progressDialog.hide();
            progressDialog.dismiss();
            navigationListViewAdapter.notifyDataSetChanged();
            // logView.setText("End");
        }
    }
}
