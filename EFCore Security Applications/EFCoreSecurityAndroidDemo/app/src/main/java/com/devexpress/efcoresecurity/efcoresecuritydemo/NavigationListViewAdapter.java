package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.content.Context;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.TextView;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Contact;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.DemoTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Department;

import java.util.ArrayList;

/**
 * Created by neroslavskiy.a on 4/6/2016.
 */
public class NavigationListViewAdapter extends BaseAdapter {
    Context ctx;
    LayoutInflater inflater;
    ArrayList<Object> objects;

    NavigationListViewAdapter(Context context, ArrayList<Object> objects) {
        ctx = context;
        this.objects = objects;
        inflater = (LayoutInflater) ctx
                .getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    }

    @Override
    public int getCount() {
        return objects.size();
    }

    @Override
    public Object getItem(int position) {
        return objects.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    enum EntityType { CONTACT, DEPARTMENT, TASK };

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        Object currentEntity = getObjectFromPosition(position);

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
        ((TextView) view.findViewById(R.id.textViewName)).setText(contact.Name);
        ((TextView) view.findViewById(R.id.textViewAddress)).setText(contact.Address);
        ((ImageView) view.findViewById(R.id.contactImage)).setImageResource(R.mipmap.ic_launcher);
    }

    public void fillDepartment(View view, Department department) {
        ((TextView) view.findViewById(R.id.textViewTitle)).setText(department.Title);
        ((TextView) view.findViewById(R.id.textViewOffice)).setText(department.Office);
    }

    public void fillTask(View view, DemoTask task) {
        ((TextView) view.findViewById(R.id.textViewDescription)).setText(task.Description);
        ((TextView) view.findViewById(R.id.textViewPercentCompleted)).setText(task.PercentCompleted + "%");
    }

    Object getObjectFromPosition(int position) {
        return ((Object) getItem(position));
    }

    ArrayList<Object> getBox() {
        ArrayList<Object> box = new ArrayList<Object>();
        for (Object p : objects) {
            // if (p.box)
            //    box.add(p);
        }
        return box;
    }

    CompoundButton.OnCheckedChangeListener checkBoxChangeListener = new CompoundButton.OnCheckedChangeListener() {
        public void onCheckedChanged(CompoundButton buttonView,
                                     boolean isChecked) {
            // getObjectFromPosition((Integer) buttonView.getTag()).box = isChecked;
        }
    };
}
