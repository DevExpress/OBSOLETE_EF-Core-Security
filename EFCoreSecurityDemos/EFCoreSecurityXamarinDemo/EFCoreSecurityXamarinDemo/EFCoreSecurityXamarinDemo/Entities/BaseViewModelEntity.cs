using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EFCoreSecurityXamarinDemo.Entities {
    public class BaseViewModelEntity {
        public string ProtectedContextText { get; set; } = "Protected";
        public Color NormalColor { get; set; } = Color.Default;
        public Color BlockedColor { get; set; } = Color.FromHex("#e7842d");
        public BaseSecurityEntity Entity { get; set; }        

        public BaseViewModelEntity(BaseSecurityEntity entity) {
            Entity = entity;
        }
        protected string GetPropertyText(string propertyName, string propertyValue) {
            string resultValue = "";
            if (Entity != null) {
                resultValue = Entity.BlockedMembers.Contains(propertyName) ? ProtectedContextText : propertyValue;
            }

            return resultValue;
        }
        protected Color GetPropertyTextColor(string propertyName) {
            Color resultTextColor = NormalColor;
            if(Entity != null) {
                resultTextColor = Entity.BlockedMembers.Contains(propertyName) ? BlockedColor : NormalColor;
            }

            return resultTextColor;
        }

        public string Id { get {
                return GetPropertyText("Id", Entity.Id.ToString());
            } 
        }
        public Color IdTextColor {
            get {
                return GetPropertyTextColor("Id");
            }
        }
    }
}
