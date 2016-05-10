package com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.Collection;

/**
 * Created by neroslavskiy.a on 4/7/2016.
 */
public class BaseSecurityEntity implements Serializable {
    public int Id;
    public ArrayList<String> BlockedMembers;
}
