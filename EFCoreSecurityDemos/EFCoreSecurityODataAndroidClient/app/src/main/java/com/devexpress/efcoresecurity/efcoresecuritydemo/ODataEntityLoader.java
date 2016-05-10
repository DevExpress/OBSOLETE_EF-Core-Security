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

    private static final String serviceRoot = "http://192.168.50.200:800";
    // static final String serviceRoot = "https://efcoresecurityodataservice20160407060012.azurewebsites.net";

    public ODataEntityLoader(ODataClient client) {
        this.client = client;
    }

    public URI getProcessedURI(URI uri) {
        String linkAddress = uri.toString();
        linkAddress = linkAddress.replace("localhost:54342", "192.168.50.200:800");
        linkAddress = linkAddress.replace(serviceRoot, "");

        URI resultURI =  client.newURIBuilder(serviceRoot).
                appendNavigationSegment(linkAddress).build();

        Log.d("ODATA", "getProcessedURI : " + resultURI.toString());

        return resultURI;
    }

    public ArrayList<BaseSecurityEntity> loadEntities(String entityName) {
        URI customersUri = client.newURIBuilder(serviceRoot)
                .appendEntitySetSegment(entityName).count(true).build();

        Log.d("ODATA", "main url : " + customersUri.toString());

        return loadEntitiesFromUri(customersUri, true);
    }

    public ArrayList<BaseSecurityEntity> loadEntitiesFromUri(URI uri, boolean withChildren) {
        ArrayList<BaseSecurityEntity> loadedEntities = new ArrayList<>();

        ODataRetrieveResponse<ClientEntitySet> response = client.getRetrieveRequestFactory().getEntitySetRequest(uri).execute();
        ClientEntitySet entitySet = response.getBody();

        // publishProgress(entitySet.getCount());
        // loadedEntities.clear();

        for (ClientEntity entity : entitySet.getEntities()) {
            BaseSecurityEntity loadedEntity = null;

            String entityName = entity.getTypeName().getName();
            Log.d("ODATA", "typename : " + entityName);

            if (entityName.equals("Contact"))
                loadedEntity = EntityCreator.createContact(entity, withChildren, this);

            if (entityName.equals("Department"))
                loadedEntity = EntityCreator.createDepartment(entity, withChildren, this);

            if (entityName.equals("DemoTask"))
                loadedEntity = EntityCreator.createTask(entity, withChildren, this);

            if (entityName.equals("ContactTask"))
                loadedEntity = EntityCreator.createContactTask(entity, withChildren, this);

            /*
            try {
                Log.d("ODATA", "subprops for : " + entity.getTypeName());

                List<ClientLink> links = entity.getNavigationLinks();
                for (ClientLink link : links) {

                    String linkAddress = link.getLink().toString();
                    String navigationName = link.getName();

                    linkAddress = linkAddress.replace("localhost:54342", "192.168.50.200:800");
                    Log.d("ODATA", "link: " + link.getName());
                    Log.d("ODATA", "link addr: " + linkAddress);
                    Log.d("ODATA", "link rel: " + link.getRel());
                    Log.d("ODATA", "link metag: " + link.getMediaETag());

                    List<ClientAnnotation> list = link.getAnnotations();

                    for (ClientAnnotation a : list)
                        Log.d("ODATA", "link annot: " + a.toString());

                    String prcessedLinkAddress = linkAddress.replace(serviceRoot, "");

                    URI processedLinkUri = client.newURIBuilder(serviceRoot)
                            .appendNavigationSegment(prcessedLinkAddress).count(true).build();

                    Log.d("ODATA", "link big url : " + processedLinkUri.toString());

                    // URI linkUri = URI.create(linkAddress);

                    /*
                    ODataRetrieveResponse<ClientEntitySetIterator<ClientEntitySet, ClientEntity>> responseOrders
                            = client2.getRetrieveRequestFactory().getEntitySetIteratorRequest(linkUri).execute();
                    ClientEntitySetIterator<ClientEntitySet, ClientEntity> iteratorOrders = responseOrders.getBody();

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
            /*

                    ODataRetrieveResponse<ClientEntitySet> navigationContentResponse
                            = client.getRetrieveRequestFactory().getEntitySetRequest(processedLinkUri).execute();
                    ClientEntitySet navigationEntitySet = navigationContentResponse.getBody();

                    // Log.d("ODATA", "navs count: " + navigationEntitySet.getCount());

                    for (ClientEntity order : navigationEntitySet.getEntities()) {
                        List<ClientProperty> propertiesOrder = order.getProperties();
                        for (ClientProperty propertyOrder : propertiesOrder) {
                            ClientValue value = propertyOrder.getValue();

                            Log.d("ODATA", "subprop: " + propertyOrder.getName());
                            Log.d("ODATA", "subprop type: " + value.getTypeName());
                            Log.d("ODATA", "subprop value: " + value.toString());
                        }
                    }

                }
            } catch (Exception e) {
                // TODO: implement
                Log.d("EXCEPTION-subprops", e.getMessage());
            }
            */

            if (loadedEntity != null)
                loadedEntities.add(loadedEntity);
            else
                Log.d("Load Entities", "NULL");
        }

        return loadedEntities;
    }
}
