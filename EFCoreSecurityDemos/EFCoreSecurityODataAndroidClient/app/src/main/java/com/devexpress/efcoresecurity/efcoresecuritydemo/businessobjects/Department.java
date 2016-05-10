package com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects;

import java.util.ArrayList;
import java.util.Collection;

/**
 * Created by neroslavskiy.a on 4/6/2016.
 */
public class Department extends BaseSecurityEntity {
    public String Title;
    public String Office;
    public ArrayList<Contact> Contacts;
}
