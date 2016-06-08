package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.util.Log;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;

import org.apache.olingo.client.api.ODataClient;
import org.apache.olingo.client.api.communication.response.ODataRetrieveResponse;
import org.apache.olingo.client.api.domain.ClientAnnotation;
import org.apache.olingo.client.api.domain.ClientEntity;
import org.apache.olingo.client.api.domain.ClientEntitySet;
import org.apache.olingo.client.api.domain.ClientEntitySetIterator;
import org.apache.olingo.client.api.domain.ClientLink;
import org.apache.olingo.client.api.domain.ClientProperty;
import org.apache.olingo.client.api.domain.ClientValue;

import java.net.URI;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by unfo on 12.04.2016.
 */
public class ODataEntityLoader {
    private final ODataClient client;

    // private static final String serviceRoot = "http://192.168.50.200:800";
    private static final String serviceRoot = "http://efcoresecurityodataservicedemo.azurewebsites.net/";

    public ODataEntityLoader(ODataClient client) {
        this.client = client;
    }

    public URI getProcessedURI(URI uri) {
        String linkAddress = uri.toString();
        linkAddress = linkAddress.replace("localhost:54342", "192.168.50.200:800");
        linkAddress = linkAddress.replace(serviceRoot, "");

        URI resultURI =  client.newURIBuilder(serviceRoot).
                appendNavigationSegment(linkAddress).build();

        return resultURI;
    }

    public ArrayList<BaseSecurityEntity> loadEntities(String entityName) {
        URI customersUri = client.newURIBuilder(serviceRoot)
                .appendEntitySetSegment(entityName).count(true).build();

        return loadEntitiesFromUri(customersUri, true);
    }

    public ArrayList<BaseSecurityEntity> loadEntitiesFromUri(URI uri, boolean withChildren) {
        ArrayList<BaseSecurityEntity> loadedEntities = new ArrayList<>();

        ODataRetrieveResponse<ClientEntitySet> response = client.getRetrieveRequestFactory().getEntitySetRequest(uri).execute();
        ClientEntitySet entitySet = response.getBody();

        for (ClientEntity entity : entitySet.getEntities()) {
            BaseSecurityEntity loadedEntity = null;

            String entityName = entity.getTypeName().getName();

            if (entityName.equals("Contact"))
                loadedEntity = EntityCreator.createContact(entity, withChildren, this);

            if (entityName.equals("Department"))
                loadedEntity = EntityCreator.createDepartment(entity, withChildren, this);

            if (entityName.equals("DemoTask"))
                loadedEntity = EntityCreator.createTask(entity, withChildren, this);

            if (entityName.equals("ContactTask"))
                loadedEntity = EntityCreator.createContactTask(entity, withChildren, this);

            if (loadedEntity != null)
                loadedEntities.add(loadedEntity);
            else
                Log.d("Load Entities", "NULL");
        }

        return loadedEntities;
    }
}
