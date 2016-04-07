package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Contact;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.DemoTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Department;
import com.jeremyfeinstein.slidingmenu.lib.SlidingMenu;

import org.apache.olingo.client.api.ODataClient;
import org.apache.olingo.client.api.communication.response.ODataRetrieveResponse;
import org.apache.olingo.client.api.domain.ClientEntity;
import org.apache.olingo.client.api.domain.ClientEntitySet;
import org.apache.olingo.client.core.ODataClientFactory;

import java.net.URI;
import java.util.ArrayList;
import java.util.concurrent.TimeUnit;

public class NavigationActivity extends AppCompatActivity {

    String[] navigationItems = { "Contacts", "Departments", "Tasks" };

    SlidingMenu menu;
    TextView logView;
    TextView title;
    LoadEntitiesTask loadEntitiesTask;
    NavigationListViewAdapter navigationListViewAdapter;
    ArrayList<Object> loadedEntities;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_navigation);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

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
        String currentUserName = intent.getStringExtra("userName");
        Toast.makeText(this, "Logged in as " + currentUserName, Toast.LENGTH_SHORT).show();

        title = (TextView) findViewById(R.id.navigationCaptionTextView);

        loadedEntities = new ArrayList<>();

        ListView navigationListView = (ListView) findViewById(R.id.navigationListView);
        navigationListViewAdapter = new NavigationListViewAdapter(this, loadedEntities);

        logView = new TextView(this);
        logView.setText("OK");
        navigationListView.addFooterView(logView);

        navigationListView.setAdapter(navigationListViewAdapter);

        loadEntities(navigationItems[0]);
    }

    public void loadEntities(String name) {
        getSupportActionBar().setTitle(name);
        title.setText(name);
        loadedEntities.clear();

        ODataClient client = ODataClientFactory.getClient();
        loadEntitiesTask = new LoadEntitiesTask(client);
        loadEntitiesTask.execute(name);
    }

    class LoadEntitiesTask extends AsyncTask<String, Integer, Void> {

        private final ODataClient client;

        public LoadEntitiesTask(final ODataClient client) {
            this.client = client;
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            logView.setText("Begin");
        }

        @Override
        protected Void doInBackground(String... entityNames) {
            try {
                String entityName = entityNames[0];

                String serviceRoot = "http://192.168.50.200:800";
                URI customersUri = client.newURIBuilder(serviceRoot)
                        .appendEntitySetSegment(entityName).count(true).build();

                // ODataRetrieveResponse<ClientEntitySetIterator<ClientEntitySet, ClientEntity>> response
                //         = client.getRetrieveRequestFactory().getEntitySetIteratorRequest(customersUri).execute();


                ODataRetrieveResponse<ClientEntitySet> response = client.getRetrieveRequestFactory().getEntitySetRequest(customersUri).execute();

                ClientEntitySet entitySet = response.getBody();

                publishProgress(entitySet.getCount());

                for (ClientEntity entity : entitySet.getEntities()) {
                    if (entityName == "Contacts") {
                        Contact contact = new Contact();
                        contact.Id = Integer.parseInt(entity.getProperty("Id").getValue().toString());
                        contact.Name = entity.getProperty("Name").getValue().toString();
                        contact.Address = entity.getProperty("Address").getValue().toString();

                        loadedEntities.add(contact);
                    }

                    if (entityName == "Departments") {
                        Department department = new Department();
                        department.Id = Integer.parseInt(entity.getProperty("Id").getValue().toString());
                        department.Office = entity.getProperty("Office").getValue().toString();
                        department.Title = entity.getProperty("Title").getValue().toString();

                        loadedEntities.add(department);
                    }

                    if (entityName == "Tasks") {
                        DemoTask task = new DemoTask();
                        task.Id = Integer.parseInt(entity.getProperty("Id").getValue().toString());
                        task.Description = entity.getProperty("Description").getValue().toString();
                        task.Note = entity.getProperty("Note").getValue().toString();
                        task.PercentCompleted = Integer.parseInt(entity.getProperty("PercentCompleted").getValue().toString());

                        loadedEntities.add(task);
                    }
                }
            } catch (Exception e) {
                // TODO: implement
            }

            // ClientEntitySetIterator<ClientEntitySet, ClientEntity> iterator = response.getBody();

            // iterator.getCount();
            /*

            while (iterator.hasNext()) {
                ClientEntity customer = iterator.next();
                List<ClientProperty> properties = customer.getProperties();
                for (ClientProperty property : properties) {
                    String name = property.getName();
                    ClientValue value = property.getValue();
                    String valueType = value.getTypeName();

                    Log.d("ODATA-customer", "name: " + name + ", value: " + value.toString() + ", type: " + valueType);
                }
            }
            */
            /*
            try {
                int cnt = 0;
                for (String url : urls) {
                    // загружаем файл
                    downloadFile(url);
                    // выводим промежуточные результаты
                    publishProgress(++cnt);
                }
                // разъединяемся
                TimeUnit.SECONDS.sleep(1);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            */
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
            navigationListViewAdapter.notifyDataSetChanged();
            // logView.setText("End");
        }

        private void downloadFile(String url) throws InterruptedException {
            TimeUnit.SECONDS.sleep(2);
        }
    }
}
