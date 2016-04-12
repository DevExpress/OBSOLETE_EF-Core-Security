package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.util.Log;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;

import org.apache.olingo.client.api.ODataClient;
import org.apache.olingo.client.api.communication.response.ODataRetrieveResponse;
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

    public ODataEntityLoader(ODataClient client) {
        this.client = client;
    }

    public ArrayList<BaseSecurityEntity> loadEntities(String entityName) {

        ArrayList<BaseSecurityEntity> loadedEntities = new ArrayList<>();

        // String serviceRoot = "http://192.168.50.200:800";
        String serviceRoot = "http://efcoresecurityodataservice20160407060012.azurewebsites.net";
        URI customersUri = client.newURIBuilder(serviceRoot)
                .appendEntitySetSegment(entityName).count(true).build();

        // ODataRetrieveResponse<ClientEntitySetIterator<ClientEntitySet, ClientEntity>> response
        //         = client.getRetrieveRequestFactory().getEntitySetIteratorRequest(customersUri).execute();


        ODataRetrieveResponse<ClientEntitySet> response = client.getRetrieveRequestFactory().getEntitySetRequest(customersUri).execute();

        ClientEntitySet entitySet = response.getBody();

        // publishProgress(entitySet.getCount());
        // loadedEntities.clear();

        for (ClientEntity entity : entitySet.getEntities()) {
            BaseSecurityEntity loadedEntity = null;

            if (entityName == "Contacts")
                loadedEntity = EntityCreator.createContact(entity);

            if (entityName == "Departments")
                loadedEntity = EntityCreator.createDepartment(entity);

            if (entityName == "Tasks")
                loadedEntity = EntityCreator.createTask(entity);

            try {
                Log.d("ODATA", "subprops for : " + entity.getTypeName());

                List<ClientLink> links = entity.getNavigationLinks();
                for (ClientLink link : links) {

                    Log.d("ODATA", "link: " + link.getName());
                    Log.d("ODATA", "link addr: " + link.getLink().toString());

                    URI linkUri = client.newURIBuilder(serviceRoot)
                            .appendNavigationSegment(link.getLink().toString()).build();
                    ODataRetrieveResponse<ClientEntitySetIterator<ClientEntitySet, ClientEntity>> responseOrders
                            = client.getRetrieveRequestFactory().getEntitySetIteratorRequest(linkUri).execute();
                    ClientEntitySetIterator<ClientEntitySet, ClientEntity> iteratorOrders = responseOrders.getBody();

                    /*
                    while (iteratorOrders.hasNext()) {
                        ClientEntity order = iteratorOrders.next();
                        List<ClientProperty> propertiesOrder = order.getProperties();
                        for (ClientProperty propertyOrder : propertiesOrder) {
                            ClientValue value = propertyOrder.getValue();

                            Log.d("ODATA", "subprop: " + propertyOrder.getName());
                            Log.d("ODATA", "subprop type: " + value.getTypeName());
                            Log.d("ODATA", "subprop value: " + value.toString());
                        }
                    }
                    */
                }
            } catch (Exception e) {
                // TODO: implement
                Log.d("EXCEPTION", e.getMessage());
            }

            if (loadedEntity != null)
                loadedEntities.add(loadedEntity);
        }

        return loadedEntities;

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
    }
}
