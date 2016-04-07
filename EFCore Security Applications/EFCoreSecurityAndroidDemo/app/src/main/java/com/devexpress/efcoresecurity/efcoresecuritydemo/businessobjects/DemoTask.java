package com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects;

import java.util.Collection;

/**
 * Created by neroslavskiy.a on 4/6/2016.
 */
public class DemoTask {
    public int Id;
    public String Description;
    public String Note;
    // public DateTime StartDate;
    // public DateTime DateCompleted;
    public int PercentCompleted;
    public Collection<ContactTask> ContactTasks;
}