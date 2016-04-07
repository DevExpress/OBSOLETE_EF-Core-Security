package com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects;

import java.util.Collection;

/**
 * Created by neroslavskiy.a on 4/6/2016.
 */
public class Contact {
    public int Id;
    public String Name;
    public String Address;
    public Department Department;
    public Collection<ContactTask> ContactTasks;
}
