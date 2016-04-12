package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;
import android.widget.Toast;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Contact;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.DemoTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Department;

public class DetailViewActivity extends AppCompatActivity {

    View detailView;
    LayoutInflater layoutInflater;
    RelativeLayout container;
    SecurityHelper helper;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_detail_view);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        Intent intent =  getIntent();
        BaseSecurityEntity currentEntity = (BaseSecurityEntity) intent.getSerializableExtra("entity");

        layoutInflater = (LayoutInflater) this.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        container = (RelativeLayout) findViewById(R.id.detailViewContainer);
        helper = new SecurityHelper(getResources());

        if(currentEntity.getClass() == Contact.class) {
            fillContact((Contact)currentEntity);
            setTitle("Details: Contact");
        }

        if(currentEntity.getClass() == Department.class) {
            fillDepartment((Department)currentEntity);
            setTitle("Details: Department");
        }

        if(currentEntity.getClass() == DemoTask.class) {
            fillTask((DemoTask) currentEntity);
            setTitle("Details: Task");
        }
        if(detailView != null)
            container.addView(detailView);
    }

    void fillContact(Contact contact) {
        detailView = layoutInflater.inflate(R.layout.detail_view_contact, null, false);

        helper.setTextInTextView(detailView.findViewById(R.id.textViewName), contact, contact.Name, "Name");
        helper.setTextInTextView(detailView.findViewById(R.id.textViewAddress), contact, contact.Address, "Address");
        // TODO: show real data
        helper.setTextInTextView(detailView.findViewById(R.id.textViewDepartment), contact, "Department", "Department");
        helper.setTextInTextView(detailView.findViewById(R.id.textViewTasks), contact, "0 tasks", "ContactTasks");
    }

    void fillDepartment(Department department) {
        detailView = layoutInflater.inflate(R.layout.detail_view_department, null, false);

        helper.setTextInTextView(detailView.findViewById(R.id.textViewTitle), department, department.Title, "Title");
        helper.setTextInTextView(detailView.findViewById(R.id.textViewOffice), department, department.Office, "Office");
    }

    void fillTask(DemoTask task) {
        detailView = layoutInflater.inflate(R.layout.detail_view_task, null, false);

        helper.setTextInTextView(detailView.findViewById(R.id.textViewDescription), task, task.Description, "Description");
        helper.setTextInTextView(detailView.findViewById(R.id.textViewDescription), task, task.Note, "Note");
        helper.setTextInTextView(detailView.findViewById(R.id.textViewPercentCompleted), task, task.PercentCompleted + "%", "PercentCompleted");

        final String DATE_FORMAT = "%1$tA %1$tb %1$td %1$tY at %1$tI:%1$tM %1$Tp";
        String startDateString = String.format(DATE_FORMAT, task.StartDate);
        String completeDateString = String.format(DATE_FORMAT, task.DateCompleted);
        helper.setTextInTextView(detailView.findViewById(R.id.textViewStartDate), task, startDateString, "StartDate");
        helper.setTextInTextView(detailView.findViewById(R.id.textViewCompleteDate), task, completeDateString, "DateCompleted");
    }
}
