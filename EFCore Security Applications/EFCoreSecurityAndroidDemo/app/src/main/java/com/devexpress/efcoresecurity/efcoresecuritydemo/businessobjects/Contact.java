package com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects;

import java.util.ArrayList;
import java.util.Collection;

/**
 * Created by neroslavskiy.a on 4/6/2016.
 */
public class Contact extends BaseSecurityEntity {
    public String Name;
    public String Address;
    public Department Department;
    public ArrayList<ContactTask> ContactTasks;
}
