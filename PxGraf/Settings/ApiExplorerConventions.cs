using Microsoft.AspNetCore.Mvc.ApplicationModels;
using PxGraf.Controllers;

namespace PxGraf.Settings
{
    /// <summary>
    /// Class for setting visibility of API controllers in Swagger. CreationController is only visible if the CreationAPI feature is enabled.
    /// </summary>
    public class ApiExplorerConventions : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            // ErrorController has been set to be ignored in Swagger. Setting shows as null in the API explorer.
            if (action.ApiExplorer.IsVisible is null)
            {
                action.ApiExplorer.IsVisible = false;
                return;
            }

            bool isCreationController = action.Controller.ControllerType == typeof(CreationController);
            bool isCreationApiEnabled = Configuration.Current.CreationAPI;
            action.ApiExplorer.IsVisible = !isCreationController || isCreationApiEnabled;
        }
    }
}
