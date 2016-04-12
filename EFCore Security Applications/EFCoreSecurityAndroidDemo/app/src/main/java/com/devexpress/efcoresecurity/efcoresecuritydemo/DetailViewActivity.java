package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.content.Intent;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Toast;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Contact;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.DemoTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Department;

public class DetailViewActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_detail_view);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        Intent intent =  getIntent();
        BaseSecurityEntity currentEntity = (BaseSecurityEntity) intent.getSerializableExtra("entity");
        Toast.makeText(this, "Entity " + currentEntity.toString(), Toast.LENGTH_SHORT).show();

        if(currentEntity.getClass() == Contact.class) {
            fillContact((Contact)currentEntity);
        }

        if(currentEntity.getClass() == Department.class) {
            fillDepartment((Department)currentEntity);
        }

        if(currentEntity.getClass() == DemoTask.class) {
            fillTask((DemoTask) currentEntity);
        }
    }

    void fillContact(Contact contact) {

    }

    void fillDepartment(Department department) {

    }

    void fillTask(DemoTask task) {

    }
}
