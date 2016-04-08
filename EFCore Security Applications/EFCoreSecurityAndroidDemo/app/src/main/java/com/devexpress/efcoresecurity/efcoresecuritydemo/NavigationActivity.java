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
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Contact;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.DemoTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Department;
import com.jeremyfeinstein.slidingmenu.lib.SlidingMenu;

import org.apache.olingo.client.api.ODataClient;
import org.apache.olingo.client.api.communication.response.ODataRetrieveResponse;
import org.apache.olingo.client.api.domain.ClientEntity;
import org.apache.olingo.client.api.domain.ClientEntitySet;
import org.apache.olingo.client.api.domain.ClientProperty;
import org.apache.olingo.client.core.ODataClientFactory;
import org.apache.olingo.client.core.domain.ClientCollectionValueImpl;
import org.apache.olingo.commons.api.edm.Edm;

import java.net.URI;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;

public class NavigationActivity extends AppCompatActivity {

    String[] navigationItems = { "Contacts", "Departments", "Tasks" };


    Context context;
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
        String currentUserName = intent.getStringExtra("userName");
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
                Log.d("Nav-onclick", String.valueOf(position));

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
        loadEntitiesTask = new LoadEntitiesTask(client);

        loadEntitiesTask.execute(name);

        // sync call, to avoid exceptions
        /*
        try {
            loadEntitiesTask.get();
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

                // String serviceRoot = "http://192.168.50.200:800";
                String serviceRoot = "http://efcoresecurityodataservice20160407060012.azurewebsites.net";
                URI customersUri = client.newURIBuilder(serviceRoot)
                        .appendEntitySetSegment(entityName).count(true).build();

                // ODataRetrieveResponse<ClientEntitySetIterator<ClientEntitySet, ClientEntity>> response
                //         = client.getRetrieveRequestFactory().getEntitySetIteratorRequest(customersUri).execute();


                ODataRetrieveResponse<ClientEntitySet> response = client.getRetrieveRequestFactory().getEntitySetRequest(customersUri).execute();

                ClientEntitySet entitySet = response.getBody();

                publishProgress(entitySet.getCount());

                // loadedEntities.clear();

                for (ClientEntity entity : entitySet.getEntities()) {
                    BaseSecurityEntity loadedEntity = null;

                    if (entityName == "Contacts") {
                        Contact contact = new Contact();
                        contact.Name = entity.getProperty("Name").getValue().toString();
                        contact.Address = entity.getProperty("Address").getValue().toString();

                        loadedEntity = contact;
                    }

                    if (entityName == "Departments") {
                        Department department = new Department();
                        department.Office = entity.getProperty("Office").getValue().toString();
                        department.Title = entity.getProperty("Title").getValue().toString();

                        loadedEntity = department;
                    }

                    if (entityName == "Tasks") {
                        DemoTask task = new DemoTask();
                        task.Description = entity.getProperty("Description").getValue().toString();
                        task.Note = entity.getProperty("Note").getValue().toString();
                        task.PercentCompleted = Integer.parseInt(entity.getProperty("PercentCompleted").getValue().toString());

                        loadedEntity = task;
                    }

                    if (loadedEntity != null) {
                        loadedEntity.Id = Integer.parseInt(entity.getProperty("Id").getValue().toString());

                        ClientProperty blockedMembersProperty =  entity.getProperty("BlockedMembers");
                        ClientCollectionValueImpl blockedMembers = (ClientCollectionValueImpl)blockedMembersProperty.getValue();

                        loadedEntity.BlockedMembers = new ArrayList();

                        for(Object blockedMember : blockedMembers.asJavaCollection()) {
                            loadedEntity.BlockedMembers.add(blockedMember.toString());
                        }

                        loadedEntities.add(loadedEntity);
                    }
                }
            } catch (Exception e) {
                // TODO: implement
                Log.d("EXCEPTION", e.getMessage());
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
            progressDialog.hide();
            // logView.setText("End");
        }

        private void downloadFile(String url) throws InterruptedException {
            TimeUnit.SECONDS.sleep(2);
        }
    }
}
