package com.devexpress.efcoresecurity.efcoresecuritydemo.authentication;

import org.apache.http.auth.AuthScope;
import org.apache.http.auth.UsernamePasswordCredentials;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.olingo.client.core.http.DefaultHttpClientFactory;
import org.apache.olingo.commons.api.http.HttpMethod;

import java.net.URI;

/**
 * Created by unfo on 14.04.2016.
 */
public class PreemptiveBasicAuthHttpClientFactory extends DefaultHttpClientFactory {
    private final String username;
    private final String password;

    public PreemptiveBasicAuthHttpClientFactory(String username, String password) {
        this.username = username;
        this.password = password;
    }

    @Override
    public DefaultHttpClient create(HttpMethod method, URI uri) {
        DefaultHttpClient httpclient = super.create(method, uri);
        httpclient.getCredentialsProvider().setCredentials(new AuthScope(uri.getHost(), uri.getPort()),
                new UsernamePasswordCredentials(this.username, this.password));

        httpclient.addRequestInterceptor(new PreemptiveAuthInterceptor(), 0);

        return httpclient;
    }
}
