using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo {
    public class MenuPage : ContentPage {
        public ListView Menu { get; set; }
        public MenuPage() {
            // Icon = "settings.png";
            Title = "menu";
            BackgroundColor = Color.FromHex("333333");

            Menu = new MenuListView();

            var menuLabel = new ContentView {
                Padding = new Thickness(10, 36, 0, 5),
                Content = new Label {
                    TextColor = Color.FromHex("AAAAAA"),
                    Text = "MENU"
                }
            };

            var layout = new StackLayout {
                Spacing = 0,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            layout.Children.Add(menuLabel);
            layout.Children.Add(Menu);

            Content = layout;
        }
    }

    public class MenuListView : ListView {
        public MenuListView() {
            List<MenuItem> data = new List<MenuItem> {
                new MenuItem() {
                    Text = "Contacts"
                },
                new MenuItem() {
                    Text = "Departments"
                },
                new MenuItem() {
                    Text = "Tasks"
                }
            };

            ItemsSource = data;

            SelectedItem = data[0];

            VerticalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = Color.Transparent;

            var cell = new DataTemplate(typeof(ImageCell));
            cell.SetBinding(TextCell.TextProperty, "Text");

            ItemTemplate = cell;
        }
    }
}
