using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.RegularExpressions;

namespace SpringBoard.Api.Conventions;

/// <summary>
/// Route convention that transforms controller names to kebab-case in API routes
/// </summary>
public class KebabCaseControllerRouteConvention : IControllerModelConvention
{
    /// <summary>
    /// Applies the kebab-case transformation to controller route templates
    /// </summary>
    /// <param name="controller">The controller model to apply the convention to</param>
    public void Apply(ControllerModel controller)
    {
        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        // Only apply to controllers with the [Route] attribute
        var routeAttribute = controller.Selectors
            .FirstOrDefault(s => s.AttributeRouteModel != null)?.AttributeRouteModel;

        if (routeAttribute == null)
        {
            return;
        }

        // Get the original template
        var template = routeAttribute.Template;
        
        // If the template contains [controller], replace it with the kebab-case version
        if (template != null && template.Contains("[controller]", StringComparison.OrdinalIgnoreCase))
        {
            // Get the controller name without the "Controller" suffix
            var controllerName = controller.ControllerName;
            
            // Convert to kebab-case
            var kebabCaseName = ToKebabCase(controllerName);
            
            // Replace [controller] with the kebab-case name
            var newTemplate = template.Replace("[controller]", kebabCaseName, StringComparison.OrdinalIgnoreCase);
            
            // Update the template
            routeAttribute.Template = newTemplate;
        }
    }

    /// <summary>
    /// Converts a string from PascalCase to kebab-case
    /// </summary>
    /// <param name="value">The string to convert</param>
    /// <returns>The kebab-case version of the string</returns>
    private static string ToKebabCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        // Add a hyphen before each uppercase letter and convert to lowercase
        var result = Regex.Replace(value, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
        
        return result;
    }
}
