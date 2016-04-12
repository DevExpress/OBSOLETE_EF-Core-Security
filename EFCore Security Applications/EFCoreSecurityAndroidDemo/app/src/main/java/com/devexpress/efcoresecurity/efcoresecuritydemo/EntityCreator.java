package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.util.Log;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Contact;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.DemoTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Department;

import org.apache.olingo.client.api.domain.ClientEntity;
import org.apache.olingo.client.api.domain.ClientProperty;
import org.apache.olingo.client.core.domain.ClientCollectionValueImpl;

import java.security.Timestamp;
import java.util.ArrayList;
import java.util.Calendar;

/**
 * Created by unfo on 12.04.2016.
 */
public class EntityCreator {
    static void fillBaseSecurityEntity(BaseSecurityEntity baseSecurityEntity, ClientEntity clientEntity) {
        if (baseSecurityEntity != null) {
            baseSecurityEntity.Id = Integer.parseInt(clientEntity.getProperty("Id").getValue().toString());

            ClientProperty blockedMembersProperty = clientEntity.getProperty("BlockedMembers");
            ClientCollectionValueImpl blockedMembers = (ClientCollectionValueImpl) blockedMembersProperty.getValue();

            baseSecurityEntity.BlockedMembers = new ArrayList();

            for (Object blockedMember : blockedMembers.asJavaCollection()) {
                baseSecurityEntity.BlockedMembers.add(blockedMember.toString());
            }
        }
    }

    public static Contact createContact(ClientEntity clientEntity) {
        Contact contact = new Contact();
        contact.Name = clientEntity.getProperty("Name").getValue().toString();
        contact.Address = clientEntity.getProperty("Address").getValue().toString();

        fillBaseSecurityEntity(contact, clientEntity);

        return contact;
    }

    public static Department createDepartment(ClientEntity clientEntity) {
        Department department = new Department();
        department.Office = clientEntity.getProperty("Office").getValue().toString();
        department.Title = clientEntity.getProperty("Title").getValue().toString();

        fillBaseSecurityEntity(department, clientEntity);

        return department;
    }

    public static DemoTask createTask(ClientEntity clientEntity) {
        DemoTask task = new DemoTask();
        task.Description = clientEntity.getProperty("Description").getValue().toString();
        task.Note = clientEntity.getProperty("Note").getValue().toString();
        task.PercentCompleted = Integer.parseInt(clientEntity.getProperty("PercentCompleted").getValue().toString());

        // TODO: fill real data
        task.StartDate = Calendar.getInstance();
        task.DateCompleted = Calendar.getInstance();
        task.DateCompleted.set(2017, 4, 233, 14, 32);

        ClientProperty startDateProperty = clientEntity.getProperty("StartDate");
        ClientProperty completeDateProperty = clientEntity.getProperty("DateCompleted");

        Log.d("ODATA", "start type: " + startDateProperty.getValue().getTypeName());
        Log.d("ODATA", "start value: " + startDateProperty.getValue().toString());

        Log.d("ODATA", "complete type: " + completeDateProperty.getValue().getTypeName());
        Log.d("ODATA", "complete value: " + completeDateProperty.getValue().toString());

        fillBaseSecurityEntity(task, clientEntity);

        return task;
    }
}
