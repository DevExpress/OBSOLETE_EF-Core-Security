package com.devexpress.efcoresecurity.efcoresecuritydemo;

import android.content.res.Resources;
import android.graphics.Color;
import android.view.View;
import android.widget.TextView;

import com.devexpress.efcoresecurity.efcoresecuritydemo.businessobjects.BaseSecurityEntity;

/**
 * Created by unfo on 12.04.2016.
 */
public class SecurityHelper {
    Resources resources;

    SecurityHelper(Resources resources) {
        this.resources = resources;
    }

    public void setTextInTextView(View view, BaseSecurityEntity entity, String text, String fieldName, int colorFromResource, boolean useColor) {
        TextView textView = (TextView) view;
        if(entity.BlockedMembers.contains(fieldName)) {
            textView.setText(resources.getString(R.string.protected_content_text));
            textView.setTextColor(resources.getColor(R.color.protected_content_color));
        } else {
            textView.setText(text);
            if(useColor)
                textView.setTextColor(resources.getColor(colorFromResource));
        }
    }

    public void setTextInTextView(View view, BaseSecurityEntity entity, String text, String fieldName) {
        setTextInTextView(view, entity, text, fieldName, R.color.protected_content_color, false);
    }

}
