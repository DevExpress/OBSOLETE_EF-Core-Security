package com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects;

import java.security.Timestamp;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collection;

/**
 * Created by neroslavskiy.a on 4/6/2016.
 */
public class DemoTask extends BaseSecurityEntity {
    public String Description;
    public String Note;
    public Calendar StartDate;
    public Calendar DateCompleted;
    public int PercentCompleted;
    public ArrayList<ContactTask> ContactTasks;
}