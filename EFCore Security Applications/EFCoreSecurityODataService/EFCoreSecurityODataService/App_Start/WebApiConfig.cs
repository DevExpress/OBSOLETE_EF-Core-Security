using EFCoreSecurityODataService.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using DevExpress.EntityFramework.SecurityDataStore.Security.BaseSecurityEntity;

namespace EFCoreSecurityODataService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            //config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //DataModelBuilder builder = new ODataConventionModelBuilder();
            //builder.EntitySet<Contact>("Contacts");
            //builder.EntitySet<DemoTask>("Tasks");
            //builder.EntitySet<Department>("Departments");
            //builder.EntitySet<Position>("Positions");
            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: GetEdmModel());
        }
        private static IEdmModel GetEdmModel() {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Contact>("Contacts");
            
            builder.EntitySet<DemoTask>("Tasks");
            builder.EntitySet<ContactTask>("ContactTasks");
            builder.EntitySet<Department>("Departments");

            foreach(var type in builder.StructuralTypes) {
                if(typeof(ISecurityEntity).IsAssignableFrom(type.ClrType)) {
                    type.AddCollectionProperty(typeof(ISecurityEntity).GetProperty("BlockedMembers"));
                }
            }

            IEdmModel edmModel = builder.GetEdmModel();
            //AddNavigations(edmModel); 
            return edmModel;
        }
        private static void AddNavigations(IEdmModel edmModel) {
            AddContactDepartmentNavigation(edmModel);
        }
        private static void AddContactDepartmentNavigation(IEdmModel edmModel) {
            EdmEntitySet contacts = (EdmEntitySet)edmModel.EntityContainer.FindEntitySet("Contacts");
            EdmEntitySet departments = (EdmEntitySet)edmModel.EntityContainer.FindEntitySet("Departments");
            EdmEntityType contact = (EdmEntityType)edmModel.FindDeclaredType("EFCoreSecurityODataService.Models.Contact");
            EdmEntityType department = (EdmEntityType)edmModel.FindDeclaredType("EFCoreSecurityODataService.Models.Department");
            AddDepartmentToContactNavigation("Departments", contacts, departments, contact, department);
            AddContactToDepartmentNavigation("Contacts", contacts, departments, contact, department);
        }

        private static void AddContactToDepartmentNavigation(string navTargetName, EdmEntitySet contactEntitySet, EdmEntitySet departmentEntitySet, EdmEntityType contactEntityType, EdmEntityType departmentEntityType) {
            EdmNavigationPropertyInfo navPropertyInfo = new EdmNavigationPropertyInfo {
                TargetMultiplicity = EdmMultiplicity.One,
                Target = departmentEntityType,
                ContainsTarget = false,
                OnDelete = EdmOnDeleteAction.None,
                Name = navTargetName
            };
            contactEntitySet.AddNavigationTarget(contactEntityType.AddUnidirectionalNavigation(navPropertyInfo), departmentEntitySet);
        }

        private static void AddDepartmentToContactNavigation(string navTargetName, EdmEntitySet contactEntitySet, EdmEntitySet departmentEntitySet, EdmEntityType contactEntityType, EdmEntityType departmentEntityType) {
            EdmNavigationPropertyInfo navPropertyInfo = new EdmNavigationPropertyInfo {
                TargetMultiplicity = EdmMultiplicity.One,
                Target = departmentEntityType,
                ContainsTarget = false,
                OnDelete = EdmOnDeleteAction.None,
                Name = navTargetName
            };
            departmentEntitySet.AddNavigationTarget(departmentEntityType.AddUnidirectionalNavigation(navPropertyInfo), contactEntitySet);
        }
    }
}
