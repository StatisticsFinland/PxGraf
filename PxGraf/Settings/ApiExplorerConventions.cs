using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.FeatureManagement.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PxGraf.Settings
{
    /// <summary>
    /// Hides controllers and actions from the OpenAPI document when their <see cref="FeatureGateAttribute"/> feature flags are disabled.
    /// </summary>
    public class ApiExplorerConventions : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            // Controllers explicitly marked as ignored (e.g. ErrorController) show as null.
            if (action.ApiExplorer.IsVisible is null)
            {
                action.ApiExplorer.IsVisible = false;
                return;
            }

            // Collect FeatureGate attributes from both the controller and the action.
            IEnumerable<FeatureGateAttribute> gates = action.Controller.ControllerType.GetCustomAttributes<FeatureGateAttribute>(inherit: true)
                .Concat(action.ActionMethod.GetCustomAttributes<FeatureGateAttribute>(inherit: true));

            foreach (FeatureGateAttribute gate in gates)
            {
                if (gate.Features.Any(f => !Configuration.Current.IsFeatureEnabled(f)))
                {
                    action.ApiExplorer.IsVisible = false;
                    return;
                }
            }
        }
    }
}
