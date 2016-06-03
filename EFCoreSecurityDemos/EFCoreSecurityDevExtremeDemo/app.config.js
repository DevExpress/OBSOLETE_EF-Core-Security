window.TestApp = $.extend(true, window.TestApp, {
  "config": {
    "layoutSet": "navbar",
    "navigation": [
      {
          "title": "Login",
          "action": "#Login",
          "icon": "user"
      },
      {
        "title": "Contacts",
        "action": "#Contacts",
        "icon": "home"
      },      
      {
        "title": "Departments",
        "onExecute": "#Departments",
        "icon": "home"
      },
      {
        "title": "Tasks",
        "onExecute": "#Tasks",
        "icon": "home"
      },
      {
        "title": "About",
        "action": "#About",
        "icon": "info"
      }
    ]
  }
});