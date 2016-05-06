package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.content.Context;
import android.content.res.Resources;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.TextView;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Contact;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.DemoTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Department;

import org.w3c.dom.Text;

import java.util.ArrayList;

/**
 * Created by neroslavskiy.a on 4/6/2016.
 */
public class NavigationListViewAdapter extends BaseAdapter {
    Context context;
    LayoutInflater inflater;
    ArrayList<BaseSecurityEntity> objects;
    Resources resources;
    SecurityHelper helper;

    NavigationListViewAdapter(Context context, ArrayList<BaseSecurityEntity> objects, Resources resources) {
        this.context = context;
        this.objects = objects;
        this.resources = resources;
        inflater = (LayoutInflater) context
                .getSystemService(Context.LAYOUT_INFLATER_SERVICE);

        helper = new SecurityHelper(resources);
    }

    @Override
    public int getCount() {
        return objects.size();
    }

    @Override
    public Object getItem(int position) {
        if(objects.size() - 1 >= position)
            return objects.get(position);
        else
            return null;
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    enum EntityType { CONTACT, DEPARTMENT, TASK };

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        Object currentEntity = getObjectFromPosition(position);

        if(currentEntity == null) {
            TextView nullTextView = new TextView(context);
            nullTextView.setText("NULL");
        }

        EntityType currentEntityType = EntityType.CONTACT;

        if(currentEntity.getClass() == Contact.class) {
            currentEntityType = EntityType.CONTACT;
        }

        if(currentEntity.getClass() == Department.class) {
            currentEntityType = EntityType.DEPARTMENT;
        }

        if(currentEntity.getClass() == DemoTask.class) {
            currentEntityType = EntityType.TASK;
        }

        Log.d("NAVADAPTER", "Pos: " + position + ", type: " + currentEntityType.toString());

        View view = null;
        if (view == null) {
            switch(currentEntityType) {
                case CONTACT:
                    view = inflater.inflate(R.layout.navigation_contact_item, parent, false);
                    break;
                case DEPARTMENT:
                    view = inflater.inflate(R.layout.navigation_department_item, parent, false);
                    break;
                case TASK:
                    view = inflater.inflate(R.layout.navigation_task_item, parent, false);
                    break;
                default:
                    throw new IllegalArgumentException("Unknown entity type: " + currentEntityType.toString());
            }
        }

        switch(currentEntityType) {
            case CONTACT:
                fillContact(view, (Contact)currentEntity);
                break;
            case DEPARTMENT:
                fillDepartment(view, (Department)currentEntity);
                break;
            case TASK:
                fillTask(view, (DemoTask)currentEntity);
                break;
            default:
                throw new IllegalArgumentException("Unknown entity type: " + currentEntityType.toString());
        }

        CheckBox checkBox = (CheckBox) view.findViewById(R.id.checkBox);
        checkBox.setOnCheckedChangeListener(checkBoxChangeListener);
        checkBox.setTag(position);
        checkBox.setChecked(false);
        return view;
    }

    public void fillContact(View view, Contact contact) {
        helper.setTextInTextView(view.findViewById(R.id.textViewName), contact, contact.Name, "Name");
        helper.setTextInTextView(view.findViewById(R.id.textViewAddress), contact, contact.Address, "Address");
        ((ImageView) view.findViewById(R.id.contactImage)).setImageResource(R.mipmap.ic_launcher);
    }

    public void fillDepartment(View view, Department department) {
        helper.setTextInTextView(view.findViewById(R.id.textViewTitle), department, department.Title, "Title");
        helper.setTextInTextView(view.findViewById(R.id.textViewOffice), department, department.Office, "Office");
    }

    public void fillTask(View view, DemoTask task) {
        helper.setTextInTextView(view.findViewById(R.id.textViewDescription), task, task.Description, "Description");
        helper.setTextInTextView(view.findViewById(R.id.textViewPercentCompleted), task, task.PercentCompleted + "%", "PercentCompleted");
    }

    BaseSecurityEntity getObjectFromPosition(int position) {
        return ((BaseSecurityEntity) getItem(position));
    }

    CompoundButton.OnCheckedChangeListener checkBoxChangeListener = new CompoundButton.OnCheckedChangeListener() {
        public void onCheckedChanged(CompoundButton buttonView,
                                     boolean isChecked) {
            // getObjectFromPosition((Integer) buttonView.getTag()).box = isChecked;
        }
    };
}
