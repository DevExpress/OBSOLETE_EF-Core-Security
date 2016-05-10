package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.util.Log;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Contact;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.ContactTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.DemoTask;
import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.Department;

import org.apache.olingo.client.api.domain.ClientEntity;
import org.apache.olingo.client.api.domain.ClientPrimitiveValue;
import org.apache.olingo.client.api.domain.ClientProperty;
import org.apache.olingo.client.core.domain.ClientCollectionValueImpl;
import org.apache.olingo.commons.core.edm.primitivetype.EdmDateTimeOffset;

import java.net.URI;
import java.sql.Timestamp;
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

    public static Contact createContact(ClientEntity clientEntity, boolean withChildren, ODataEntityLoader loader) {
        Log.d("EntityCreator", "createContact");
        Contact contact = new Contact();
        contact.Name = clientEntity.getProperty("Name").getValue().toString();
        contact.Address = clientEntity.getProperty("Address").getValue().toString();
        contact.ContactTasks = new ArrayList<>();

        if (withChildren) {
            Log.d("EntityCreator", "load department for contact");
            URI departmentURI = loader.getProcessedURI(clientEntity.getNavigationLink("Department").getLink());
            ArrayList<BaseSecurityEntity> departments = loader.loadEntitiesFromUri(departmentURI, false);
            if(departments.size() == 1) {
                contact.Department = (Department)(departments.toArray()[0]);
            }

            Log.d("EntityCreator", "load tasks for contact");
            URI tasksURI = loader.getProcessedURI(clientEntity.getNavigationLink("ContactTasks").getLink());
            ArrayList<BaseSecurityEntity> tasks = loader.loadEntitiesFromUri(tasksURI, false);
            if(tasks.size() > 0) {
                for (BaseSecurityEntity task : tasks)
                    contact.ContactTasks.add((ContactTask)task);
            }
        }

        fillBaseSecurityEntity(contact, clientEntity);

        return contact;
    }

    public static Department createDepartment(ClientEntity clientEntity, boolean withChildren, ODataEntityLoader loader) {
        Log.d("EntityCreator", "createDepartment");
        Department department = new Department();
        department.Office = clientEntity.getProperty("Office").getValue().toString();
        department.Title = clientEntity.getProperty("Title").getValue().toString();

        fillBaseSecurityEntity(department, clientEntity);

        return department;
    }

    public static DemoTask createTask(ClientEntity clientEntity, boolean withChildren, ODataEntityLoader loader) {
        Log.d("EntityCreator", "createTask");
        DemoTask task = new DemoTask();
        task.Description = clientEntity.getProperty("Description").getValue().toString();
        task.Note = clientEntity.getProperty("Note").getValue().toString();
        task.PercentCompleted = Integer.parseInt(clientEntity.getProperty("PercentCompleted").getValue().toString());

        ClientProperty startDateProperty = clientEntity.getProperty("StartDate");
        ClientProperty completeDateProperty = clientEntity.getProperty("DateCompleted");

        Timestamp startDateTimeStamp = (Timestamp)(startDateProperty.getPrimitiveValue().toValue());
        Timestamp completeDateTimeStamp = (Timestamp)(completeDateProperty.getPrimitiveValue().toValue());

        task.StartDate = Calendar.getInstance();
        task.StartDate.setTimeInMillis(startDateTimeStamp.getTime());

        task.DateCompleted = Calendar.getInstance();
        task.DateCompleted.setTimeInMillis(completeDateTimeStamp.getTime());

        fillBaseSecurityEntity(task, clientEntity);

        return task;
    }

    public static ContactTask createContactTask(ClientEntity clientEntity, boolean withChildren, ODataEntityLoader loader) {
        ContactTask contactTask = new ContactTask();
        // TODO: fill with data
        fillBaseSecurityEntity(contactTask, clientEntity);

        return contactTask;
    }
}
